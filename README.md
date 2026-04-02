# HealthTracker

A comprehensive health tracking application built with .NET 9.0 following Clean Architecture principles. This application allows users to track their health metrics, manage their profiles, and gain insights into their health data.

## Project Structure

The solution follows Clean Architecture with the following projects:

```
HealthTracker/
├── HealthTracker.Domain/          # Domain layer - Entities and core business objects
├── HealthTracker.Application/     # Application layer - Business logic, DTOs, services
├── HealthTracker.Infrastructure/  # Infrastructure layer - Data access, external services
├── HealthTracker.API/             # API layer - RESTful API endpoints
└── HealthTracker.Web/             # Web layer - Razor Pages UI
```

### Layer Descriptions

- **Domain**: Contains enterprise business logic and types (Entities, Value Objects, Domain Events)
- **Application**: Contains application business rules and interfaces
- **Infrastructure**: Implements interfaces from Application layer, contains EF Core, external services
- **API**: Exposes RESTful API endpoints for client applications
- **Web**: Razor Pages based web interface for end users

## Features

- **User Authentication & Authorization**
  - JWT-based authentication
  - ASP.NET Core Identity integration
  - Secure password hashing with BCrypt
  
- **Health Metrics Tracking**
  - Track various health metrics (weight, blood pressure, glucose levels, etc.)
  - Custom metric types support
  - Historical data visualization
  
- **RESTful API**
  - Fully documented with Swagger/OpenAPI
  - Controller-based architecture
  - Centralized exception handling
  
- **Web Interface**
  - Razor Pages based UI
  - User-friendly dashboard
  - CRUD operations for health metrics

## Technology Stack

- **.NET 9.0** - Latest .NET runtime
- **ASP.NET Core** - Web framework
- **Entity Framework Core 9.0** - ORM for data access
- **SQL Server** - Database
- **ASP.NET Core Identity** - User management
- **JWT Bearer Authentication** - API security
- **AutoMapper** - Object-to-object mapping
- **FluentValidation** - Validation library
- **Serilog** - Structured logging
- **Swagger/OpenAPI** - API documentation

## Getting Started

### Prerequisites

- .NET 9.0 SDK or later
- SQL Server (LocalDB, Express, or full version)
- Visual Studio 2022 or VS Code (optional)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd HealthTracker
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Configure the database connection**
   
   Update the connection string in `appsettings.json` (API and Web projects):
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HealthTracker;Trusted_Connection=True;"
     }
   }
   ```

4. **Apply database migrations**
   ```bash
   dotnet ef database update --project HealthTracker.Infrastructure --startup-project HealthTracker.API
   ```

5. **Run the application**
   
   To run the API:
   ```bash
   dotnet run --project HealthTracker.API
   ```
   
   To run the Web application:
   ```bash
   dotnet run --project HealthTracker.Web
   ```

## Configuration

### appsettings.json

Key configuration sections:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your-connection-string-here"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "HealthTracker",
    "Audience": "HealthTrackerUsers",
    "ExpirationInMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

## API Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/auth/register` | POST | Register new user |
| `/api/auth/login` | POST | User login |
| `/api/users/profile` | GET | Get user profile |
| `/api/users/profile` | PUT | Update user profile |
| `/api/healthmetrics` | GET | Get all health metrics |
| `/api/healthmetrics/{id}` | GET | Get specific metric |
| `/api/healthmetrics` | POST | Create new metric |
| `/api/healthmetrics/{id}` | PUT | Update metric |
| `/api/healthmetrics/{id}` | DELETE | Delete metric |
| `/api/metictypes` | GET | Get all metric types |
| `/api/ai/insights` | GET | Get AI-powered health insights |

## Development

### Building the Solution

```bash
dotnet build HealthTracker.slnx
```

### Running Tests

```bash
dotnet test
```

### Code Style

This project follows Microsoft's C# coding conventions with:
- Nullable reference types enabled
- Implicit usings enabled
- Modern C# features (.NET 9.0)

## Architecture Patterns

- **Repository Pattern**: Data access abstraction
- **Dependency Injection**: Built-in .NET DI container
- **CQRS-inspired**: Separation of commands and queries
- **Middleware Pipeline**: Custom exception handling middleware
- **DTO Pattern**: Data transfer between layers

## Security

- Password hashing using BCrypt
- JWT token-based authentication
- Role-based authorization
- Input validation using FluentValidation
- CORS policy configuration
- HTTPS enforcement

## Logging

Structured logging is implemented using Serilog with:
- Console output
- File output (configured in appsettings)
- Custom log formatting
- Request/response logging

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- .NET Foundation
- ASP.NET Core Team
- All contributors to the libraries used in this project
