# Push all UrbanFix Docker images to ECR.
# Run this AFTER "terraform apply" in 01-infra and BEFORE "terraform apply" in 02-app.
#
# Usage:
#   cd terraform
#   .\push_images.ps1

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$ScriptDir  = Split-Path -Parent $MyInvocation.MyCommand.Path
$InfraDir   = Join-Path $ScriptDir "01-infra"
$UrbanFix   = Join-Path $ScriptDir "..\UrbanFix"

# Read outputs from Stage 1
Push-Location $InfraDir
$Registry = terraform output -raw ecr_registry
$Region   = "us-east-1"
Pop-Location

Write-Host "=== Logging in to ECR ($Registry) ===" -ForegroundColor Cyan
$Password = aws ecr get-login-password --region $Region
docker login --username AWS --password $Password $Registry

# api-gateway has its own build context (no UrbanFix.Common dependency)
Write-Host "=== Building api-gateway ===" -ForegroundColor Cyan
docker build -t "${Registry}/urbanfix-api-gateway:latest" `
    -f "$UrbanFix\ApiGateway\Dockerfile" `
    "$UrbanFix\ApiGateway"
docker push "${Registry}/urbanfix-api-gateway:latest"

# Microservices are built from the UrbanFix root (UrbanFix.Common is in the context)
$Services = @{
    "report"       = "UrbanFix.ReportService"
    "verification" = "UrbanFix.VerificationService"
    "assignment"   = "UrbanFix.AssignmentService"
    "timeline"     = "UrbanFix.TimelineService"
    "notification" = "UrbanFix.NotificationService"
}

foreach ($svc in $Services.Keys) {
    $proj = $Services[$svc]
    Write-Host "=== Building $svc ($proj) ===" -ForegroundColor Cyan
    docker build -t "${Registry}/urbanfix-${svc}:latest" `
        -f "$UrbanFix\$proj\Dockerfile" `
        "$UrbanFix"
    docker push "${Registry}/urbanfix-${svc}:latest"
}

Write-Host "=== All images pushed to ECR ===" -ForegroundColor Green
