# Banking Solution (.Net9)

## Overview
This project is a **.NET 9** application built using **Clean Architecture** principles and .Net9. The goal is to ensure:

- Clear separation of concerns
- High testability
- Maintainability and scalability
- Independence from frameworks and infrastructure details

The solution is designed so that business logic remains stable even if external technologies (database, UI, frameworks) change.

---

## Solution Structure

The solution is organized into multiple projects, each with a clear responsibility:

```
/src
 ├── Domain
 ├── Application
 ├── Infrastructure
 └── Api
```

## Prerequisites

To run this application on **any operating system (Windows, macOS, Linux)**, ensure the following are installed:

### Required Software

1. **.NET 9 SDK**
   - Download: https://dotnet.microsoft.com
   - Verify installation:
     ```bash
     dotnet --version
     ```

2. **Database Server** (depending on configuration)
   - MySQL
   - Or Docker (recommended for consistency)

3. **Git**
   - Download: https://git-scm.com

---

## Cloning the Repository

```bash
git clone https://github.com/codedfellow/BankingSolution.git
cd BankingSolution
```

---

## Configuration (IMPORTANT)

### Updating the Connection String

Before running the application, **you must update the database connection string**.

1. Navigate to:
   ```
   src/Api/appsettings.json
   ```

2. Update the `ConnectionStrings` section:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3306;Database=BankingSolution;User=userName;Password=dbPassword;CharSet=utf8mb4;"
}
```

> ⚠️ Once the connection string has been updated you can run the application and the database will be created

---

## Running the Application

```bash
dotnet restore
dotnet build
dotnet run --project API
```

The API will start and be available at:

```
https://localhost:7057
http://localhost:5297
```

Swagger UI (if enabled):
```
https://localhost:7057/Index.html or http://localhost:5297/index.html
```

---


## Common Commands

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test

# Run Web API
dotnet run --project API
```

---


## Troubleshooting

- Ensure .NET 9 SDK is installed
- Confirm correct connection string in `appsettings.json`
- Verify database server is running
- Run `dotnet clean` if build errors occur

---


## License

This project is proprietary / MIT (update as needed).