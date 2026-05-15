# 🚀 Deployment & Production Guide

## Pre-Deployment Checklist

### Security
- [ ] Change JWT secret key to a strong, random value
- [ ] Update CORS policy for production domains only
- [ ] Enable HTTPS (set `RequireHttpsMetadata = true`)
- [ ] Review and secure connection strings
- [ ] Remove or secure Swagger in production
- [ ] Enable rate limiting
- [ ] Implement API versioning
- [ ] Add request size limits

### Database
- [ ] Review all migrations
- [ ] Create database backup strategy
- [ ] Set up database connection pooling
- [ ] Configure database monitoring
- [ ] Plan for database scaling

### Performance
- [ ] Enable response caching where appropriate
- [ ] Configure compression
- [ ] Optimize database queries
- [ ] Add database indexes
- [ ] Configure connection string pooling

### Monitoring
- [ ] Set up application logging
- [ ] Configure error tracking (e.g., Sentry, Application Insights)
- [ ] Set up performance monitoring
- [ ] Create health check endpoints
- [ ] Configure alerts

---

## Production Configuration

### appsettings.Production.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-server;Database=RealEstateDB;User Id=dbuser;Password=<strong-password>;TrustServerCertificate=False;Encrypt=True"
  },
  "Jwt": {
    "Key": "<generate-a-very-strong-secret-key-at-least-64-characters>",
    "Issuer": "RealEstateAPI-Production",
    "Audience": "RealEstateAPIUsers-Production"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Warning",
      "Override": {
        "Microsoft": "Error",
        "System": "Error"
      }
    },
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "/var/log/realestate-api/log-.txt",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30
        }
      }
    ]
  },
  "AllowedHosts": "yourdomain.com"
}
```

---

## Docker Deployment

### Dockerfile

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore
COPY ["API/RealEstateAPI.API.csproj", "API/"]
COPY ["Application/RealEstateAPI.Application.csproj", "Application/"]
COPY ["Infrastructure/RealEstateAPI.Infrastructure.csproj", "Infrastructure/"]
COPY ["Domain/RealEstateAPI.Domain.csproj", "Domain/"]

RUN dotnet restore "API/RealEstateAPI.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/API"
RUN dotnet build "RealEstateAPI.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "RealEstateAPI.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RealEstateAPI.API.dll"]
```

### docker-compose.yml

```yaml
version: '3.8'

services:
  api:
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "5000:80"
      - "5001:443"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=https://+:443;http://+:80
      - ASPNETCORE_Kestrel__Certificates__Default__Password=<certificate-password>
      - ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
    volumes:
      - ./logs:/var/log/realestate-api
      - ~/.aspnet/https:/https:ro
    depends_on:
      - db
    networks:
      - realestate-network

  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Passw0rd
      - MSSQL_PID=Express
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql
    networks:
      - realestate-network

volumes:
  sqldata:

networks:
  realestate-network:
    driver: bridge
```

### Build and Run

```bash
# Build the image
docker-compose build

# Run the containers
docker-compose up -d

# View logs
docker-compose logs -f api

# Stop containers
docker-compose down
```

---

## Azure Deployment

### Azure App Service

1. **Create Resources:**
```bash
# Login to Azure
az login

# Create resource group
az group create --name RealEstateAPI-RG --location eastus

# Create App Service plan
az appservice plan create --name RealEstateAPI-Plan --resource-group RealEstateAPI-RG --sku B1 --is-linux

# Create Web App
az webapp create --name realestate-api-prod --resource-group RealEstateAPI-RG --plan RealEstateAPI-Plan --runtime "DOTNETCORE:8.0"

# Create SQL Database
az sql server create --name realestate-sql-server --resource-group RealEstateAPI-RG --location eastus --admin-user sqladmin --admin-password <strong-password>

az sql db create --resource-group RealEstateAPI-RG --server realestate-sql-server --name RealEstateDB --service-objective S0
```

2. **Configure App Settings:**
```bash
az webapp config appsettings set --name realestate-api-prod --resource-group RealEstateAPI-RG --settings \
  ASPNETCORE_ENVIRONMENT="Production" \
  Jwt__Key="<your-secret-key>" \
  ConnectionStrings__DefaultConnection="<your-connection-string>"
```

3. **Deploy:**
```bash
# Publish the app
dotnet publish -c Release -o ./publish

# Create deployment package
cd publish
zip -r ../deploy.zip .
cd ..

# Deploy to Azure
az webapp deployment source config-zip --resource-group RealEstateAPI-RG --name realestate-api-prod --src deploy.zip
```

---

## AWS Deployment (Elastic Beanstalk)

### Prerequisites
- AWS CLI installed
- EB CLI installed

### Steps

1. **Initialize Elastic Beanstalk:**
```bash
eb init -p "64bit Amazon Linux 2 v2.5.0 running .NET Core" realestate-api --region us-east-1
```

