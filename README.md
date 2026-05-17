# Carvia 🛡️📋

**Carvia** is a high-octane digital automotive registry and premium journalism showcase platform engineered around a high-performance **Vertical Slice Architecture**. Embodying a bold, brutalist design aesthetic, it rejects conventional flat interfaces in favor of high-contrast "hazard" visual metrics, raw typographic weight, and an uncompromising dark canvas workflow.

The entire solution runs inside a highly optimized monolithic structure powered by **.NET 10 (ASP.NET Core MVC with Razor Views)** backend processing combined with a compiled client pipeline utilizing **Tailwind CSS v4**.

## 🌐 Overview

Carvia establishes an editorial standard for vehicle spec logging, inventory curation, and technical asset tracking:

* **Vertical Slice Architecture:** Built following Screaming Architecture principles, code layout boundaries are organized strictly by functional feature slices (Cars, CuratorItems, Authentication) rather than classical database layer tiers.
* **Hybrid Authentication Engine:** Features a dual-layered authorization security perimeter that pairs traditional stateful ASP.NET Core Cookie sessions for secure user-facing view access with stateless JSON Web Tokens (JWT) for secure data pipeline exchanges.
* **Brutalist Visual Identity:** Employs an ultra-modern design language mapped to an open-loop grid system, utilizing heavy bounding structures, solid color elevations, and a piercing signature color palette centered around Acid Mint (#3cffd0) highlights.
* **Feature View Location Expansion:** Leveraged customized Razor execution structures to map MVC view resolution paths right into the feature slices folder matrix.

## ✨ Features

* **🔐 Security & Cryptographic Integrity**
  * Hardened Hash Protocols: Leverages standard HMACSHA512 encryption architectures to perform secure salt-and-hash credential validation routines.
  * Domain Validation Boundary: Employs central business rule abstractions (UserBusinessRules, CarBusinessRules) to intercept unauthorized parameters before transactions touch the persistence layer.
  * Cookie Rigor: Fully locked down authentication session cookies matching HttpOnly, SameSite=Strict, and request-matching secure policies.

* **🏁 Global Live Catalog & Specs Showcase**
  * Double-Column Stream: An interactive repository displaying available hypercar models with smooth detail routing transitions.
  * Technical Spec Matrix: High-contrast grids displaying market evaluations, releasing years, model classifications and multi-angle structural gallery frames.

* **🎯 Vault Curator Mode**
  * Custom Vision Logs: Allows registered collectors to instantly archive inventory vehicles and map custom markdown-style commentary notes.
  * Refined Management Actions: Full CRUD lifecycle integration enabling quick item removals, updates and direct specification reviews from a personal archive dashboard.

## 🛠️ Tech Stack

**Tech Stack**
* Core Runtime: .NET 10 (ASP.NET Core MVC
* Data Access Layer: Entity Framework Core 10 (SQL Server)
* Design Engine: Tailwind CSS v4.2 + PostCSS Build Pipeline
* Identity Management: Native ASP.NET Core Claims-Based Security (Cookies + JWT Bearer Validation Tokens)

## 📂 Project Structure

```text
Carvia/
├── Core/                           # Infrastructure & Common Logic Shared Abstractions
│   ├── Entities/                   # Core Domain Entities (BaseEntity.cs)
│   ├── Exceptions/                 # Central Exception Registry (BusinessException.cs, NotFoundException.cs)
│   ├── Models/                     # Common Transport Records (ReturnModel.cs, TokenOptions.cs)
│   ├── Persistence/                # Unit of Work Abstract Interfaces (IUnitOfWork.cs)
│   ├── Repositories/               # Generic Data Repositories Contracts (IRepository.cs)
│   └── Utilities/                  
│       ├── Files/                  # Storage Utilities (FileHelper.cs)
│       └── Security/               # Cryptographic Utilities (HashingHelper.cs)
├── Features/                       # Vertical Slices (Self-Contained Domain Slices)
│   ├── Authentication/             # User Identity Registration and Login Handlers
│   │   ├── Views/                  # Login.cshtml, Register.cshtml
│   │   └── [BusinessRules, Controller, Service, ViewModels, Registration].cs
│   ├── Carlimages/                 # Car Media Processing Logic Matrix
│   ├── Cars/                       # Global Live Vehicle Catalog Stream
│   │   ├── Views/                  # Showcase.cshtml, Details.cshtml, Create.cshtml, Index.cshtml
│   │   └── [Entity, BusinessRules, Controller, Service, ViewModels, Registration].cs
│   ├── Categories/                 # Classification Engine Slices
│   ├── CuratorItems/               # User Collection Curation & Commentary Loggers
│   │   ├── Views/                  # Index.cshtml, Create.cshtml, Edit.cshtml
│   │   └── [Entity, BusinessRules, Controller, Service, ViewModels, Registration].cs
│   ├── Roles/                      # Authorization Rights Assignment Matrix
│   ├── Shared/                     # Global Shared Application Components Layouts
│   │   └── Views/                  # _Layout.cshtml, _Navbar.cshtml, _Footer.cshtml, Legal.cshtml
│   └── Users/                      # Profile Identity Modification and Telemetry Systems
│       ├── Views/                  # Profile.cshtml, Edit.cshtml
│       └── [Entity, BusinessRules, Controller, Service, ViewModels, Registration].cs
├── Infrastructure/                 # Technical Concrete Providers Implementation
│   ├── Contexts/                   # EF Core DB Access Layer Mapping (BasedDbContext.cs)
│   ├── Controllers/                # Intermediary Controller Anchors (BaseController.cs)
│   ├── Data/                       # Persistence Workload Implementations (UnitOfWork.cs, DataRegistration.cs)
│   ├── Middlewares/                # Server Pipeline Hooks (GlobalExceptionHandler.cs)
│   └── Razor/                      # View Location Overrides Engine (FeatureViewLocationExpander.cs)
├── Migrations/                     # Persistent Database Schema Version Control Records
├── wwwroot/                        # Compiled Output UI Staging Ground (Tailwind Distribution Files)
├── appsettings.json                # Telemetry Connection Strings and Token Signing Configurations
├── package.json                    # Tailwind CSS Node CLI Compilation Rules
└── Program.cs                      # Application Entry Bootstrap and Middleware Pipe Configuration
```