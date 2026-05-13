data "aws_caller_identity" "current" {}

locals {
  ecr_registry = "${data.aws_caller_identity.current.account_id}.dkr.ecr.${var.aws_region}.amazonaws.com"
}

output "rds_address" {
  description = "RDS hostname (without port)"
  value       = aws_db_instance.urbanfix.address
}

output "rds_endpoint" {
  description = "RDS endpoint (host:port)"
  value       = aws_db_instance.urbanfix.endpoint
}

output "s3_bucket_name" {
  description = "S3 bucket name for report images"
  value       = aws_s3_bucket.report.bucket
}

output "ecr_registry" {
  description = "ECR registry base URL"
  value       = local.ecr_registry
}

output "ecr_repository_urls" {
  description = "Map of service name to ECR repository URL"
  value       = { for k, r in aws_ecr_repository.repos : k => r.repository_url }
}
