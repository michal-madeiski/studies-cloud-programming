resource "null_resource" "create_databases" {
  depends_on = [aws_db_instance.urbanfix]

  triggers = {
    db_instance_id = aws_db_instance.urbanfix.id
  }

  provisioner "local-exec" {
    interpreter = ["PowerShell", "-Command"]
    command     = <<-EOT
      $ErrorActionPreference = "Continue"
      $conn = "postgresql://${var.db_username}:${var.db_password}@${aws_db_instance.urbanfix.address}/postgres?sslmode=require"
      foreach ($db in @("db_report_tf","db_verification_tf","db_assignment_tf","db_timeline_tf","db_notification_tf")) {
        Write-Host "Creating database: $db"
        docker run --rm postgres:15 psql $conn -c "CREATE DATABASE `"$db`";"
        if ($LASTEXITCODE -ne 0) { Write-Host "  -> $db already exists, continuing." }
      }
    EOT
  }
}
