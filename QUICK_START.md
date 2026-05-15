# ⚡ Quick Start Guide - Real Estate API

## Get Running in 5 Minutes

### Step 1: Prerequisites Check
```bash
# Verify .NET 8.0 is installed
dotnet --version
# Should show 8.0.x or higher
```

### Step 2: Navigate to Project
```bash
cd RealEstateAPI
```

### Step 3: Restore Packages
```bash
dotnet restore
```

### Step 4: Update Database Connection (if needed)
Edit `API/appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=RealEstateDB;Trusted_Connection=True;TrustServerCertificate=True"
}
```

### Step 5: Create Database
```bash
cd API
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Step 6: Run the API
```bash
dotnet run
```

### Step 7: Open Swagger
Navigate to: `https://localhost:5001` or `http://localhost:5000`

---

## Test the API (First 3 Requests)

### 1. Register a User
```http
POST https://localhost:5001/api/auth/register
Content-Type: application/json

{
  "firstName": "Test",
  "lastName": "User",
  "email": "test@example.com",
  "password": "Test@1234",
  "confirmPassword": "Test@1234",
  "phoneNumber": "+1234567890"
}
```

**Copy the `token` from the response!**

### 2. Create a Property
```http
POST https://localhost:5001/api/properties
Authorization: Bearer YOUR_TOKEN_HERE
Content-Type: application/json

{
  "title": "Beautiful Villa",
  "description": "A stunning 4-bedroom villa with a private pool and garden, located in a prime neighborhood.",
  "price": 5000000,
  "location": "Downtown",
  "city": "Cairo",
  "state": "Cairo",
  "zipCode": "12345",
  "address": "123 Main Street",
  "propertyType": 2,
  "bedrooms": 4,
  "bathrooms": 3,
  "squareFeet": 3500,
  "yearBuilt": 2020,
  "hasParking": true,
  "hasGarden": true,
  "hasPool": true,
  "imageUrls": ["https://example.com/image1.jpg"]
}
```

### 3. Get All Properties
```http
GET https://localhost:5001/api/properties
```

---

## What's Included?

✅ **Complete Onion Architecture**
- Domain Layer (Entities, Enums, Interfaces)
- Application Layer (Services, DTOs, Validators, Mappings)
- Infrastructure Layer (DbContext, Repositories, JWT)
- API Layer (Controllers, Middleware, Configuration)

✅ **Features**
- JWT Authentication & Authorization
- Property CRUD with filtering & pagination
- Reviews & Ratings
- Favorites System
- Inquiry/Booking System
- Global Exception Handling
- Serilog Logging
- FluentValidation
- AutoMapper
- Swagger/OpenAPI

✅ **Documentation**
- README.md - Complete overview
- API_TESTING_GUIDE.md - API testing examples
- DEPLOYMENT_GUIDE.md - Production deployment
- ARCHITECTURE.md - Design decisions
- PROJECT_STRUCTURE.md - Folder organization

---

## Next Steps

1. **Explore the API** - Use Swagger UI to test endpoints
2. **Review the Code** - Check out the clean architecture
3. **Customize** - Modify entities, add features
4. **Deploy** - Follow DEPLOYMENT_GUIDE.md

---

## Project Structure

```
RealEstateAPI/
├── Domain/                 # Core entities and business logic
├── Application/            # Services, DTOs, Validators
├── Infrastructure/         # Database, Repositories
├── API/                   # Controllers, Middleware
├── README.md              # Main documentation
├── API_TESTING_GUIDE.md   # Testing guide
├── DEPLOYMENT_GUIDE.md    # Deployment instructions
└── ARCHITECTURE.md        # Architecture decisions
```

---

## Default Test Credentials (After Seeding)

**Admin User:**
- Email: admin@realestate.com
- Password: Admin@123

**Regular User:**
- Email: john@example.com
- Password: User@123

---

## Common Commands

```bash
# Restore packages
dotnet restore

# Build solution
dotnet build

# Run API
cd API && dotnet run

# Create migration
cd API && dotnet ef migrations add MigrationName

# Update database
cd API && dotnet ef database update

# Run tests (if you add them)
dotnet test
```

---

## Need Help?

1. Check README.md for complete documentation
2. Review API_TESTING_GUIDE.md for API examples
3. See ARCHITECTURE.md for design decisions
4. Follow DEPLOYMENT_GUIDE.md for production setup

---

## Troubleshooting

**Database connection fails:**
- Ensure SQL Server is running
- Check connection string in appsettings.json

**Migration fails:**
- Delete Migrations folder
- Run `dotnet ef migrations add InitialCreate` again

**Port already in use:**
- Change ports in launchSettings.json
- Or stop other applications using ports 5000/5001

---

## 🎉 You're Ready!

Open Swagger and start exploring the API:
- Register users
- Create properties
- Add reviews
- Manage favorites
- Send inquiries

Happy coding! 🚀
