# 🏛️ Architecture & Design Decisions

## Overview

This document explains the architectural decisions and design patterns used in the Real Estate API.

---

## Architecture: Onion Architecture

### Why Onion Architecture?

**Benefits:**
1. **Independence of frameworks** - The domain layer doesn't depend on EF Core or ASP.NET Core
2. **Testability** - Each layer can be tested independently
3. **Independence of UI** - We can change from REST API to GraphQL without changing business logic
4. **Independence of Database** - We can switch from SQL Server to PostgreSQL with minimal changes
5. **Business logic first** - The core domain is at the center

### Layer Responsibilities

```
┌─────────────────────────────────────────────────────┐
│                    API Layer                        │
│  - HTTP endpoints                                   │
│  - Request/Response handling                        │
│  - Authentication/Authorization                     │
│  - Middleware                                       │
└───────────────────┬─────────────────────────────────┘
                    │
┌───────────────────▼─────────────────────────────────┐
│               Application Layer                     │
│  - Business logic orchestration                     │
│  - DTOs                                             │
│  - Validation                                       │
│  - Mapping                                          │
│  - Service interfaces                               │
└───────────────────┬─────────────────────────────────┘
                    │
┌───────────────────▼─────────────────────────────────┐
│             Infrastructure Layer                    │
│  - Database access (EF Core)                        │
│  - Repository implementations                       │
│  - External service integrations                    │
│  - Identity (JWT)                                   │
└───────────────────┬─────────────────────────────────┘
                    │
┌───────────────────▼─────────────────────────────────┐
│                 Domain Layer                        │
│  - Entities                                         │
│  - Enums                                            │
│  - Domain interfaces                                │
│  - Business rules                                   │
└─────────────────────────────────────────────────────┘
```

---

## Design Patterns

### 1. Repository Pattern

**Purpose:** Abstract data access logic

**Implementation:**
- `IBaseRepository<T>` - Generic repository interface
- `BaseRepository<T>` - Generic implementation
- Specialized repositories (UserRepository, PropertyRepository) for complex queries

**Benefits:**
- Centralized data access logic
- Easy to mock for testing
- Consistent data access patterns

### 2. Service Pattern

**Purpose:** Encapsulate business logic

**Implementation:**
- Service interfaces in Application layer
- Service implementations with business logic
- Services use repositories for data access

**Example:**
```csharp
public class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _repository;
    
    public async Task<PropertyDto> CreateAsync(CreatePropertyDto dto)
    {
        // Business logic here
    }
}
```

### 3. Unit of Work Pattern

**Implementation:** Entity Framework DbContext acts as Unit of Work

**Benefits:**
- Atomic transactions
- Change tracking
- Single SaveChanges() call

### 4. Dependency Injection

**Purpose:** Achieve loose coupling

**Implementation:**
- Constructor injection throughout
- Services registered in `ServiceExtensions`
- Scoped lifetime for services with DbContext

### 5. DTO Pattern

**Purpose:** Data transfer without exposing entities

**Benefits:**
- API versioning flexibility
- Security (hide sensitive fields)
- Optimize data transfer
- Separate API contract from domain

### 6. Strategy Pattern

**Implementation:** Sorting and filtering strategies in PropertyRepository

```csharp
private IQueryable<Property> ApplySorting(IQueryable<Property> query, string? sortBy)
{
    return sortBy?.ToLower() switch
    {
        "price" => query.OrderBy(p => p.Price),
        "rating" => query.OrderByDescending(p => p.AverageRating),
        _ => query.OrderByDescending(p => p.CreatedAt)
    };
}
```

### 7. Middleware Pattern

**Purpose:** Cross-cutting concerns

**Implementation:**
- Global exception handler
- Authentication/Authorization
- Logging
- Response compression

---

## SOLID Principles Application

### Single Responsibility Principle (SRP)
- Each class has one reason to change
- Controllers only handle HTTP concerns
- Services only contain business logic
- Repositories only handle data access

