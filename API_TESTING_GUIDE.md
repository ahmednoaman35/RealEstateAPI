# 🧪 API Testing Guide

## Quick Start Testing

### 1. Register a New User

**Endpoint:** `POST /api/auth/register`

**Request Body:**
```json
{
  "firstName": "Ahmed",
  "lastName": "Ali",
  "email": "ahmed@example.com",
  "password": "Test@1234",
  "confirmPassword": "Test@1234",
  "phoneNumber": "+201234567890"
}
```

**Expected Response (200 OK):**
```json
{
  "success": true,
  "message": "User registered successfully",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "email": "ahmed@example.com",
    "firstName": "Ahmed",
    "lastName": "Ali",
    "role": "User",
    "expiresAt": "2024-01-16T10:30:00Z"
  },
  "errors": null,
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### 2. Login

**Endpoint:** `POST /api/auth/login`

**Request Body:**
```json
{
  "email": "ahmed@example.com",
  "password": "Test@1234"
}
```

**Expected Response (200 OK):**
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "email": "ahmed@example.com",
    "firstName": "Ahmed",
    "lastName": "Ali",
    "role": "User",
    "expiresAt": "2024-01-16T10:30:00Z"
  }
}
```

**Copy the token** - You'll need it for authenticated requests!

---

## Property Endpoints

### 3. Get All Properties (Public - No Auth Required)

**Endpoint:** `GET /api/properties`

**Query Parameters:**
```
?minPrice=100000
&maxPrice=10000000
&city=Cairo
&propertyType=2
&hasPool=true
&sortBy=price
&sortDescending=false
&pageNumber=1
&pageSize=10
```

### 4. Get Property by ID (Public)

**Endpoint:** `GET /api/properties/{propertyId}`

**Example:** `GET /api/properties/3fa85f64-5717-4562-b3fc-2c963f66afa6`

### 5. Create Property (Authenticated)

**Endpoint:** `POST /api/properties`

**Headers:**
```
Authorization: Bearer {your-jwt-token}
Content-Type: application/json
```

**Request Body:**
```json
{
  "title": "Seaside Apartment in Alexandria",
  "description": "Beautiful beachfront apartment with stunning Mediterranean views. Recently renovated with modern appliances and contemporary design. Perfect for those seeking a coastal lifestyle.",
  "price": 3500000,
  "location": "Miami",
  "city": "Alexandria",
  "state": "Alexandria Governorate",
  "zipCode": "21500",
  "address": "Building 10, Corniche Street",
  "propertyType": 1,
  "bedrooms": 2,
  "bathrooms": 2,
  "squareFeet": 1800,
  "yearBuilt": 2022,
  "hasParking": true,
  "hasGarden": false,
  "hasPool": false,
  "imageUrls": [
    "https://images.unsplash.com/photo-1512917774080-9991f1c4c750",
    "https://images.unsplash.com/photo-1600596542815-ffad4c1539a9"
  ]
}
```

### 6. Update Property (Owner Only)

**Endpoint:** `PUT /api/properties/{propertyId}`

**Headers:**
```
Authorization: Bearer {your-jwt-token}
```

**Request Body:**
```json
{
  "title": "Updated Title",
  "price": 3800000,
  "status": 1
}
```

### 7. Delete Property (Owner Only)

**Endpoint:** `DELETE /api/properties/{propertyId}`

**Headers:**
```
Authorization: Bearer {your-jwt-token}
```

---

## Review Endpoints

### 8. Get Property Reviews (Public)

**Endpoint:** `GET /api/reviews/property/{propertyId}?pageNumber=1&pageSize=10`

### 9. Create Review (Authenticated)

**Endpoint:** `POST /api/reviews`

**Headers:**
```
Authorization: Bearer {your-jwt-token}
```

**Request Body:**
```json
{
  "propertyId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "rating": 5,
  "comment": "Amazing property! The location is perfect and the amenities are top-notch. Highly recommended for families looking for a spacious home."
}
```

**Validation Rules:**
- Rating must be between 1-5
- Comment must be at least 10 characters
- User can only review a property once

### 10. Update Review (Review Owner Only)

**Endpoint:** `PUT /api/reviews/{reviewId}`

**Headers:**
```
Authorization: Bearer {your-jwt-token}
```

**Request Body:**
```json
{
  "rating": 4,
  "comment": "Updated review comment"
}
```

### 11. Delete Review (Review Owner Only)

**Endpoint:** `DELETE /api/reviews/{reviewId}`

---

## Favorites Endpoints

### 12. Get My Favorites (Authenticated)

**Endpoint:** `GET /api/favorites?pageNumber=1&pageSize=10`

**Headers:**
```
Authorization: Bearer {your-jwt-token}
```

