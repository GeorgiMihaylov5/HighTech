# HighTech 🖥️

HighTech is a full-stack e-commerce web application for selling computer hardware and building custom PC configurations.

The project was developed as a diploma project with a focus on scalable architecture, dynamic product management and intelligent assistance when choosing computer components.

## ✨ Key Features

* **Product Catalog** – browsing, filtering, product details, favorites, reviews and comparison.
* **PC Configurator** – build a custom PC while automatically checking component compatibility.
* **AI Assistant** – integrated AI assistant that can create and modify configurations based on natural-language requirements and suggest corrections for incompatible components.
* **Dynamic Product Catalog** – administrators can create new product categories and characteristics without modifying application code.
* **Order Management** – complete order workflow for customers and employees.
* **Role-Based Access** – Guests, Clients, Employees and Administrators.
* **Administration Panel** – management of products, categories, characteristics, users, employees and sales-related information.

## 📸 Application Preview

### 🏠 Home Page

The home page provides the main entry point to the platform, giving users quick access to the product catalog, PC configurator and other core functionality.

<img width="500" alt="HomePage" src="https://github.com/user-attachments/assets/64a8311e-a416-4539-a76d-49a11ac9d3ff" />

### 🖥️ PC Configurator

The PC configurator allows users to build a custom computer while the system automatically validates component compatibility.

<img width="900" alt="configurator" src="https://github.com/user-attachments/assets/5a447254-e3cc-4256-94bf-87f9fa799dc5" />

### 🤖 AI Assistant

The AI assistant allows users to describe their requirements in natural language and helps create or modify PC configurations. It can also suggest solutions when incompatible components are selected.

<img width="300" alt="ai chatbot" src="https://github.com/user-attachments/assets/27986ae1-c02b-401a-9531-fefb72e3852b" />

## 🏗️ Architecture
The application follows a **Client-Server architecture**:

**Angular SPA → REST/JSON → ASP.NET Core Web API → Business Services / AI Assistant / Compatibility Engine → SQL Server**

The backend uses a three-layer architecture separating Controllers, Business Services and Data Access. The frontend is implemented as a modular Angular SPA.

## 🛠️ Tech Stack

**Backend:**
C# 14, .NET 10, ASP.NET Core Web API, Entity Framework Core, Microsoft SQL Server, ASP.NET Identity, JWT Authentication, OpenAI API

**Frontend:**
Angular 20, TypeScript, RxJS, Bootstrap 5, ngx-toastr

## ⭐ What Makes It Different?

### 1. Dynamic Product Catalog

New product types and characteristics can be introduced through the administration panel without requiring changes to the application code.

### 2. AI-Assisted Configuration

Users can describe the computer they want in natural language and use the AI assistant to create or modify a configuration.

### 3. Server-Side Compatibility Validation

The configurator validates component compatibility on the server and prevents invalid configurations from being ordered.

## 🔐 Security

Authentication and authorization are implemented using **ASP.NET Identity and JWT**. Access to resources is controlled through roles and server-side ownership checks.

## 📁 Project Structure

```text
HighTech/
├── Backend/
│   ├── Controllers/
│   ├── Services/
│   ├── Models/
│   ├── Data/
│   └── Migrations/
└── Frontend/
    ├── Overview/
    ├── Configurator/
    ├── Chatbot/
    ├── Manage/
    └── ApiAuthorization/
```

## 🎓 About

Developed as a Software Engineering diploma project.
