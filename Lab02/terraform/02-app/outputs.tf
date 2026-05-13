output "app_public_ip" {
  description = "Public IP of the EC2 instance"
  value       = aws_instance.app.public_ip
}

output "app_public_dns" {
  description = "Public DNS of the EC2 instance"
  value       = aws_instance.app.public_dns
}

output "api_gateway_url" {
  description = "UrbanFix API Gateway URL"
  value       = "http://${aws_instance.app.public_dns}:5200"
}

output "swagger_url" {
  description = "Swagger UI URL (available ~2 min after apply)"
  value       = "http://${aws_instance.app.public_dns}:5200/swagger"
}

output "mailhog_url" {
  description = "MailHog web UI"
  value       = "http://${aws_instance.app.public_dns}:8025"
}
