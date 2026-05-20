# <p align="center">✨ ExpenseTracker (ASP.NET Core MVC) ✨</p>

<p align="center">
  <strong>A premium, modern, and production-grade personal finance SaaS-like web application.</strong>
</p>

<p align="center">
  <a href="https://dotnet.microsoft.com/en-us/apps/aspnet/mvc"><img src="https://img.shields.io/badge/.NET-10.0-blueviolet?style=for-the-badge&logo=dotnet" alt=".NET 10" /></a>
  <a href="https://learn.microsoft.com/en-us/ef/core/"><img src="https://img.shields.io/badge/EF%20Core-ORM-blue?style=for-the-badge&logo=dotnet" alt="EF Core" /></a>
  <a href="https://www.microsoft.com/en-us/sql-server"><img src="https://img.shields.io/badge/SQL%20Server-Database-red?style=for-the-badge&logo=microsoft-sql-server" alt="SQL Server" /></a>
  <a href="https://getbootstrap.com/"><img src="https://img.shields.io/badge/Bootstrap-5.3-purple?style=for-the-badge&logo=bootstrap" alt="Bootstrap 5" /></a>
  <a href="https://playwright.dev/dotnet/"><img src="https://img.shields.io/badge/Playwright-PDF%20Export-green?style=for-the-badge&logo=playwright" alt="Playwright" /></a>
  <img src="https://img.shields.io/badge/Theme-Light%20%2F%20Dark-darkgreen?style=for-the-badge" alt="Theme Toggle" />
</p>

---

## 🌟 Executive Summary

**ExpenseTracker** is a sleek, multi-module personal finance dashboard that enables users to securely record, audit, and forecast their financial health. 

Built with software engineering best practices in mind, this application offers real-time account balance synchronization, automated recurring transactional scheduling, structured budget enforcement, dynamic data visualization, and professional-grade server-side report generation (PDF & Excel) with complete multi-tenant tenant-like data isolation.

---

## 🚀 Core Features & High-End UI

| Module | Feature | UI Highlights |
| :--- | :--- | :--- |
| 💳 **Account Management** | Multiple active wallets (Cash, Bank, Savings, Credit Cards) | Protective delete constraints & running balance indicators. |
| 💸 **Transactional Engines** | Dual-track Expense & Income ledgers linked to accounts | Multi-step interactive forms, modern responsive data tables. |
| 📈 **Budget Planning** | Monthly expense caps set by specific categories | Progress-bar metrics reflecting actual vs. budgeted spending. |
| 🔄 **Recurring Automations** | Cron-like frequency processing (Daily, Weekly, Monthly, Yearly) | Silent opportunistic execution updates on dashboard load. |
| 📊 **Advanced Analytics** | Dynamic periods reporting & category breakdowns | Premium typography, dark mode-ready charts via **Chart.js** & Lucide icons. |
| 📤 **Enterprise Exports** | High-fidelity server-side reports | Headless **Playwright (Chromium)** PDFs & **ClosedXML** Spreadsheets. |

---

## 🛡️ Architecture & Technical Design

ExpenseTracker adheres to a modern, decoupled three-tier MVC design pattern with structured services handling high-overhead automation and document compilation.

### 📐 System Topology & Data Flow

This Mermaid blueprint visualizes how components interact:

```mermaid
graph TD
    User([User / Browser]) <--> |HTTP Requests / Web UI| Views[Razor Views / Bootstrap 5 / Chart.js / Lucide]
    Views <--> Controllers[MVC Controllers / Filter Attributes]
    Controllers <--> Services[Application Service Layer]
    
    subgraph Core Services [Business & Rendering Services]
        RT[RecurringTransactionService]
        VR[ViewRenderService]
        EX[ExportService]
    end
    
    subgraph Storage [Persistent Data Layer]
        DB[ApplicationDbContext] [(SQL Server Database)]
        ID[ASP.NET Core Identity]
    end
    
    Controllers <--> RT
    Controllers <--> VR
    Controllers <--> EX
    Controllers <--> DB
    Controllers <--> ID
    
    EX -->|Chromium Headless| PW[Microsoft Playwright PDF Engine]
    EX -->|In-Memory Workbook| CX[ClosedXML Excel Generator]
```

