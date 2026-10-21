# NutriWeek

NutriWeek is an ASP.NET Core MVC web application for planning daily and weekly meals.

Users can create and manage dishes, add meals for specific dates, and organize them into daily and weekly menus. Each registered user has private meal plans, while shared dishes are available to everyone.

---

## Features

- User registration and login with ASP.NET Core Identity
- Shared and personal dishes
- Full CRUD operations for dishes
- Add, edit and delete meals
- Daily meal planning
- Weekly meal planning
- User-specific data
- Server-side and client-side validation
- Responsive design

---

## Main Entities

### Dish
Represents a recipe or food item.

### WeeklyMenu
Represents one week of meal planning.

### DailyMenu
Represents one specific day inside a weekly menu.

### Meal
Represents a selected dish for a specific date and meal type.

The main relationship is:

```text
WeeklyMenu
    ↓
DailyMenu
    ↓
Meal
    ↓
Dish
```

---

## Technologies Used

- ASP.NET Core MVC
- .NET 8
- C#
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- Razor Views
- Dependency Injection
- Bootstrap
- HTML / CSS / JavaScript
- Git and GitHub

---

## Authentication

NutriWeek uses ASP.NET Core Identity.

Users can:

- Register
- Login
- Logout

Private dishes, meals and menus are connected to the currently logged-in user.

Shared dishes can be used by all registered users.

---

## Validation

The application uses both server-side and client-side validation to prevent invalid data from being saved.

---

## Setup

### 1. Clone the repository

```bash
git clone YOUR_GITHUB_REPOSITORY_URL
```

### 2. Open the solution

Open the `.sln` file in Visual Studio.

### 3. Configure the database

The SQL Server connection string is stored in:

```text
appsettings.json
```

Example:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=NutriWeek;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

No external API keys are required.

### 4. Apply migrations

Open Package Manager Console and run:

```powershell
Update-Database -Project NutriWeek.Data -StartupProject NutriWeek
```

### 5. Run the application

Start the project from Visual Studio.

---

## Initial Data

The application includes shared dishes that are available to all users.

Weekly menus, daily menus and meals are created separately for each user.

---

## Project Purpose

NutriWeek was created as an ASP.NET Fundamentals course project to demonstrate:

- MVC architecture
- Entity Framework Core
- CRUD operations
- Dependency Injection
- Validation
- Authentication
- Database relationships
- Razor Views
- Responsive web design

---

## Author
Aleksandra Novovich

                                       ASP.NET Fundamentals Project
