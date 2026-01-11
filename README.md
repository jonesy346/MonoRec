# MonoRec

A medical records management system built with ASP.NET Core 6.0 and React. MonoRec allows doctors and patients to manage medical visits, doctor notes, and patient-doctor relationships.

## Architecture at a Glance

**Backend:**
- **Controllers** - RESTful API endpoints for Patients, Doctors, and Visits
- **Models** - Entity classes (Doctor, Patient, Visit, DoctorNote) with Identity integration
- **Repositories** - Data access layer using the repository pattern
- **Entity Framework Core** - ORM with SQLite (local dev) and SQL Server (production) support
- **ASP.NET Identity + IdentityServer** - Dual authentication system with role-based authorization

**Frontend:**
- **ClientApp** - React SPA with Bootstrap for UI
- **React Router** - Client-side routing
- **SpaProxy** - Development server with hot reload

### Authentication Architecture

MonoRec uses a dual authentication system:

1. **Cookie Authentication** (ASP.NET Core Identity)
   - Used for Razor Pages (login/register UI at `/Identity/Account/Login`)
   - Creates `.AspNetCore.Identity.Application` cookie
   - Works seamlessly for server-side rendered pages

2. **JWT Token Authentication** (IdentityServer4)
   - Used by the React SPA for API calls
   - Tokens contain user claims including roles, name, and email
   - Custom `ProfileService` enriches JWT tokens with user data from the database
   - Managed by `authService` in React components

This hybrid approach allows:
- Traditional login/register forms via Razor Pages
- Modern SPA authentication via JWT tokens
- Seamless integration between server-rendered auth pages and the React frontend
- Custom user properties (Name) included in JWT tokens via ProfileService

## Quickstart

### Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Node.js 14+](https://nodejs.org/) (for React development)
- SQLite (included with .NET)

### Installation & Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd MonoRec
   ```

2. **Restore dependencies**
   ```bash
   cd MonoRec
   dotnet restore
   cd ClientApp && npm install && cd ..
   ```

3. **Run database migrations**
   ```bash
   dotnet ef database update --context MonoRecDbContext
   dotnet ef database update --context ApplicationDbContext
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

   The app will:
   - Start the backend on `http://localhost:5032`
   - Automatically launch the React dev server on `http://localhost:3000`
   - Seed the database with test data on first run
   - Open your browser automatically

### Expected URLs

- **Backend API**: `http://localhost:5032`
- **React Dev Server**: `http://localhost:3000` (auto-launched, recommended)
- **Auth Health Check**: `http://localhost:3000/auth-health-check` - View current user info and roles

### API Endpoints

#### Patient Endpoints
- `GET /patient` - Get all patients *(Doctor only)* or filtered patient with `?currentUserOnly=true` *(Authenticated users)*
- `GET /patient/me` - Get current patient entity *(Patient only)*
- `GET /patient/{id}` - Get patient by ID
- `POST /patient/{name}` - Create new patient
- `GET /patient/{patientId}/doctor` - Get doctors affiliated with patient *(Patient and Doctor)*

#### Doctor Endpoints
- `GET /doctor` - Get all doctors *(Authenticated users)*
- `GET /doctor/me` - Get current doctor entity *(Doctor only)*
- `GET /doctor/{id}` - Get doctor by ID
- `POST /doctor/{name}` - Create new doctor
- `GET /doctor/{doctorId}/patient` - Get patients affiliated with doctor *(Authenticated users)*

#### Visit Endpoints
- `GET /visit` - Get all visits
- `GET /visit/{id}` - Get visit by ID *(Patient and Doctor)*
- `POST /visit` - Create new visit *(Doctor only)*
- `DELETE /visit/{id}` - Delete visit *(Doctor only)*

#### User Endpoints
- `GET /api/users/me` - Get current authenticated user info (returns userId, email, name, roles)

## Testing

Run unit tests:
```bash
dotnet test
```

Unit tests are located in the `MonoRec.UnitTests` project and cover controller logic.

## User Registration & Roles

