# Admin-Only Endpoints Security Documentation

## Overview
This document outlines the endpoints that are restricted to admin users only and the security measures in place.

## Protected Endpoints

### 🔒 Admin-Only Endpoints

#### GET /api/users
- **Access**: Admin users only
- **Authorization**: `[Authorize(Roles = "admin")]`
- **Description**: Retrieves all users with pagination (includes all user types: users, admins, and external systems)
- **Usage**: Admin panels, user management systems, system monitoring

#### GET /api/order
- **Access**: Admin users only
- **Authorization**: `[Authorize(Roles = "admin")]`
- **Description**: Retrieves all orders with pagination across the entire system
- **Usage**: Admin panels, order management, system monitoring

#### GET /api/order/by-user/{userId}
- **Access**: Admin users only
- **Authorization**: `[Authorize(Roles = "admin")]`
- **Description**: Retrieves all orders for a specific user by their ID
- **Usage**: Admin support, user account investigation

#### POST /api/category
- **Access**: Admin users only
- **Authorization**: `[Authorize(Roles = "admin")]`
- **Description**: Create new product categories
- **Usage**: Catalog management, product organization

#### PUT /api/category/{id}
- **Access**: Admin users only
- **Authorization**: `[Authorize(Roles = "admin")]`
- **Description**: Update existing categories
- **Usage**: Catalog maintenance, category modifications

#### DELETE /api/category/{id}
- **Access**: Admin users only
- **Authorization**: `[Authorize(Roles = "admin")]`
- **Description**: Delete categories from the system
- **Usage**: Catalog cleanup, category removal

#### POST /api/paymentmethod
- **Access**: Admin users only
- **Authorization**: `[Authorize(Roles = "admin")]`
- **Description**: Create new payment methods
- **Usage**: Payment system configuration

#### PUT /api/paymentmethod/{id}
- **Access**: Admin users only
- **Authorization**: `[Authorize(Roles = "admin")]`
- **Description**: Update existing payment methods
- **Usage**: Payment method maintenance

#### DELETE /api/paymentmethod/{id}
- **Access**: Admin users only
- **Authorization**: `[Authorize(Roles = "admin")]`
- **Description**: Delete payment methods
- **Usage**: Payment system cleanup

## Authorization Implementation

### JWT Role Claims
The system uses JWT tokens with role-based claims:

```csharp
// JWT Token includes role claim
new Claim(ClaimTypes.Role, userType) // "user", "admin", or "rating_system"
```

### Controller Authorization
```csharp
[HttpGet]
[Authorize(Roles = "admin")]
public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
```

## Security Features

### ✅ What's Protected
1. **Get All Users** - Only admins can see user lists
2. **Role Verification** - JWT token must contain "admin" role
3. **Filtered Results** - Only returns regular users (excludes other admins/external systems)

### 🔓 What Remains Public
1. **User Registration** - `[AllowAnonymous]`
2. **User Search by Name** - `[AllowAnonymous]` (for discovery)
3. **Get User by ID** - `[AllowAnonymous]` (for public profiles)
4. **User Rating Updates** - `[AllowAnonymous]` (for reviews)

### 🔐 Authenticated User Actions
1. **Get Current User** - `[Authorize]` (any authenticated user)
2. **Update Current User** - `[Authorize]` (own profile only)
3. **Delete Current User** - `[Authorize]` (own profile only)

## Error Responses

### 401 Unauthorized
```json
{
  "message": "Unauthorized",
  "status": 401
}
```
**Cause**: No valid JWT token provided

### 403 Forbidden
```json
{
  "message": "Forbidden",
  "status": 403
}
```
**Cause**: Valid token but insufficient permissions (not admin role)

## Testing Admin Access

### Step 1: Register Admin User
```bash
POST /api/auth/register-admin
{
  "firstName": "أحمد",
  "lastName": "المدير",
  "phoneNumber": "0912345678",
  "email": "admin@ecommerce.com",
  "password": "AdminPassword123",
  "description": "مدير النظام"
}
```

### Step 2: Login as Admin
```bash
POST /api/auth/email-login
{
  "email": "admin@ecommerce.com",
  "password": "AdminPassword123"
}
```
**Response**: JWT token with `"role": "admin"` claim

### Step 3: Access Protected Endpoints
```bash
# Get all users (admin only)
GET /api/users?pageNumber=1&pageSize=10
Authorization: Bearer {admin_jwt_token}

# Get all orders (admin only)
GET /api/order?pageNumber=1&pageSize=10
Authorization: Bearer {admin_jwt_token}

# Get orders for specific user (admin only)
GET /api/order/by-user/123e4567-e89b-12d3-a456-426614174000?pageNumber=1&pageSize=10
Authorization: Bearer {admin_jwt_token}

# Create category (admin only)
POST /api/category
Authorization: Bearer {admin_jwt_token}
Content-Type: multipart/form-data

# Update category (admin only)
PUT /api/category/123e4567-e89b-12d3-a456-426614174000
Authorization: Bearer {admin_jwt_token}

# Delete category (admin only)
DELETE /api/category/123e4567-e89b-12d3-a456-426614174000
Authorization: Bearer {admin_jwt_token}

# Create payment method (admin only)
POST /api/paymentmethod
Authorization: Bearer {admin_jwt_token}
Content-Type: application/json

# Update payment method (admin only)
PUT /api/paymentmethod/123e4567-e89b-12d3-a456-426614174000
Authorization: Bearer {admin_jwt_token}

# Delete payment method (admin only)
DELETE /api/paymentmethod/123e4567-e89b-12d3-a456-426614174000
Authorization: Bearer {admin_jwt_token}
```

