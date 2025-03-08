# Expensify-MVC

Expensify-MVC is an ASP.NET MVC web application for expense management with four user roles: Employee, Manager, Accountant, and Admin. Employees manage expenses, Managers approve/reject them, Accountants mark them as paid, and Admins oversee all with Chart.js reports.

## Features
- **Expense Management:**
  - Employees can add, update, and delete expenses.
  - Managers can approve or disapprove expenses.
  - Employees can view and correct disapproved expenses.
  - Accountants can mark approved expenses as paid.
- **Reporting:**
  - Admins view all operations and generate reports with Chart.js.
- **Authentication & Authorization:**
  - ASP.NET Identity for user roles.
  - JWT Token for API authorization.
- **Responsive Design:** Mobile and desktop compatible.

## Technologies
- **Framework:** ASP.NET MVC (.NET 6/8)
- **Language:** C#
- **Database:** SQL Server 2022, Entity Framework Core
- **Authentication:** ASP.NET Identity
- **Authorization:** JWT Token
- **Design Pattern:** Repository Pattern
- **Frontend:** HTML, CSS, JavaScript, Bootstrap, Chart.js
- **Tools:** Visual Studio 2022, SSMS

## Prerequisites
- .NET SDK (e.g., .NET 6/8)
- Visual Studio 2022 (ASP.NET workload)
- SQL Server 2022
- SSMS

## Installation
1. Clone: `git clone https://github.com/celikmehmetfatihh/Expensify---MVC.git`
2. Navigate: `cd Expensify---MVC/Expensify/Expensify.Web`
3. Open `Expensify.sln` in Visual Studio.
4. Restore NuGet packages.
5. Set up `ExpensifyDB` in SQL Server (run EF migrations).
6. Update `appsettings.json` with your connection string:
   "ConnectionStrings": {
     "DefaultConnection": "Server=FATIh;Database=ExpensifyDB;Trusted_Connection=True;"
   }