**Example:**
```csharp
// Controller - HTTP concerns only
public class PropertiesController : ControllerBase
{
    public async Task<IActionResult> GetAll([FromQuery] PropertyFilterDto filter)
    {
        var result = await _propertyService.GetAllAsync(filter);
        return Ok(ApiResponse.SuccessResponse(result));
    }
}

// Service - Business logic only
public class PropertyService : IPropertyService
{
    public async Task<PagedResponse<PropertyDto>> GetAllAsync(PropertyFilterDto filter)
    {
        var (properties, totalCount) = await _repository.GetFilteredPropertiesAsync(filter);
        return new PagedResponse<PropertyDto>(properties, filter.PageNumber, filter.PageSize, totalCount);
    }
}

// Repository - Data access only
public class PropertyRepository : IPropertyRepository
{
    public async Task<(List<Property>, int)> GetFilteredPropertiesAsync(PropertyFilterDto filter)
    {
        // Query building and execution
    }
}
```

### Open/Closed Principle (OCP)
- Open for extension, closed for modification
- New property types can be added via enum without changing code
- New sorting strategies can be added without modifying existing code

### Liskov Substitution Principle (LSP)
- All repositories implement IBaseRepository<T>
- Any IPropertyRepository can be substituted with PropertyRepository
- Service interfaces allow for easy substitution

### Interface Segregation Principle (ISP)
- Specific interfaces (IPropertyService, IReviewService)
- Clients don't depend on methods they don't use
- IBaseRepository has only essential CRUD operations

### Dependency Inversion Principle (DIP)
- High-level modules depend on abstractions
- Controllers depend on IPropertyService, not PropertyService
- Services depend on IPropertyRepository, not PropertyRepository

---

## Data Access Strategy

### Entity Framework Core (Code-First)

**Why Code-First?**
1. Version control for database schema
2. Easier collaboration
3. Automated migrations
4. Type-safe queries

**Configuration:**
- Fluent API in `OnModelCreating`
- Separate configuration for each entity
- Global query filters for soft deletes

### Soft Delete Implementation

```csharp
public abstract class BaseEntity
{
    public bool IsDeleted { get; set; } = false;
}

// Global filter
modelBuilder.Entity<Property>()
    .HasQueryFilter(e => !e.IsDeleted);
```

**Benefits:**
- Data preservation for auditing
- Easier to recover deleted items
- Maintains referential integrity

### Relationships

```
User 1──────────* Property
Property 1──────* PropertyImage
Property 1──────* Review
User 1──────────* Review
User *──────────* Property (Favorites - Many-to-Many)
Property 1──────* Inquiry
User 1──────────* Inquiry
```

---

## Security Design

### Authentication Flow

```
1. User registers/logs in
   ↓
2. Server validates credentials
   ↓
3. Server generates JWT token
   ↓
4. Client stores token
   ↓
5. Client sends token in Authorization header
   ↓
6. Server validates token and extracts claims
   ↓
7. Server authorizes based on role
```

### Password Security

- BCrypt hashing (work factor: 10)
- Passwords never stored in plain text
- Password complexity requirements via FluentValidation

### Authorization Levels

1. **Public** - Anyone can access (GET properties, reviews)
2. **Authenticated** - Logged-in users (Create property, add favorites)
3. **Owner** - Resource owner (Update/Delete own property)
4. **Admin** - Administrative users (Manage inquiries, all CRUD operations)

---

## API Design Principles

### RESTful Design

- Resources as nouns (properties, reviews, favorites)
- HTTP verbs for actions (GET, POST, PUT, DELETE, PATCH)
- Proper status codes (200, 201, 400, 401, 404, 500)
- Hierarchical URLs (/api/properties/{id}/reviews)

### Standardized Responses

All responses follow consistent format:
```csharp
{
    "success": bool,
    "message": string,
    "data": T,
    "errors": string[],
    "timestamp": DateTime
}
```

### Pagination

- Cursor-based pagination support
- Page size limits (max 100)
- Total count included
- Navigation links (hasNext, hasPrevious)

### Filtering & Sorting

- Query parameters for filtering
- Multiple sort options
- Combining filters with AND logic

---

## Error Handling Strategy

### Centralized Exception Handling

```csharp
GlobalExceptionHandlerMiddleware
    ↓
Map exceptions to HTTP status codes
    ↓
Return standardized error response
    ↓
Log error details
```

### Exception Types

