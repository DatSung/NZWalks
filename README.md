# NZ Walks

NZ Walks is an ASP.NET Core API project following an N-Tier architecture with API, Data Access, Service, and Model layers. It implements the Repository Pattern for data access operations, providing CRUD (Create, Read, Update, Delete) functionality for entities like Region, Walk, and Difficulty. The project also includes user authentication using Identity JWT and integrates third-party libraries such as AutoMapper, API Versioning, Serilog, and JWT Bearer.

## Features

- **Region Management:** Provides endpoints to add, edit, delete, and retrieve information about regions of New Zealand.
- **Walk Management:** Allows CRUD operations on walking trails.
- **Difficulty Management:** Used to define the difficulty level of a walking trail.
- **User Authentication:** Utilizes Identity JWT for user authentication and management.

## Architecture

The project follows an N-Tier architecture with the following layers:

- **API Layer:** Contains API controllers responsible for handling HTTP requests and responses.
- **Service Layer:** Implements business logic and interacts with the Data Access layer.
- **Data Access Layer:** Includes repositories implementing the Repository Pattern for data access operations.
- **Model Layer:** Contains domain models representing entities and DTOs (Data Transfer Objects) for data exchange.

## Requirements

- .NET Core SDK
- Visual Studio or Visual Studio Code
- Database (SQL Server)

## Third-Party Libraries

- AutoMapper
- API Versioning
- Serilog
- JWT Bearer