2. **Create Environment:**
```bash
eb create realestate-api-prod --database.engine sqlserver-ex
```

3. **Configure Environment Variables:**
```bash
eb setenv ASPNETCORE_ENVIRONMENT=Production \
  Jwt__Key=<your-secret-key> \
  ConnectionStrings__DefaultConnection=<your-connection-string>
```

4. **Deploy:**
```bash
dotnet publish -c Release
eb deploy
```

---

## Database Migrations in Production

### Automated Migration (Recommended for small apps)

Add to `Program.cs`:
```csharp
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();
        
        // Optionally seed data
        if (app.Environment.IsDevelopment())
        {
            await DbInitializer.SeedAsync(context);
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database");
    }
}
```

### Manual Migration (Recommended for production)

```bash
# Generate SQL script
dotnet ef migrations script --output migration.sql --idempotent

# Review and run the script manually on production database
```

---

## Health Checks

### Add Health Check Endpoint

Install package:
```bash
dotnet add package Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore
```

Add to `ServiceExtensions.cs`:
```csharp
public static IServiceCollection AddHealthChecks(this IServiceCollection services)
{
    services.AddHealthChecks()
        .AddDbContextCheck<ApplicationDbContext>();
    
    return services;
}
```

Add to `Program.cs`:
```csharp
app.MapHealthChecks("/health");
```

---

## Monitoring & Logging

### Application Insights (Azure)

1. **Install Package:**
```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

2. **Configure in Program.cs:**
```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

3. **Add to appsettings.json:**
```json
{
  "ApplicationInsights": {
    "InstrumentationKey": "<your-instrumentation-key>"
  }
}
```

---

## Performance Optimization

### Response Caching

Add to `Program.cs`:
```csharp
builder.Services.AddResponseCaching();
app.UseResponseCaching();
```

Add to controller methods:
```csharp
[ResponseCache(Duration = 300)] // Cache for 5 minutes
public async Task<IActionResult> GetAll()
```

### Response Compression

Add to `Program.cs`:
```csharp
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

app.UseResponseCompression();
```

### Database Indexing

Add indexes to frequently queried columns:
```csharp
// In ApplicationDbContext
modelBuilder.Entity<Property>(entity =>
{
    entity.HasIndex(e => e.City);
    entity.HasIndex(e => e.Price);
    entity.HasIndex(e => e.PropertyType);
    entity.HasIndex(e => new { e.City, e.PropertyType });
});
```

---

## Security Hardening

### Rate Limiting

Install package:
```bash
dotnet add package AspNetCoreRateLimit
```

Configure in `appsettings.json`:
```json
{
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": true,
    "StackBlockedRequests": false,
    "RealIpHeader": "X-Real-IP",
    "HttpStatusCode": 429,
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 60
      }
    ]
  }
}
```

### CORS for Production

Update `ServiceExtensions.cs`:
```csharp
services.AddCors(options =>
{
    options.AddPolicy("Production", builder =>
    {
        builder.WithOrigins("https://yourdomain.com", "https://www.yourdomain.com")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});
```

---

## Backup Strategy

### Database Backups

**SQL Server:**
```sql
-- Full backup
BACKUP DATABASE RealEstateDB 
TO DISK = 'C:\Backups\RealEstateDB_Full.bak' 
WITH FORMAT;

-- Differential backup
BACKUP DATABASE RealEstateDB 
TO DISK = 'C:\Backups\RealEstateDB_Diff.bak' 
WITH DIFFERENTIAL;
```

**Automated Azure SQL Backups:**
- Azure SQL Database provides automatic backups
- Retention: 7-35 days
- Point-in-time restore available

---

## Troubleshooting

### Common Issues

**Issue: Database connection timeout**
```
Solution: Increase connection timeout in connection string
"Connection Timeout=30"
```

**Issue: JWT token validation fails**
```
Solution: Ensure clock skew is configured
ClockSkew = TimeSpan.Zero
```

**Issue: High memory usage**
```
Solution: Enable garbage collection settings in .csproj
<ServerGarbageCollection>true</ServerGarbageCollection>
<ConcurrentGarbageCollection>true</ConcurrentGarbageCollection>
```

---

## Maintenance

### Regular Tasks

- [ ] Monitor application logs weekly
- [ ] Review database performance monthly
- [ ] Update NuGet packages quarterly
- [ ] Security audit quarterly
- [ ] Load testing before major releases
- [ ] Database backup verification monthly

---

## Rollback Plan

1. **Keep previous deployment package**
2. **Database: Keep migration scripts**
3. **If deployment fails:**
   - Stop new deployment
   - Restore previous version
   - Rollback database if needed
   - Investigate logs
   - Fix issues
   - Redeploy

---

## Support & Monitoring URLs

After deployment, monitor these endpoints:

```
https://yourdomain.com/health          - Health check
https://yourdomain.com/swagger         - API documentation (disable in production)
```

