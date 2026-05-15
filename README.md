# 🏠 Real Estate API - ASP.NET Core with Onion Architecture

A production-ready, scalable Real Estate Backend API built with ASP.NET Core 8.0 following Onion Architecture principles and SOLID design patterns.

## 🎯 Features

### Core Functionality
- ✅ **User Authentication & Authorization** - JWT-based authentication with role support (Admin, User)
- ✅ **Property Management** - Full CRUD operations for properties
- ✅ **Advanced Search & Filtering** - Filter by price, location, type, amenities
- ✅ **Pagination & Sorting** - Efficient data retrieval
- ✅ **Favorites System** - Users can save favorite properties
- ✅ **Reviews & Ratings** - Property review system with automatic rating calculation
- ✅ **Inquiry/Booking System** - Property inquiry management

### Technical Features
- ✅ **Onion Architecture** - Clean separation of concerns
- ✅ **SOLID Principles** - Maintainable and extensible code
- ✅ **Repository Pattern** - Data access abstraction
- ✅ **Service Pattern** - Business logic encapsulation
- ✅ **AutoMapper** - Object-to-object mapping
- ✅ **FluentValidation** - Input validation
- ✅ **Global Exception Handling** - Centralized error handling
- ✅ **Serilog Logging** - Structured logging
- ✅ **Swagger/OpenAPI** - Interactive API documentation
- ✅ **Entity Framework Core** - Code-First approach
- ✅ **Soft Delete** - Data preservation
- ✅ **Standardized API Responses** - Consistent response format

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                        API Layer                            │
│  (Controllers, Middleware, Configuration)                   │
└───────────────────────┬─────────────────────────────────────┘
                        │
┌───────────────────────▼─────────────────────────────────────┐
│                   Application Layer                         │
│  (Services, DTOs, Validators, Mappings, Interfaces)         │
└───────────────────────┬─────────────────────────────────────┘
                        │
┌───────────────────────▼─────────────────────────────────────┐
│                 Infrastructure Layer                        │
│  (DbContext, Repositories, Identity, External Services)     │
└───────────────────────┬─────────────────────────────────────┘
                        │
┌───────────────────────▼─────────────────────────────────────┐
│                    Domain Layer                             │
│  (Entities, Enums, Domain Interfaces)                       │
└─────────────────────────────────────────────────────────────┘
```

### Layer Dependencies
- **Domain**: No dependencies (pure business logic)
- **Application**: Depends on Domain
- **Infrastructure**: Depends on Domain and Application
- **API**: Depends on Application (references Infrastructure for DI only)

## 📁 Project Structure

```
RealEstateAPI/
├── Domain/                          # Core business logic
│   ├── Entities/
│   ├── Enums/
│   └── Interfaces/
├── Application/                     # Business logic implementation
│   ├── DTOs/
│   ├── Services/
│   ├── Interfaces/
│   ├── Validators/
│   └── Mappings/
├── Infrastructure/                  # Data access & external services
│   ├── Data/
│   ├── Repositories/
│   └── Identity/
└── API/                            # Web API
    ├── Controllers/
    ├── Middleware/
    ├── Extensions/
    └── Configuration files
```

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- SQL Server (LocalDB, Express, or Full)
- Visual Studio 2022 / VS Code / JetBrains Rider

### Installation Steps

1. **Clone the repository**
```bash
git clone <repository-url>
cd RealEstateAPI
```

2. **Update Database Connection String**

Edit `API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=RealEstateDB;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

3. **Update JWT Configuration**

Edit `API/appsettings.json`:
```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "RealEstateAPI",
    "Audience": "RealEstateAPIUsers"
  }
}
```

4. **Restore NuGet Packages**
```bash
dotnet restore
```

5. **Create Database Migration**
```bash
cd API
dotnet ef migrations add InitialCreate
```

6. **Update Database**
```bash
dotnet ef database update
```

7. **Run the Application**
```bash
dotnet run
```

8. **Access Swagger UI**
```
https://localhost:5001
or
http://localhost:5000
```

## 📚 API Endpoints

### Authentication
```
POST   /api/auth/register          - Register new user
POST   /api/auth/login             - Login user
```

### Properties
```
GET    /api/properties             - Get all properties (with filtering)
GET    /api/properties/{id}        - Get property by ID
POST   /api/properties             - Create property (Authenticated)
PUT    /api/properties/{id}        - Update property (Owner only)
DELETE /api/properties/{id}        - Delete property (Owner only)
```

### Reviews
```
GET    /api/reviews/{id}                    - Get review by ID
GET    /api/reviews/property/{propertyId}   - Get property reviews
POST   /api/reviews                         - Create review (Authenticated)
PUT    /api/reviews/{id}                    - Update review (Owner only)
DELETE /api/reviews/{id}                    - Delete review (Owner only)
```

### Favorites
```
GET    /api/favorites                       - Get user favorites
POST   /api/favorites/{propertyId}          - Add to favorites
DELETE /api/favorites/{propertyId}          - Remove from favorites
GET    /api/favorites/check/{propertyId}    - Check if favorite
```

