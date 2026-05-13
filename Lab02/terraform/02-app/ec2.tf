data "aws_ami" "al2023" {
  most_recent = true
  owners      = ["amazon"]

  filter {
    name   = "name"
    values = ["al2023-ami-*-x86_64"]
  }

  filter {
    name   = "virtualization-type"
    values = ["hvm"]
  }
}

data "aws_vpc" "default" {
  default = true
}

data "aws_subnets" "default" {
  filter {
    name   = "vpc-id"
    values = [data.aws_vpc.default.id]
  }
}

resource "aws_security_group" "app" {
  name        = "urbanfix-app-sg"
  description = "UrbanFix application ports"
  vpc_id      = data.aws_vpc.default.id

  ingress {
    description = "SSH"
    from_port   = 22
    to_port     = 22
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  ingress {
    description = "API Gateway"
    from_port   = 5200
    to_port     = 5200
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  ingress {
    description = "Microservices (direct swagger access)"
    from_port   = 5201
    to_port     = 5205
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  ingress {
    description = "MailHog UI"
    from_port   = 8025
    to_port     = 8025
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = { Project = "UrbanFix" }
}

locals {
  infra        = data.terraform_remote_state.infra.outputs
  ecr_registry = local.infra.ecr_registry
  ecr_urls     = local.infra.ecr_repository_urls
}

resource "aws_instance" "app" {
  ami                         = data.aws_ami.al2023.id
  instance_type               = "t3.small"
  subnet_id                   = tolist(data.aws_subnets.default.ids)[0]
  vpc_security_group_ids      = [aws_security_group.app.id]
  associate_public_ip_address = true
  iam_instance_profile        = "LabInstanceProfile"
  key_name                    = var.ssh_key_name != "" ? var.ssh_key_name : null
  user_data_replace_on_change = true

  root_block_device {
    volume_size = 20
    volume_type = "gp3"
  }

  user_data = templatefile("${path.module}/user_data.sh.tftpl", {
    aws_region        = var.aws_region
    ecr_registry      = local.ecr_registry
    ecr_api_gateway   = local.ecr_urls["api-gateway"]
    ecr_report        = local.ecr_urls["report"]
    ecr_verification  = local.ecr_urls["verification"]
    ecr_assignment    = local.ecr_urls["assignment"]
    ecr_timeline      = local.ecr_urls["timeline"]
    ecr_notification  = local.ecr_urls["notification"]
    db_host           = local.infra.rds_address
    db_username       = var.db_username
    db_password       = var.db_password
    mq_conn           = var.mq_conn
    cognito_authority = var.cognito_authority
    cognito_client_id = var.cognito_client_id
    aws_access_key    = var.aws_access_key
    aws_secret_key    = var.aws_secret_key
    aws_session_token = var.aws_session_token
    s3_bucket_name    = local.infra.s3_bucket_name
    s3_bucket_folder  = var.aws_bucket_folder
  })

  tags = {
    Name    = "urbanfix-app"
    Project = "UrbanFix"
  }
}
