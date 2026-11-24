# IdentityHub 🔐
### ASP.NET Core Identity & JWT Authentication API

IdentityHub is a practical authentication and authorization Web API built with ASP.NET Core Identity. It demonstrates user identity management, secure authentication, password management, two-factor authentication, and role-based access control.

## 🚀 Features

### 1. Authentication
- User Registration
- User Login
- User Logout
- JWT Access Token Generation
- Refresh Token Management
- Get Current User Information

### 2. Email & Password Management
- Email Confirmation
- Resend Email Confirmation
- Forgot Password
- Reset Password
- Change Password

### 3. Two-Factor Authentication (2FA)
- Set Up Two-Factor Authentication
- Enable 2FA
- Disable 2FA
- Verify 2FA Code
- Recovery Codes

### 4. User & Role Management
- Get All Users
- Get User By ID
- Update User Information
- Assign Roles to Users
- Remove Roles from Users

## 🛠️ Tech Stack

- ASP.NET Core Web API
- ASP.NET Core Identity
- Entity Framework Core
- SQL Server
- JWT Bearer Authentication
- MediatR
- C#
- Swagger / OpenAPI

## 🏗️ Project Structure

```text
IdentityHub/
│
├── Controllers/
│
├── Data/
│
├── DTOs/
│
├── Helpers/
│
├── Interfaces/
│
├── Middleware/
│
├── Migrations/
│
├── Models/
│
├── Services/
│
├── Program.cs
│
└── appsettings.json
```

## ⚙️ Getting Started

### Prerequisites
- .NET SDK
- SQL Server
- Visual Studio or Visual Studio Code

### 1. Clone the Repository

```bash
git clone https://github.com/YOUR_USERNAME/IdentityHub.git
```

### 2. Configure the Database

Update the `DefaultConnection` in your configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_CONNECTION_STRING"
  }
}
```

### 3. Configure JWT

```json
{
  "Jwt": {
    "Key": "YOUR_SECRET_KEY",
    "Issuer": "YOUR_ISSUER",
    "Audience": "YOUR_AUDIENCE"
  }
}
```

Use User Secrets or environment variables for sensitive configuration.

### 4. Apply Database Migrations

```bash
dotnet ef database update
```

### 5. Run the Application

```bash
dotnet run
```

## 🔒 Security

- ASP.NET Core Identity for user and password management.
- JWT Bearer Authentication for securing API endpoints.
- Role-Based Authorization for access control.
- Secure password management workflows.

## 🎯 Project Goals

- Understand ASP.NET Core Identity and its practical usage.
- Implement secure authentication and authorization workflows.
- Explore JWT-based authentication.
- Practice user and role management.
- Build a reusable authentication API for future projects.

## 📌 Project Status

Under Development 🚧

## 👨‍💻 Author

**Abdelrahman Mohammed**

Computer Science Student | ASP.NET Core Backend Developer

---

⭐ If you find this project useful, consider giving it a star!