### Registration
Users can register at `/Identity/Account/Register` with:
- **Name** - Full name
- **Email** - Must be in format `username@domain.extension` (e.g., `user@gmail.com`)
- **Password** - Minimum 1 character (no complexity requirements for development)
- **Role** - Choose between Patient or Doctor

During registration:
- A corresponding Patient or Doctor entity is automatically created in the database
- The entity is linked to the Identity user via the `UserId` field
- Role-based permissions are immediately active

### Role-Based Permissions & Security

MonoRec implements role-based authorization with secure data filtering to protect sensitive patient information.

#### Doctor Role
Doctors have full management privileges for patient care:

- ✅ View all patients and doctors
- ✅ View affiliated patients
- ✅ Create, edit, and delete visits
- ✅ Add and modify doctor notes
- ✅ Full access to visit history and details

#### Patient Role
Patients have view-only access with restricted data visibility:

- ✅ View all doctors and their affiliated doctors
- ✅ View only their own patient record
- ✅ **Read-only** access to their own visit history and details
- ❌ Cannot view other patients' data
- ❌ Cannot create, edit, or delete visits

#### Security Implementation

- **Query Parameter Filtering** - `GET /patient?currentUserOnly=true` filters server-side by user ID, preventing patients from accessing other patients' data
- **Role-Based Access Control** - Doctor role required for full patient list access
- **"sub" Claim** - Consistent user identification across cookie and JWT authentication
- **Dual Authentication** - All endpoints accept both `Identity.Application` (cookies) and `IdentityServerJwt` (JWT tokens)

### Auth Health Check
Navigate to `/auth-health-check` to view your authentication status, user ID, email, name, and assigned roles.

## Seeded Accounts & Test Data

On first run, the database is seeded with:

**User Accounts:**
- **Doctor**: `doctor@example.com` / `Password123!`
- **Patient**: `patient@example.com` / `Password123!`
- **Test User**: `test@test.com` / `Welcome123!` (No role)

**Entities:**
- 10 Doctors (Dr. Smith, Dr. Johnson, Dr. Williams, etc.)
- 10 Patients (Alice, Bob, Charlie, Diana, etc.)
- Random doctor-patient relationships (each entity linked with 2-4 others)

**Auto-Linking:**
When a newly registered user logs in for the first time, they're automatically linked with 3 random seeded entities (doctors get patients, patients get doctors). This ensures immediate data to explore.

## Development

### Database Migrations
Create migrations when you modify models:
```bash
# For MonoRec entities (Doctor, Patient, Visit)
dotnet ef migrations add YourMigrationName --context MonoRecDbContext

# For Identity entities (Users, Roles)
dotnet ef migrations add YourMigrationName --context ApplicationDbContext

# Apply migrations
dotnet ef database update --context MonoRecDbContext
dotnet ef database update --context ApplicationDbContext
```

### Reset Database
```bash
rm MonoRec/app.db
dotnet run
```

### Production Build
```bash
cd ClientApp
npm run build
```

## Project Structure

```
MonoRec/
├── Controllers/          # API endpoints
├── Models/              # Entity classes
├── Repositories/        # Data access layer
├── Data/                # DbContexts and seeding
├── Migrations/          # EF Core migrations
├── ClientApp/           # React frontend
│   └── src/
│       ├── components/  # React components
│       └── setupProxy.js # API proxy configuration
└── MonoRec.UnitTests/   # Unit tests
```

## Configuration

**Database:**
- Local Development: SQLite (`app.db`)
- Production: SQL Server (uncomment AWS configuration in `Program.cs` and `appsettings.json`)

## Known Limitations

- **HTTPS**: Disabled in development (macOS certificate issues)
- **Email Confirmation**: Disabled (`RequireConfirmedAccount = false`) - production should implement proper email verification
- **Password Requirements**: Minimum 1 character for development - production should enforce strong policies
- **User-Entity Linking**: No foreign key constraint at database level between Identity users and business entities
- **Frontend Authorization**: UI doesn't dynamically hide/show elements based on roles (backend rejects unauthorized requests)
- **Cross-Port Authentication**: `/api/users/me` doesn't work in browser - use Auth Health Check page instead
