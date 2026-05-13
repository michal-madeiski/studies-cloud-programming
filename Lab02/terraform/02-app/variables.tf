variable "aws_region" {
  description = "AWS region"
  default     = "us-east-1"
}

variable "db_username" {
  description = "RDS master username (must match Stage 1)"
  default     = "postgres"
}

variable "db_password" {
  description = "RDS master password (must match Stage 1)"
  sensitive   = true
}

# CloudAMQP — stays as external service
variable "mq_conn" {
  description = "RabbitMQ connection string (CloudAMQP)"
  default     = "amqps://emsntwqm:jos0dE2QCIoKzfwqxA4koeis8zNrct-E@cow.rmq2.cloudamqp.com/emsntwqm"
}

# Cognito (existing User Pool — not managed by Terraform)
variable "cognito_authority" {
  description = "Cognito authority URL"
  default     = "https://cognito-idp.us-east-1.amazonaws.com/us-east-1_wMB0xF8DJ"
}

variable "cognito_client_id" {
  description = "Cognito app client ID"
  default     = "5j9aoe1r76vdaacgntoin09kl9"
}

# AWS credentials passed to ReportService for S3 access.
# In LearnerLab these are short-lived session credentials.
variable "aws_access_key" {
  description = "AWS Access Key ID (for microservice S3 access)"
  sensitive   = true
}

variable "aws_secret_key" {
  description = "AWS Secret Access Key (for microservice S3 access)"
  sensitive   = true
}

variable "aws_session_token" {
  description = "AWS Session Token (LearnerLab short-lived token)"
  sensitive   = true
  default     = ""
}

variable "aws_bucket_folder" {
  description = "S3 subfolder for report images"
  default     = "img"
}

# Optional: name of an existing EC2 key pair for SSH access
variable "ssh_key_name" {
  description = "Name of existing EC2 Key Pair for SSH (leave empty to skip)"
  default     = ""
}
