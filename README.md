# Nilearn Backend API

<div align="center">
  <h3>A scalable, production-ready backend for an E-Learning platform</h3>
</div>

## 📖 Overview
Nilearn is a robust backend API designed to power a modern e-learning platform (similar to Udemy). Built with **.NET 10**, it strictly adheres to **Clean Architecture** principles and implements the **CQRS (Command Query Responsibility Segregation)** pattern using **MediatR**. The application provides a comprehensive, secure, and performant suite of features for managing users, courses, financial transactions, and content delivery.

## ✨ Key Features
- **Robust User Management & Auth**: JWT-based authentication, role-based authorization (Admin, SuperAdmin, Instructor, Student), and refresh token mechanisms using ASP.NET Core Identity.
- **Advanced Course Management**: Hierarchical course structuring (Courses > Sections > Lessons) with support for multi-criteria search, filtering, and categorization.
- **Rich & Secure Lesson Content**: Support for various lesson formats (Video, PDF, and interactive Articles). Content delivery is heavily secured based on strict enrollment status and ownership verification.
- **Financial & Wallet System**: A comprehensive wallet system for managing platform revenue and instructor funds, featuring complete transaction tracking and revenue splitting.
- **Production-Grade Review System**: Fully featured course review and rating system ensuring data integrity (preventing duplicates, validating enrollments).
- **Asynchronous Background Processing**: Reliable background job scheduling powered by Hangfire for email notifications and media processing.
- **Media Management**: Seamless integration with Cloudinary for secure file storage, media uploads, and delivery.
- **Exception-Based Error Handling**: Consistent, RFC-compliant error responses across the entire application using global exception middleware, ensuring a clean "happy path" in the application layer.

## 🛠️ Technology Stack
- **Framework**: .NET 10 (ASP.NET Core Web API)
- **Architecture**: Clean Architecture, CQRS, Domain-Driven Design (DDD) concepts
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core 10
- **Mediator Pattern**: MediatR
- **Validation**: FluentValidation
- **Background Jobs**: Hangfire (backed by PostgreSQL)
- **Security**: ASP.NET Core Identity, JWT Bearer Authentication
- **Media Storage**: CloudinaryDotNet
- **Logging**: Serilog
- **API Documentation**: OpenAPI / Swagger

## 📂 Project Structure
The solution is logically separated into five main projects to enforce separation of concerns:

- `Nilearn.API`: The presentation layer containing Controllers, global Middlewares (e.g., Exception Handling, Request Logging), and dependency injection composition.
- `Nilearn.Application`: Contains use cases, business rules, CQRS Commands/Queries, Handlers, Fluent Validators, and DTOs.
- `Nilearn.Domain`: The core of the system containing business entities (e.g., `Course`, `Lesson`, `Wallet`), enums, domain exceptions, and repository interfaces.
- `Nilearn.Infrastructure`: Implementations of data access (EF Core Repositories, Migrations), external services (Cloudinary, Email), and identity management.
- `Nilearn.Shared`: Common models, constants, and utilities shared across the solution.

## 🚀 Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [PostgreSQL](https://www.postgresql.org/download/)
- A [Cloudinary](https://cloudinary.com/) account for media storage
- SMTP credentials for email delivery

### Installation & Setup
1. **Clone the repository**:
   ```bash
   git clone https://github.com/Ibrahim-Mohamed66/Nilearn.Backend.git
   cd Nilearn
   ```

2. **Configure Environment Variables & AppSettings**:
   Update `appsettings.json` or `appsettings.Development.json` in the `Nilearn.API` project.
   Ensure you set the `PG_PASSWORD` environment variable or explicitly replace `${PG_PASSWORD}` in the ConnectionStrings.

   Key configurations required:
   - `ConnectionStrings:DefaultConnection`
   - `AppSettings:FrontendUrl`
   - `JwtSettings`
   - `CloudinarySettings`
   - `SmtpSettings`

3. **Apply Database Migrations**:
   Open a terminal in the `Nilearn.API` directory and run:
   ```bash
   dotnet ef database update -p ../Nilearn.Infrastructure -s .
   ```

4. **Run the Application**:
   ```bash
   dotnet run --project Nilearn.API
   ```
   The API will start at `https://localhost:<port>`. 
   In the development environment, navigate to `https://localhost:<port>/swagger` to view and interact with the API documentation.

## 📊 Background Jobs Dashboard
Hangfire is fully configured for asynchronous task processing. Once the application is running, authenticated administrators can monitor background jobs by navigating to:
`https://localhost:<port>/hangfire`

## 🤝 Contributing
Contributions, issues, and feature requests are welcome! Feel free to check the issues page.

## 📝 License
This project is proprietary and confidential. All rights reserved.