### 13. Add to Favorites (Authenticated)

**Endpoint:** `POST /api/favorites/{propertyId}`

**Headers:**
```
Authorization: Bearer {your-jwt-token}
```

### 14. Remove from Favorites (Authenticated)

**Endpoint:** `DELETE /api/favorites/{propertyId}`

### 15. Check if Property is Favorite (Authenticated)

**Endpoint:** `GET /api/favorites/check/{propertyId}`

---

## Inquiry Endpoints

### 16. Create Inquiry (Authenticated)

**Endpoint:** `POST /api/inquiries`

**Headers:**
```
Authorization: Bearer {your-jwt-token}
```

**Request Body:**
```json
{
  "propertyId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "message": "I am interested in viewing this property. Could we schedule a visit this weekend? I would also like to know more about the payment options available.",
  "contactEmail": "ahmed@example.com",
  "contactPhone": "+201234567890",
  "preferredContactDate": "2024-01-20T10:00:00Z"
}
```

### 17. Get My Inquiries (Authenticated)

**Endpoint:** `GET /api/inquiries/my-inquiries?pageNumber=1&pageSize=10`

### 18. Get All Inquiries (Admin Only)

**Endpoint:** `GET /api/inquiries/all?pageNumber=1&pageSize=10`

**Headers:**
```
Authorization: Bearer {admin-jwt-token}
```

### 19. Update Inquiry Status (Admin Only)

**Endpoint:** `PATCH /api/inquiries/{inquiryId}/status`

**Headers:**
```
Authorization: Bearer {admin-jwt-token}
```

**Request Body:**
```json
{
  "status": 3,
  "adminResponse": "Thank you for your inquiry. We have scheduled a viewing for Saturday at 2 PM. Our agent will contact you shortly."
}
```

**Status Enum Values:**
- 1 = Pending
- 2 = InProgress
- 3 = Responded
- 4 = Closed
- 5 = Cancelled

---

## Property Type Enum Values

Use these values in the `propertyType` field:

```
1 = Apartment
2 = Villa
3 = House
4 = Condo
5 = Townhouse
6 = Land
7 = Commercial
8 = Studio
9 = Penthouse
```

## Property Status Enum Values

Use these values in the `status` field:

```
1 = Available
2 = Sold
3 = Rented
4 = UnderContract
5 = Pending
```

---

## Error Response Examples

### 400 Bad Request (Validation Error)
```json
{
  "success": false,
  "message": "Validation failed",
  "data": null,
  "errors": [
    "Email is required",
    "Password must be at least 8 characters"
  ],
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### 401 Unauthorized
```json
{
  "success": false,
  "message": "Unauthorized access",
  "data": null,
  "errors": ["Invalid or expired token"],
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### 404 Not Found
```json
{
  "success": false,
  "message": "Property not found",
  "data": null,
  "errors": null,
  "timestamp": "2024-01-15T10:30:00Z"
}
```

---

## Postman Collection Setup

### Environment Variables

Create these variables in Postman:

```
base_url: https://localhost:5001
user_token: (leave empty - will be set automatically)
admin_token: (leave empty - will be set automatically)
```

### Auto-Set Token Script

Add this to the "Tests" tab of your login request:

```javascript
var response = pm.response.json();
if (response.success && response.data.token) {
    pm.environment.set("user_token", response.data.token);
}
```

### Use Token in Requests

In the Authorization tab:
- Type: Bearer Token
- Token: `{{user_token}}`

---

## Testing Checklist

✅ **Authentication**
- [ ] Register new user
- [ ] Login with credentials
- [ ] Test invalid credentials

✅ **Properties**
- [ ] Get all properties
- [ ] Filter properties by price
- [ ] Filter properties by location
- [ ] Create property (authenticated)
- [ ] Update own property
- [ ] Attempt to update another user's property (should fail)
- [ ] Delete own property

✅ **Reviews**
- [ ] Create review for property
- [ ] Attempt to create duplicate review (should fail)
- [ ] Update own review
- [ ] Delete own review
- [ ] Verify property rating updates

✅ **Favorites**
- [ ] Add property to favorites
- [ ] View favorites list
- [ ] Remove from favorites
- [ ] Check favorite status

✅ **Inquiries**
- [ ] Create inquiry
- [ ] View own inquiries
- [ ] Update inquiry status (admin)
- [ ] Delete inquiry

---

## Common Issues & Solutions

### Issue: 401 Unauthorized
**Solution:** Ensure JWT token is included in Authorization header

### Issue: 404 Not Found
**Solution:** Check if the GUID format is correct and resource exists

### Issue: 400 Bad Request
**Solution:** Review validation errors in response and fix request body

### Issue: Connection Error
**Solution:** Ensure the API is running and SSL certificate is trusted