### 🧠 High-Fidelity Business Logic Notes

*   **Transactional Integrity**: Account balance operations use synchronous database locking. Adding a transaction updates `CurrentBalance` in real time, and deletions or editing revert historical modifications automatically to prevent math skew.
*   **Opportunistic Execution Engine**: The `RecurringTransactionService` processes pending recurring transactions when a user visits their dashboard, ensuring zero-overhead background scheduler costs while keeping data perfectly fresh.
*   **Complete Tenant Isolation**: Every ledger operation is automatically queried against a scoped `UserId` resolving from the authenticated ClaimsPrincipal, preventing data leaks across registered accounts.

---

## 📂 Developer Deep Dive & Setup

Click any of the dropdowns below to explore project details, schemas, local setup, and troubleshooting documentation:

<details>
<summary>📁 View Detailed Project Structure</summary>

```text
ExpenseTracker/
├── assets/                     # Visual brand assets (banner, logos)
├── ExpenseTracker.slnx         # Modern Visual Studio Solution file
└── ExpenseTracker/             # Primary Project Source Code
    ├── Controllers/            # Route orchestration & authorization flows
    │   ├── AccountController.cs
    │   ├── ExpenseController.cs
    │   ├── IncomeController.cs
    │   ├── BudgetController.cs
    │   ├── WalletController.cs
    │   ├── RecurringTransactionController.cs
    │   ├── ReportController.cs
    │   └── ExportController.cs
    ├── Data/
    │   └── ApplicationDbContext.cs # Entity mapping and migration setups
    ├── Models/
    │   ├── Entities/           # Database Domain entities
    │   │   ├── Account.cs
    │   │   ├── Expense.cs
    │   │   ├── Income.cs
    │   │   ├── Category.cs
    │   │   ├── IncomeCategory.cs
    │   │   ├── Budget.cs
    │   │   ├── RecurringTransaction.cs
    │   │   └── ApplicationUser.cs
    │   └── ViewModels/         # UI Data transfer objects
    │       ├── DashboardViewModel.cs
    │       ├── MonthlySummaryViewModel.cs
    │       ├── FullReportViewModel.cs
    │       └── Exports/
    ├── Services/               # Encapsulated Business Logic layer
    │   ├── RecurringTransactionService.cs
    │   ├── ViewRenderService.cs
    │   └── ExportService.cs
    ├── Views/                  # Adaptive Razor pages
    │   ├── Shared/
    │   ├── Expense/
    │   ├── Income/
    │   ├── Budget/
    │   ├── Wallet/
    │   ├── Report/
    │   └── ExportTemplates/     # Document generation templates
    ├── wwwroot/                # Static assets
    │   ├── css/site.css        # Core custom styles (Adaptive Dark Theme)
    │   ├── js/site.js          # Reactive UI widgets & LocalStorage preference toggles
    │   └── lib/
    ├── Program.cs              # DI Services container setup & middleware
    └── appsettings.json        # Configurations (Connection strings, logging)
```
</details>

<details>
<summary>💾 View Relational Data Model Schema</summary>

The application utilizes an elegant relational database topology mapped via EF Core migrations:

```
  ┌─────────────────┐       ┌─────────────────┐
  │ ApplicationUser │       │     Account     │
  └────────┬────────┘       └────────┬────────┘
           │                         │
           ├───────────┐             │
           ▼           ▼             ▼
      ┌─────────┐ ┌─────────┐   ┌─────────┐
      │ Expense │ │ Income  │◄──┤ Wallet  │
      └────┬────┘ └────┬────┘   └─────────┘
           │           │
           ▼           ▼
      ┌─────────┐ ┌─────────┐
      │Category │ │IncCateg │
      └─────────┘ └─────────┘
```

