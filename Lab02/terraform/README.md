# UrbanFix — Terraform (Lab06)

Dwuetapowy deployment UrbanFix na AWS przy użyciu Terraforma.

## Wymagania (na maszynie deweloperskiej)

- Terraform >= 1.6
- AWS CLI skonfigurowane (`AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`, `AWS_SESSION_TOKEN`)
- Docker (działający daemon) — do budowania i pushowania obrazów w Stage 1
- PowerShell

## Etap 1 — Infrastruktura

Tworzy: RDS PostgreSQL, 5 baz danych, S3 bucket, 6 repozytoriów ECR.

```bash
cd terraform/01-infra
cp terraform.tfvars.example terraform.tfvars
# wypełnij terraform.tfvars (co najmniej db_password)
terraform init
terraform apply
```

## Krok pośredni — push obrazów Docker do ECR

Buduje 6 obrazów lokalnie i pushuje je do ECR utworzonego w Etapie 1.

```powershell
cd terraform
.\push_images.ps1
```

> Pierwsze budowanie trwa kilka minut (pobieranie .NET SDK image i restore NuGet packages).

## Etap 2 — Deployment mikroserwisów na EC2

Tworzy: EC2 t3.small (Amazon Linux 2023) z docker-compose uruchamiającym wszystkie mikroserwisy.

```bash
cd terraform/02-app
cp terraform.tfvars.example terraform.tfvars
# wypełnij terraform.tfvars (db_password + AWS credentials dla S3)
terraform init
terraform apply
```

Po ~2 minutach od zakończenia `apply` (EC2 startuje kontenery):

- API Gateway / Swagger UI: `http://<app_public_dns>:5200`
- MailHog UI: `http://<app_public_dns>:8025`

Adresy dostępne jako outputy Terraforma:
```bash
terraform output api_gateway_url
```

## Czyszczenie

```bash
cd terraform/02-app && terraform destroy
cd ../01-infra && terraform destroy
```

## Pliki

```
terraform/
├── README.md
├── .gitignore
├── push_images.ps1        ← krok pośredni (PowerShell)
├── 01-infra/
│   ├── main.tf            ← provider aws + random + null
│   ├── variables.tf
│   ├── rds.tf             ← RDS Postgres + security group
│   ├── databases.tf       ← tworzy 5 baz przez null_resource + psql w docker
│   ├── s3.tf              ← S3 bucket na zdjęcia zgłoszeń
│   ├── ecr.tf             ← 6 repozytoriów ECR
│   ├── outputs.tf         ← rds_address, ecr_registry, s3_bucket_name, ...
│   └── terraform.tfvars.example
└── 02-app/
    ├── main.tf            ← provider aws + remote_state z 01-infra
    ├── variables.tf
    ├── ec2.tf             ← AMI AL2023, t3.small, SG (22/5200/8025), LabInstanceProfile
    ├── user_data.sh.tftpl ← bootstrap EC2: instaluje docker, loguje ECR, startuje compose
    ├── outputs.tf         ← app_public_dns, api_gateway_url, swagger_url
    └── terraform.tfvars.example
```

## Uwagi

- **LearnerLab**: credentials (session token) wygasają co ~4h — przed `apply` zaktualizuj `.env`
  w AWS Academy i przekopiuj nowe wartości do `terraform.tfvars` w 02-app.
- **RDS** jest publicamente dostępne (demo). Hasło w `terraform.tfvars` — plik jest w `.gitignore`.
- Obrazy pushuje się skryptem `push_images.ps1` (PowerShell) między Stage 1 a Stage 2.
