# Email Service Configuration - Development vs Production

## Overview

The NotificationService email implementation supports **automatic provider switching** based on environment configuration:
- **Development:** SMTP (Mailhog or local SMTP server)
- **Production:** AWS SES (Simple Email Service)

**No code changes required** - just configuration!

---

## Development Setup (Local Testing)

### Requirements
- Mailhog running: `docker run -p 1025:1025 -p 8025:8025 mailhog/mailhog`

### Configuration
**File:** `appsettings.json` (already configured)

```json
{
  "Email": {
    "Provider": "Smtp",
    "SmtpHost": "localhost",
    "SmtpPort": "1025",
    "FromEmail": "noreply@urbanfix.pl"
  }
}
```

### How it works
1. EmailService receives DI request
2. Factory checks `Email:Provider` setting
3. If Provider = "Smtp" → Uses `SmtpEmailProvider` (MailKit)
4. Emails sent to Mailhog on localhost:1025
5. View emails at http://localhost:8025

### Testing locally
```powershell
# Start Mailhog
docker run -p 1025:1025 -p 8025:8025 mailhog/mailhog

# Run NotificationService with Development environment
dotnet run

# Send test email (via Postman or workflow)
# View email at http://localhost:8025
```

---

## Production Setup (AWS)

### Architecture
```
NotificationService
        ↓
EmailService (IEmailService)
        ↓
EmailProviderFactory
        ↓
AwsSesEmailProvider
        ↓
AWS SES (Simple Email Service)
        ↓
Email delivered to recipient
```

### Requirements
- AWS Account with SES configured
- IAM user with `ses:SendEmail` permission
- Verified sender email in AWS SES (noreply@urbanfix.pl)
- AWS credentials (AccessKey, SecretKey, SessionToken)

### Configuration
**File:** `appsettings.Production.json`

```json
{
  "Email": {
    "Provider": "AwsSes",
    "FromEmail": "noreply@urbanfix.pl"
  },
  "AWS": {
    "Region": "eu-central-1",
    "AccessKey": "AKIA...",
    "SecretKey": "...",
    "SessionToken": "..." // Optional for temporary credentials
  }
}
```

### Environment Variables (Recommended for Docker/AWS)
Instead of hardcoding credentials in config, use environment variables:

```bash
# .env file or ECS Task Definition / Lambda environment
AWS_REGION=eu-central-1
AWS_ACCESS_KEY_ID=AKIA...
AWS_SECRET_ACCESS_KEY=...
AWS_SESSION_TOKEN=... # Optional

EMAIL_PROVIDER=AwsSes
EMAIL_FROM=noreply@urbanfix.pl
```

### Docker Image Configuration
When deploying to AWS (ECS, Lambda, etc.):

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0

# Copy application
COPY bin/Release/net10.0/publish /app
WORKDIR /app

# Set production environment
ENV ASPNETCORE_ENVIRONMENT=Production

# Credentials injected at runtime via ECS Task Definition or Secrets Manager
ENTRYPOINT ["dotnet", "UrbanFix.NotificationService.dll"]
```

### AWS Secrets Manager Integration (Optional)
For enhanced security, use AWS Secrets Manager instead of environment variables:

```csharp
// This would be added to Program.cs if using Secrets Manager
var secretsManager = new AmazonSecretsManagerClient(region: RegionEndpoint.EUCentral1);
var secret = await secretsManager.GetSecretValueAsync(new GetSecretValueRequest 
{ 
    SecretId = "urbanfix/notification-service" 
});
var credentialsJson = JsonSerializer.Deserialize<Dictionary<string, string>>(secret.SecretString);
```

---

## Switching Environments

### Development → Local SMTP
```json
// appsettings.json
"Email": {
  "Provider": "Smtp",
  "SmtpHost": "localhost",
  "SmtpPort": "1025",
  "FromEmail": "noreply@urbanfix.pl"
}
```

### Production → AWS SES
```json
// appsettings.Production.json
"Email": {
  "Provider": "AwsSes",
  "FromEmail": "noreply@urbanfix.pl"
}
```

**That's it!** No code changes needed.

---

## Implementation Details

### Factory Pattern
The `EmailProviderFactory` implements the Factory Pattern:

```csharp
public IEmailProvider CreateProvider()
{
    var provider = _configuration["Email:Provider"] ?? "Smtp";

    if (provider.Equals("AwsSes", StringComparison.OrdinalIgnoreCase))
        return _serviceProvider.GetRequiredService<AwsSesEmailProvider>();

    return _serviceProvider.GetRequiredService<SmtpEmailProvider>();
}
```

### Interfaces
```csharp
IEmailProvider          // Abstraction for email providers
├─ SmtpEmailProvider    // SMTP implementation (Mailhog/local)
└─ AwsSesEmailProvider  // AWS SES implementation

IEmailService           // Main service interface
└─ EmailService         // Implementation using factory
```

### Dependency Injection
```csharp
// Program.cs registration
builder.Services.AddScoped<SmtpEmailProvider>();
builder.Services.AddScoped<AwsSesEmailProvider>();
builder.Services.AddScoped<IEmailProviderFactory, EmailProviderFactory>();
builder.Services.AddScoped<IEmailService, EmailService>();
```

---

## Troubleshooting

### Emails not sending in Development
1. Check Mailhog is running: `docker ps | grep mailhog`
2. Check SMTP connection: http://localhost:8025
3. Check logs for SMTP errors
4. Verify `Email:Provider` is set to "Smtp"

### Emails not sending in Production (AWS)
1. Verify AWS credentials are correct
2. Check IAM permissions: `ses:SendEmail`
3. Verify sender email is verified in AWS SES
4. Check AWS region matches configuration
5. Check CloudWatch logs for SES errors
6. Verify email is not in SES Sandbox mode (production accounts only)

### AWS SES Sandbox Mode
If using a new AWS account, SES starts in Sandbox mode:
- Can only send TO verified emails
- Can only send FROM verified emails
- Limited sending quota (200 emails/day)

To request production access:
1. Go to AWS SES Console
2. Click "Edit account details"
3. Fill out production access request
4. AWS will review and enable

---

## Cost Considerations

### Development (Mailhog)
- **Cost:** FREE
- **Limitations:** Local only, no actual delivery

### Production (AWS SES)
- **Cost:** $0.10 per 1,000 emails (first 62,000 free per month)
- **Benefits:** Automatic retries, delivery tracking, compliance

---

## Migration from Development to Production

1. Deploy Docker image to AWS (ECS/Lambda)
2. Set environment: `ASPNETCORE_ENVIRONMENT=Production`
3. Set AWS credentials via Secrets Manager or environment variables
4. Application automatically switches to AWS SES
5. ✅ No code changes needed!

---

**Summary:** Your application is environment-agnostic. Deploy the same code everywhere! 🚀
