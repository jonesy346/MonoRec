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
   - Tokens contain user claims including roles
   - Managed by `authService` in React components

This hybrid approach allows:
- Traditional login/register forms via Razor Pages
- Modern SPA authentication via JWT tokens
- Seamless integration between server-rendered auth pages and the React frontend

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
- `GET /patient` - Get all patients *(Doctor only)*
- `GET /patient/{id}` - Get patient by ID
- `POST /patient/{name}` - Create new patient
- `GET /patient/{patientId}/doctors` - Get doctors affiliated with patient *(Patient and Doctor)*

#### Doctor Endpoints
- `GET /doctor` - Get all doctors *(Patient only)*
- `GET /doctor/{id}` - Get doctor by ID
- `POST /doctor/{name}` - Create new doctor
- `GET /doctor/{doctorId}/patients` - Get patients affiliated with doctor *(Doctor only)*

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

### Role-Based Permissions

#### Doctor Role
Doctors have elevated privileges:
- ✅ View all patients (`GET /patient`)
- ✅ View patients affiliated with them (`GET /doctor/{doctorId}/patients`)
- ✅ Create new visits (`POST /visit`)
- ✅ Delete visits (`DELETE /visit/{id}`)
- ✅ View visit details (`GET /visit/{id}`)
- ❌ Cannot view list of all doctors

#### Patient Role
Patients have restricted access:
- ✅ View all doctors (`GET /doctor`) - to find and select doctors
- ✅ View doctors affiliated with them (`GET /patient/{patientId}/doctors`)
- ✅ View their own visit details (`GET /visit/{id}`)
- ❌ Cannot view other patients
- ❌ Cannot create or delete visits
- ❌ Cannot view all patients

### Auth Health Check
Navigate to `/auth-health-check` to view:
- Current authentication status
- User ID
- Email address
- Full name
- Assigned role(s)

This is useful for verifying your login status and understanding what permissions you have.

## Seeded Accounts

On first run, the following test accounts are created:

- **Doctor**: `doctor@example.com` / `Password123!` (Doctor role)
- **Patient**: `patient@example.com` / `Password123!` (Patient role)
- **Test User**: `test@test.com` / `Welcome123!` (No role)

Corresponding Doctor and Patient entities are automatically created and linked via `UserId`.

## Development

### Hot Reload
The React dev server supports hot reload. Edit any file in `ClientApp/src/` and the browser will automatically update.

### Database Migrations
When you modify models, create a new migration:
```bash
# For MonoRec entities (Doctor, Patient, Visit)
dotnet ef migrations add YourMigrationName --context MonoRecDbContext

# For Identity entities (Users, Roles)
dotnet ef migrations add YourMigrationName --context ApplicationDbContext
```

Apply migrations:
```bash
dotnet ef database update --context MonoRecDbContext
dotnet ef database update --context ApplicationDbContext
```

### Reset Database
To start fresh with seeded data:
```bash
rm MonoRec/app.db
dotnet run
```

### Production Build
Build the React app for production:
```bash
cd ClientApp
npm run build
```

The built files will be copied to `wwwroot/` during `dotnet publish`.

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

### Database
- **Local Development**: SQLite (`app.db` in project root)
- **Production**: SQL Server (connection strings in `appsettings.json`)

To switch to SQL Server, uncomment the AWS configuration in `Program.cs` and `appsettings.json`.

## Known Limitations

- **HTTPS Certificate**: HTTPS is disabled in development due to certificate issues on macOS. The app runs on HTTP only for local development.
- **Email Confirmation**: Email confirmation is disabled (`RequireConfirmedAccount = false`). Production should implement proper email verification.
- **Email Validation**: Only basic email format validation is enforced (`username@domain.extension`). The email doesn't need to be valid or deliverable.
- **Password Requirements**: Disabled for development convenience (minimum 1 character). Production should enforce strong password policies.
- **User-Entity Linking**: The `UserId` field provides a conceptual link between Identity users and business entities, but there's no foreign key constraint enforced at the database level.
- **Frontend Authorization**: While backend endpoints are protected with role-based authorization, the React frontend doesn't dynamically hide/show UI elements based on roles. Users may see UI elements for actions they cannot perform (the backend will reject unauthorized requests).
- **Cross-Port Authentication**: The `/api/users/me` endpoint doesn't work when accessed directly in the browser due to cookie authentication not crossing the proxy boundary reliably. Use the Auth Health Check page instead, which properly uses JWT tokens.

## License

[Your License Here]

## Contributing

[Your Contributing Guidelines Here]