## Security Best Practices

### For Admins
1. **Strong Passwords**: Enforce complex passwords for admin accounts
2. **Token Security**: Store JWT tokens securely (never in localStorage)
3. **Network Security**: Use HTTPS for all admin operations
4. **Session Management**: Monitor and limit admin session duration

### For Developers
1. **Role Validation**: Always validate role claims in protected endpoints
2. **Audit Logging**: Log all admin actions for security audits
3. **Principle of Least Privilege**: Only grant admin access when necessary
4. **Regular Review**: Periodically review admin user list

## Admin User Management

### Creating Admin Users
- Use the dedicated `/api/auth/register-admin` endpoint
- Requires email/password authentication (not phone/OTP)
- Sets `UserType = "admin"` automatically

### Admin Token Characteristics
- **Duration**: 30 days (longer than regular users)
- **Claims**: Includes `role: "admin"`
- **Permissions**: Access to protected endpoints

### Monitoring Admin Activity
Track these metrics for security:
- Admin login frequency
- Failed admin login attempts
- Admin endpoint access patterns
- Unusual admin behavior

## Future Enhancements

### Planned Security Features
1. **Admin Approval**: Require existing admin approval for new admin registrations
2. **Multi-Factor Authentication**: Add 2FA for admin accounts
3. **IP Whitelisting**: Restrict admin access to specific IP ranges
4. **Activity Logging**: Detailed audit trails for admin actions
5. **Role Hierarchy**: Different admin levels (super admin, moderator, etc.)

### Additional Protected Endpoints (Future)
- User management (create/update/delete other users)
- System configuration
- Analytics and reporting
- Content moderation tools

## Order Management Security

### 🔒 Admin-Only Order Endpoints

#### GET /api/order
- **Access**: Admin users only
- **Description**: System-wide order listing with pagination
- **Security**: Prevents regular users from seeing all orders across the platform

#### GET /api/order/by-user/{userId}
- **Access**: Admin users only  
- **Description**: View all orders for any specific user
- **Security**: Admin support and user account investigation

### ✅ User-Accessible Order Endpoints

#### GET /api/order/my-orders
- **Access**: Authenticated users (own orders only)
- **Description**: Users can view their own orders

#### GET /api/order/seller-orders
- **Access**: Authenticated users (seller's orders only)
- **Description**: Sellers can view orders for their products

#### GET /api/order/{id}
- **Access**: Authenticated users (with ownership validation)
- **Description**: View specific order details

### Security Implementation
- Admin endpoints use `[Authorize(Roles = "admin")]`
- User endpoints use `[Authorize]` with ownership validation
- Proper error handling for unauthorized access (401/403)

## Category Management Security

### 🔒 Admin-Only Category Endpoints

#### POST /api/category
- **Access**: Admin users only
- **Description**: Create new product categories with image upload
- **Security**: Prevents unauthorized category creation

#### PUT /api/category/{id}
- **Access**: Admin users only
- **Description**: Update existing categories and their images
- **Security**: Ensures only admins can modify catalog structure

#### DELETE /api/category/{id}
- **Access**: Admin users only
- **Description**: Delete categories from the system
- **Security**: Prevents accidental or malicious category removal

### ✅ Public Category Endpoints

#### GET /api/category
- **Access**: Public (anonymous)
- **Description**: Browse available categories
- **Reason**: Users need to see categories for navigation

#### GET /api/category/{id}
- **Access**: Public (anonymous)
- **Description**: View category details
- **Reason**: Required for product browsing

## Payment Method Management Security

### 🔒 Admin-Only Payment Method Endpoints

#### POST /api/paymentmethod
- **Access**: Admin users only
- **Description**: Add new payment methods to the system
- **Security**: Controls available payment options

#### PUT /api/paymentmethod/{id}
- **Access**: Admin users only
- **Description**: Update payment method configuration
- **Security**: Prevents unauthorized payment system changes

#### DELETE /api/paymentmethod/{id}
- **Access**: Admin users only
- **Description**: Remove payment methods
- **Security**: Protects payment system integrity

### ✅ Public Payment Method Endpoints

#### GET /api/paymentmethod
- **Access**: Public (anonymous)
- **Description**: List available payment methods
- **Reason**: Users need to see payment options during checkout

#### GET /api/paymentmethod/{id}
- **Access**: Public (anonymous)
- **Description**: Get payment method details
- **Reason**: Required for payment processing