*   **`ApplicationUser`**: Standard Identity definition with a unique primary key.
*   **`Account`**: Represents a wallet holding a running ledger balance (`InitialBalance`, `CurrentBalance`). Contains constraints to block deletions if associated transactions exist.
*   **`Expense` / `Income`**: Individual transaction entries. Every record links directly to both an `Account` and a `Category` / `IncomeCategory`.
*   **`Budget`**: Configured caps scoped monthly against specific expense Categories.
*   **`RecurringTransaction`**: Rule engines tracking automated future expenses/incomes. Features automated frequency increments.
</details>

<details>
<summary>⚙️ View Local Setup & Dependencies</summary>

### Prerequisites
*   **.NET SDK 10.0+**
*   **SQL Server** (LocalDB, Express, or Full instance)
*   **PowerShell** (for installing automated client browsers)

### 1. Database Connection Configuration
Modify `ExpenseTracker/appsettings.json` with your database credentials:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=ExpenseTrackerDB;Trusted_Connection=True;TrustServerCertificate=True"
}
```

### 2. Dependency Resolution
Restore NuGet packages from the solution root:
```bash
dotnet restore
```

### 3. Apply Schema Migrations
Construct the database tables through EF Core:
```bash
dotnet ef database update --project ExpenseTracker
```

### 4. Headless PDF Engine Installation
The server-side PDF generator uses Microsoft Playwright to build responsive document formats. Build the project and install Chromium:
```bash
# Build the project first
dotnet build

# Install Chromium binaries via Playwright powershell utility
pwsh bin/Debug/net10.0/playwright.ps1 install chromium
```
</details>

<details>
<summary>🚀 View How to Run & Seed Account details</summary>

### Run commands
Launch the ASP.NET Core server directly using the CLI:
```bash
dotnet run --project ExpenseTracker
```

Upon launching, the app runs at the following default development addresses:
*   **HTTPS**: `https://localhost:7139`
*   **HTTP**: `http://localhost:5015`

### 🔑 Instant Testing Accounts
If the database is initialized with zero user entries, the seeding service registers an instant demo account on startup:

*   **Email Address**: `demo@example.com`
*   **Password**: `Demo@123`

You can use these credentials to immediately explore the system dashboards.
</details>

<details>
<summary>🛠️ View System Troubleshooting Guides</summary>

### 🔴 1. PDF Export Service Returns "Engine Unavailable"
*   **Cause**: The headless Chromium environment is missing or was installed under a different user scope.
*   **Solution**: Ensure you run `dotnet build` before the Playwright command. Execute `pwsh bin/Debug/net10.0/playwright.ps1 install chromium` with administrator rights if directory permissions block the download.

### 🔴 2. Database Connection Failures
*   **Cause**: SQL Server service is stopped, or the connection parameters are incorrect.
*   **Solution**: Ensure your connection string includes `TrustServerCertificate=True` if you are using developer-signed local SSL certificates. Verify that the SQL Server service (`MSSQLSERVER` or `MSSQL$SQLEXPRESS`) is actively running in Windows Services.

### 🔴 3. Entity Framework Command Unrecognized
*   **Cause**: The EF CLI tool is not globally installed.
*   **Solution**: Run `dotnet tool install --global dotnet-ef` before trying to apply database updates.

### 🔴 4. "File in Use" Lock on DLLs
*   **Cause**: An active debugging session or hot-reload process is holding an exclusive handle.
*   **Solution**: Stop any active IIS Express, Kestrel, or Visual Studio processes before running updates or rebuilding the solution.
</details>

---

## 📈 Roadmap & Upcoming Features

- [ ] **Decoupled Background Queue**: Transition recurring transaction executions into a hosted service worker (`IHostedService`) for decoupled enterprise scheduling.
- [ ] **Branded Export Layouts**: Enable users to upload business logo layouts to overlay on PDF invoices.
- [ ] **Custom Date Range Auditing**: Expand PDF/Excel reports to support completely custom filtering periods.
- [ ] **Telemetry Audit Logs**: Introduce structured logging tracking all export file downloads.

---

> [!NOTE]
> **Onboarding Contributor Tip**: Start exploring by reviewing `Program.cs` to understand middleware bindings, navigate into `ExportController` + `ExportService` to inspect document rendering systems, and analyze `wwwroot/css/site.css` to grasp the adaptive CSS theme system.
