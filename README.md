# 📚 Book Subscription Web API

A production-ready **RESTful Web API** built with **ASP.NET Core** that manages books and authors, featuring **secure authentication**, **role-based authorization**, and **API key–based subscriptions**. The project follows modern backend best practices and includes **CI/CD with GitHub Actions**.

---

## 🚀 Features

* 🔐 **Authentication & Authorization**

  * JWT-based authentication
  * Role-based access control (Admin / User)

* 🔑 **API Key Subscriptions**

  * API access controlled via API Keys
  * Subscription-based access to endpoints

* 📚 **Domain Management**

  * Books and Authors relationship
  * Full CRUD operations

* ✅ **Validation & Error Handling**

  * Model validation
  * Global exception handling
  * Production-ready error responses

* 🗄️ **Database**

  * SQL Server
  * Entity Framework Core
  * Code First & Migrations
  * Relational data modeling

* 🧪 **Testing**

  * Unit tests
  * Integration tests

* ⚙️ **CI/CD**

  * Automated build and test pipeline using **GitHub Actions**

---

## 🛠️ Tech Stack

* **Backend:** ASP.NET Core Web API
* **Language:** C#
* **Framework:** .NET
* **ORM:** Entity Framework Core
* **Database:** SQL Server
* **Authentication:** JWT + Roles
* **API Security:** API Key–based subscriptions
* **CI/CD:** GitHub Actions
* **Documentation:** Swagger / OpenAPI

---

## 🧱 Architecture

* RESTful API design
* Separation of concerns
* Dependency Injection
* Environment-based configuration

---

## 🔐 Security Overview

* Secure authentication using JWT tokens
* Role-based authorization for protected endpoints
* API Key validation for subscription access
* Sensitive configuration handled via environment variables

---

## ▶️ Getting Started

### Prerequisites

* .NET SDK
* SQL Server
* Git

### Setup

1. Clone the repository
2. Configure connection strings and secrets in `appsettings.json` or environment variables
3. Apply database migrations
4. Run the application

```bash
dotnet restore
dotnet ef database update
dotnet run
```

---

## 📄 API Documentation

Once running, access Swagger UI at:

```
/ swagger
```

---

## 🧪 Testing

This project includes comprehensive test coverage using **MSTest Framework**, demonstrating test-driven development practices and quality assurance fundamentals.

### Test Structure

The test suite is organized into:

- **Unit Tests**: Isolated testing of business logic, services, and data validation
- **Integration Tests**: End-to-end testing of API endpoints with database interactions
- **Test Coverage**: Focus on critical paths and edge cases

### Running Tests

Execute all tests:
```bash
dotnet test
```

Run tests with detailed output:
```bash
dotnet test --logger "console;verbosity=detailed"
```

Generate test results:
```bash
dotnet test --logger "trx;LogFileName=testresults.trx"
```

### What's Tested

✅ **CRUD Operations**: All Create, Read, Update, Delete operations for Books and Authors  
✅ **Authentication Flow**: Login, token generation, and validation  
✅ **Authorization**: Role-based access control (Admin/User permissions)  
✅ **Input Validation**: Model validation and error handling  
✅ **API Responses**: Correct HTTP status codes and response formats  
✅ **Business Logic**: Service layer methods and data transformations  

### Testing Approach

The project follows testing best practices:

- **Arrange-Act-Assert (AAA)** pattern for test organization
- **Test isolation**: Each test runs independently
- **Descriptive test names**: Clear indication of what's being tested
- **Mock data**: Controlled test environments using test data

### Example Test Coverage
```csharp
// Example from BibliotecaAPITests
[TestClass]
public class BookControllerTests
{
    [TestMethod]
    public void GetAllBooks_ReturnsOkResult()
    {
        // Arrange: Setup test data and dependencies
        // Act: Execute the method being tested
        // Assert: Verify expected outcomes
    }
    
    [TestMethod]
    public void CreateBook_WithInvalidData_ReturnsBadRequest()
    {
        // Test validation and error handling
    }
}
```

### Continuous Testing

All tests are automatically executed in the **CI/CD pipeline** via GitHub Actions on:
- Every push to main branch
- Every pull request
- Before deployment

This ensures code quality and prevents regressions.

---

**💡 Note**: Understanding testing principles from this project has given me valuable insight into **Quality Assurance practices**, including test design, defect identification, and validation strategies—directly applicable to QA roles.

---

## 🔄 CI/CD Pipeline

This project uses **GitHub Actions** to:

* Build the solution
* Run automated tests
* Ensure code quality before merging

---

## 📌 Project Purpose

This project was developed as part of a professional backend training course, with the goal of building a **real-world Web API** using modern .NET technologies and best practices.

---

## 👤 Author

**Fausto Sosa**
.NET Backend Developer
Specialized in ASP.NET Core Web APIs

---

## 📬 Contact

* LinkedIn: https://www.linkedin.com/in/fausto-sosa/
* GitHub: github.com/faustososa22
