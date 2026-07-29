# 📚 Library Management System

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![GitHub](https://img.shields.io/badge/GitHub-100000?style=for-the-badge&logo=github&logoColor=white)
![MIT License](https://img.shields.io/badge/MIT%20License-green?style=for-the-badge)

<!-- Project Banner Placeholder -->
<div align="center">
  <img src="https://via.placeholder.com/1200x400/8E6A6A/FFFFFF?text=Library+Management+System" alt="Project Banner" />
</div>

---

## 📖 Table of Contents

- [📚 Project Overview](#-project-overview)
- [✨ Features](#-features)
- [🛠️ Technology Stack](#️-technology-stack)
- [📁 Project Structure](#-project-structure)
- [🚀 Installation & Setup](#-installation--setup)
- [▶️ How to Run](#️-how-to-run)
- [📸 Screenshots](#-screenshots)
- [🔮 Future Enhancements](#-future-enhancements)
- [🤝 Contributing](#-contributing)
- [📄 License](#-license)
- [📞 Contact](#-contact)

---

## 📚 Project Overview

The **Library Management System** is a comprehensive web-based application built with ASP.NET Core MVC that streamlines library operations. This system enables librarians to efficiently manage books, magazines, newspapers, and member records while tracking book issues, returns, and fine calculations. The application features a modern, responsive UI with a user-friendly dashboard that provides real-time statistics and insights into library operations.

Designed for educational institutions, public libraries, and corporate libraries, this system simplifies day-to-day library management tasks, ensuring accurate record-keeping and seamless user experience.

---

## ✨ Features

### 🔐 User Authentication
- Secure login system with session management
- Role-based access control
- Password hashing for enhanced security

### 📊 Dashboard
- Real-time statistics overview
- Visual cards displaying total books, magazines, newspapers, members
- Quick access to due today and overdue book information
- Responsive design with modern UI

### 📖 Book Management
- Complete CRUD operations for books
- Search functionality by title, author, ISBN, category
- Filter by category and availability status
- Track available copies and total quantity
- ISBN validation and duplicate prevention

### 👥 Member Management
- Add, edit, and delete member records
- Search members by name, email, or phone
- Track member registration dates
- Email and phone validation
- Active borrowing limit enforcement

### 📰 Magazine Management
- Comprehensive magazine catalog management
- Track publisher, issue date, language, and category
- Search and filter by publisher or status
- Status tracking (Available/Issued)
- Detailed magazine descriptions

### 🗞️ Newspaper Management
- Complete newspaper inventory system
- Track publisher, published date, language, and edition
- Advanced search and filtering capabilities
- Status management (Available/Issued)
- Edition tracking for different newspaper variants

### 📤 Issue & Return Books
- Issue books to members with due date calculation
- Automatic fine calculation for overdue returns
- Track borrowing history
- Member borrowing limit enforcement (max 3 books)
- Real-time availability updates

### 📋 Reports
- Generate Excel reports for books and members
- Export data for analysis and record-keeping
- Customizable report formats

### ⚙️ Settings
- Database backup functionality
- Database restore capability
- System configuration options

---

## 🛠️ Technology Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| **C#** | .NET 8.0 | Backend logic and business operations |
| **ASP.NET Core MVC** | 8.0 | Web framework for building the application |
| **Entity Framework Core** | 8.0 | ORM for database operations |
| **SQLite** | 3.x | Database for data persistence |
| **HTML5** | - | Frontend structure |
| **CSS3** | - | Styling and responsive design |
| **Bootstrap 5** | 5.3 | UI framework for responsive components |
| **JavaScript** | ES6+ | Client-side interactivity |
| **System.Data.SQLite** | Latest | SQLite database driver |

---

## 📁 Project Structure

```
LibraryManagementSystem.Web/
├── Controllers/
│   ├── AccountController.cs          # Authentication & authorization
│   ├── BooksController.cs           # Book management
│   ├── DashboardController.cs       # Main dashboard
│   ├── MagazinesController.cs       # Magazine management
│   ├── MembersController.cs         # Member management
│   ├── NewspapersController.cs      # Newspaper management
│   ├── IssueController.cs           # Book issue operations
│   ├── ReturnController.cs          # Book return operations
│   ├── ReportsController.cs         # Report generation
│   └── SettingsController.cs        # System settings
├── DataAccess/
│   └── DatabaseHelper.cs            # Database initialization & operations
├── Models/
│   └── Entities.cs                  # Data models (Book, Member, Magazine, Newspaper, etc.)
├── Services/
│   └── AppServices.cs               # Business logic services
├── Views/
│   ├── Shared/
│   │   └── _Layout.cshtml           # Main layout template
│   ├── Dashboard/
│   │   └── Index.cshtml             # Dashboard view
│   ├── Books/
│   │   └── Index.cshtml             # Books management view
│   ├── Magazines/
│   │   └── Index.cshtml             # Magazines management view
│   ├── Newspapers/
│   │   └── Index.cshtml             # Newspapers management view
│   ├── Members/
│   │   └── Index.cshtml             # Members management view
│   ├── Issue/
│   │   └── Index.cshtml             # Issue book view
│   ├── Return/
│   │   └── Index.cshtml             # Return book view
│   ├── Reports/
│   │   └── Index.cshtml             # Reports view
│   ├── Settings/
│   │   └── Index.cshtml             # Settings view
│   └── Account/
│       └── Login.cshtml             # Login page
├── wwwroot/
│   └── css/
│       └── site.css                 # Custom styles
├── Program.cs                       # Application entry point
├── appsettings.json                 # Configuration settings
└── LibraryManagementSystem.Web.csproj # Project file
```

---

## 🚀 Installation & Setup

### Prerequisites

- **.NET 8.0 SDK** - Download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Visual Studio 2022** or **Visual Studio Code** (recommended)
- **Git** (for cloning the repository)

### Installation Steps

1. **Clone the Repository**
   ```bash
   git clone https://github.com/Swastikad4/LibraryManagementSystem.git
   cd LibraryManagementSystem
   ```

2. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the Project**
   ```bash
   dotnet build
   ```

4. **Database Setup**
   - The SQLite database (`library.db`) is automatically created on first run
   - Seed data is automatically populated including:
     - Default admin user (username: `admin`, password: `admin123`)
     - Sample books, magazines, newspapers, and members

---

## ▶️ How to Run

### Using Command Line

1. **Navigate to the Project Directory**
   ```bash
   cd LibraryManagementSystem.Web
   ```

2. **Run the Application**
   ```bash
   dotnet run
   ```

3. **Access the Application**
   - Open your browser and navigate to: `http://localhost:5000`
   - Login with default credentials:
     - **Username:** `admin`
     - **Password:** `admin123`

### Using Visual Studio

1. **Open the Project**
   - Open `LibraryManagementSystem.Web.csproj` in Visual Studio

2. **Build and Run**
   - Press `F5` or click the "Run" button in Visual Studio
   - The application will launch in your default browser

3. **Login**
   - Use the default credentials mentioned above

---

## 🔮 Future Enhancements

- [ ] **Barcode/QR Code Integration** - Scan books for quick checkout
- [ ] **Email Notifications** - Automatic due date reminders to members
- [ ] **Multi-language Support** - Support for multiple languages
- [ ] **Mobile Application** - Native mobile app for members
- [ ] **Advanced Analytics** - Detailed usage statistics and trends
- [ ] **Book Reservation System** - Allow members to reserve books
- [ ] **Fine Payment Gateway** - Online fine payment integration
- [ ] **API Development** - RESTful API for third-party integrations
- [ ] **Cloud Database Support** - Migration to SQL Server or PostgreSQL
- [ ] **Role-based Access Control** - Different user roles (Librarian, Admin, Member)

---

## 🤝 Contributing

Contributions are welcome! If you'd like to contribute to this project, please follow these steps:

1. **Fork the Repository**
   - Click the "Fork" button on the GitHub repository

2. **Create a Branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

3. **Make Your Changes**
   - Implement your feature or bug fix
   - Ensure code follows the existing style
   - Add comments where necessary

4. **Commit Your Changes**
   ```bash
   git commit -m "Add your commit message here"
   ```

5. **Push to Your Branch**
   ```bash
   git push origin feature/your-feature-name
   ```

6. **Submit a Pull Request**
   - Create a pull request with a clear description of your changes

---

## 📄 License

This project is licensed under the **MIT License**.

```
MIT License

Copyright (c) 2024 Swastika Dey

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

---



**Swastika Dey**

- 🌐 **GitHub:** [https://github.com/Swastikad4](https://github.com/Swastikad4)


---

<div align="center">
  
</div>
