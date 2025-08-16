## 🏥 Medical Consultation Booking Website

A web application for scheduling medical consultations with role-based access control (administrator, doctor, patient), record management, and automated reporting.

---

## 🚀 Features
- Role-based system: **Administrator, Doctor, Patient**
- CRUD operations for managing medical appointments
- Automated **PDF report generation**
- Scheduled **database backups** for reliability

---

## 🛠 Tech Stack
- **Language:** C#
- **Backend:** ASP.NET Core Web API
- **Frontend:** HTML, CSS
- **Database:** PostgreSQL

---

## ⚙️ Installation & Usage

Clone the repository:

```bash
git clone https://github.com/oduvanchikm/websiteMedicalService.git
cd CourseWorkDataBase
```

Build and run the project (using .NET CLI):

```bash
dotnet build
dotnet run
```

## 📂 Project Structure
```bash
CourseWorkDataBase/
│── Backups/       # Database backup files  
│── Controllers/   # Controllers  
│── DAL/           # Data access layer (database connection configuration)  
│── Helpers/       # Utility methods (slot generation, PDF export, etc.)  
│── Migrations/    # Database migrations (EF Core)  
│── Models/        # Database models (entities)  
│── Pdfs/          # Generated PDF reports  
│── Services/      # Core business logic (backend services)  
│── ViewModels/    # View models for data transfer  
│── Views/         # Frontend (HTML, CSS templates)  
└── README.md      # Project overview
```

## 👤 Author
oduvanchikm