### Inquiries
```
GET    /api/inquiries/{id}                  - Get inquiry by ID
GET    /api/inquiries/my-inquiries          - Get user inquiries
GET    /api/inquiries/all                   - Get all inquiries (Admin)
POST   /api/inquiries                       - Create inquiry
PATCH  /api/inquiries/{id}/status           - Update status (Admin)
DELETE /api/inquiries/{id}                  - Delete inquiry
```

## 🔍 Query Parameters for Filtering

### Property Filtering
```
GET /api/properties?minPrice=100000&maxPrice=500000&city=Cairo&propertyType=Villa&hasParking=true&sortBy=price&sortDescending=false&pageNumber=1&pageSize=10
```

**Available Filters:**
- `minPrice`, `maxPrice` - Price range
- `location`, `city` - Location filtering
- `propertyType` - Apartment, Villa, House, etc.
- `status` - Available, Sold, Rented, etc.
- `minBedrooms`, `minBathrooms` - Minimum requirements
- `minSquareFeet` - Minimum size
- `hasParking`, `hasGarden`, `hasPool` - Amenities
- `sortBy` - price, created, rating
- `sortDescending` - true/false
- `pageNumber`, `pageSize` - Pagination

## 🔐 Authentication Flow

1. **Register a new user**
```json
POST /api/auth/register
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "password": "Pass@123",
  "confirmPassword": "Pass@123",
  "phoneNumber": "+1234567890"
}
```

2. **Login**
```json
POST /api/auth/login
{
  "email": "john@example.com",
  "password": "Pass@123"
}
```

3. **Use the returned JWT token**
```
Authorization: Bearer <your-jwt-token>
```

## 🧪 Sample Requests

### Create Property
```json
POST /api/properties
Authorization: Bearer <token>

{
  "title": "Luxury Villa in Cairo",
  "description": "Beautiful 5-bedroom villa with pool and garden",
  "price": 5000000,
  "location": "New Cairo",
  "city": "Cairo",
  "state": "Cairo Governorate",
  "zipCode": "11835",
  "address": "123 Fifth Settlement Street",
  "propertyType": 2,
  "bedrooms": 5,
  "bathrooms": 4,
  "squareFeet": 4500,
  "yearBuilt": 2020,
  "hasParking": true,
  "hasGarden": true,
  "hasPool": true,
  "imageUrls": [
    "https://example.com/image1.jpg",
    "https://example.com/image2.jpg"
  ]
}
```

### Create Review
```json
POST /api/reviews
Authorization: Bearer <token>

{
  "propertyId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "rating": 5,
  "comment": "Excellent property! Highly recommended."
}
```

## 🛠️ Database Schema

### Main Tables
- **Users** - User accounts and authentication
- **Properties** - Property listings
- **PropertyImages** - Property photos
- **Reviews** - Property reviews and ratings
- **Favorites** - User favorite properties (Many-to-Many)
- **Inquiries** - Property booking/inquiry requests

### Relationships
- User → Properties (One-to-Many)
- User → Reviews (One-to-Many)
- User → Favorites (One-to-Many)
- User → Inquiries (One-to-Many)
- Property → PropertyImages (One-to-Many)
- Property → Reviews (One-to-Many)
- Property → Favorites (One-to-Many)
- Property → Inquiries (One-to-Many)

## 🔧 Configuration

### Environment-Specific Settings

**Development** (`appsettings.Development.json`):
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

**Production**: Update connection strings, JWT secrets, and logging levels appropriately.

## 📊 Response Format

All API responses follow a standardized format:

**Success Response:**
```json
{
  "success": true,
  "message": "Operation successful",
  "data": { },
  "errors": null,
  "timestamp": "2024-01-15T10:30:00Z"
}
```

**Error Response:**
```json
{
  "success": false,
  "message": "Operation failed",
  "data": null,
  "errors": ["Error detail 1", "Error detail 2"],
  "timestamp": "2024-01-15T10:30:00Z"
}
```

## 🎨 Design Patterns Used

1. **Repository Pattern** - Data access abstraction
2. **Service Pattern** - Business logic encapsulation
3. **Dependency Injection** - Loose coupling
4. **DTO Pattern** - Data transfer optimization
5. **Middleware Pattern** - Cross-cutting concerns
6. **Strategy Pattern** - Flexible algorithms (sorting, filtering)

## 🔒 Security Features

- ✅ JWT Authentication
- ✅ Role-based Authorization
- ✅ Password hashing (BCrypt)
- ✅ Input validation
- ✅ SQL injection prevention (EF Core parameterization)
- ✅ Global exception handling
- ✅ CORS configuration

## 📝 Logging

Logs are written to:
- Console (for development)
- File: `logs/realestate-api-YYYYMMDD.txt` (rolling daily)

## 🚀 Deployment

### Docker (Optional)
```dockerfile
# Create Dockerfile in root directory
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RealEstateAPI.API.dll"]
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## 📄 License

This project is licensed under the MIT License.

## 👨‍💻 Author

Built with ❤️ using ASP.NET Core and Onion Architecture

## 📞 Support

For issues and questions, please create an issue in the repository.