- `UnauthorizedAccessException` → 401
- `KeyNotFoundException` → 404
- `InvalidOperationException` → 400
- `ArgumentNullException` → 400
- Generic exceptions → 500

---

## Validation Strategy

### Multi-Layer Validation

1. **DTO Validation** - FluentValidation at API layer
2. **Business Rule Validation** - Service layer
3. **Database Constraints** - Entity Framework configurations

### Example Validation Flow

```
Request
    ↓
FluentValidation (DTO)
    ↓
Service validation (business rules)
    ↓
Database constraints
    ↓
Response
```

---

## Performance Considerations

### Database Optimization

1. **Eager Loading** - Include related entities when needed
   ```csharp
   .Include(p => p.Owner)
   .Include(p => p.Images)
   ```

2. **Pagination** - Limit result sets
3. **Indexes** - On frequently queried columns
4. **Projection** - Select only needed columns

### Caching Strategy (Future Enhancement)

- Response caching for GET endpoints
- Distributed caching (Redis) for high-traffic scenarios
- Cache invalidation on updates

### Asynchronous Operations

- All I/O operations are async
- Non-blocking database calls
- Better scalability under load

---

## Logging Strategy

### Structured Logging with Serilog

**What we log:**
- Information: Successful operations
- Warning: Validation failures, unauthorized access
- Error: Exceptions, failed operations
- Debug: Detailed execution flow (development only)

**Log Enrichment:**
- Request context
- User information
- Timestamps
- Correlation IDs (future enhancement)

---

## Testing Strategy (Recommended)

### Unit Tests
- Service layer business logic
- Validation rules
- Mapping configurations

### Integration Tests
- API endpoints
- Database operations
- Authentication/Authorization

### Test Structure
```
Tests/
├── UnitTests/
│   ├── Services/
│   ├── Validators/
│   └── Mappers/
└── IntegrationTests/
    ├── Controllers/
    └── Repositories/
```

---

## Scalability Considerations

### Horizontal Scaling

- Stateless API design
- JWT tokens (no server-side sessions)
- Database connection pooling

### Vertical Scaling

- Async operations
- Efficient queries
- Response compression

### Future Enhancements

1. **Caching layer** (Redis)
2. **Message queue** (RabbitMQ) for async operations
3. **CDN** for static files (property images)
4. **Read replicas** for database
5. **API Gateway** for microservices transition

---

## Maintainability

### Code Organization

- Clear folder structure
- Separation of concerns
- Consistent naming conventions
- XML documentation comments

### Extensibility

- New features can be added without modifying existing code
- Plugin architecture potential
- Easy to add new endpoints
- Simple to extend entities

---

## Trade-offs & Limitations

### Current Implementation

**Pros:**
- Clean architecture
- Easy to test
- Maintainable
- Scalable foundation

**Cons:**
- More initial complexity
- More files/projects
- Learning curve for new developers
- Slight performance overhead (acceptable for most scenarios)

### Known Limitations

1. **Image Storage** - Currently stores URLs only (implement blob storage for production)
2. **Real-time Updates** - No SignalR/WebSockets (add for real-time features)
3. **Advanced Search** - No full-text search (consider Elasticsearch for large datasets)
4. **File Uploads** - No direct file upload (implement with Azure Blob/AWS S3)

---

## Future Roadmap

### Phase 1 (Current)
✅ Core CRUD operations
✅ Authentication & Authorization
✅ Search & Filtering
✅ Reviews & Favorites

### Phase 2 (Recommended)
- Image upload functionality
- Email notifications
- Advanced search (Elasticsearch)
- Payment integration

### Phase 3 (Advanced)
- Real-time notifications (SignalR)
- Map integration
- Analytics dashboard
- Mobile app support

---

## Conclusion

This architecture provides a solid foundation for a production-ready real estate API. The use of Onion Architecture, SOLID principles, and established design patterns ensures the codebase is:

- **Testable** - Each layer can be tested independently
- **Maintainable** - Clear separation of concerns
- **Scalable** - Designed for growth
- **Extensible** - Easy to add new features
- **Secure** - Multiple security layers

The architecture balances complexity with practical needs, providing enterprise-grade structure while remaining understandable and maintainable.
