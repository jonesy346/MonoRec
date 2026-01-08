# MonoRec

A medical records management system built with ASP.NET Core 6.0 and React. MonoRec allows doctors and patients to manage medical visits, doctor notes, and patient-doctor relationships.

## Architecture at a Glance

**Backend:**
- **Controllers** - RESTful API endpoints for Patients, Doctors, and Visits
- **Models** - Entity classes (Doctor, Patient, Visit, DoctorNote) with Identity integration
- **Repositories** - Data access layer using the repository pattern
- **Entity Framework Core** - ORM with SQLite (local dev) and SQL Server (production) support
- **ASP.NET Identity** - Authentication and authorization with role-based access (Doctor, Patient)

**Frontend:**
- **ClientApp** - React SPA with Bootstrap for UI
- **React Router** - Client-side routing
- **SpaProxy** - Development server with hot reload

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
- **React Dev Server**: `http://localhost:3000` (auto-launched)
- **Access the app**: Navigate to `http://localhost:5032` in your browser

### API Endpoints

- `GET /patient` - Get all patients
- `GET /patient/{id}` - Get patient by ID
- `POST /patient/{name}` - Create new patient
- `GET /doctor` - Get all doctors
- `GET /doctor/{id}` - Get doctor by ID
- `POST /doctor/{name}` - Create new doctor
- `GET /visit` - Get all visits
- `GET /visit/{id}` - Get visit by ID

## Testing

Run unit tests:
```bash
dotnet test
```

Unit tests are located in the `MonoRec.UnitTests` project and cover controller logic.

## Seeded Accounts

On first run, the following accounts are created:

### Users (for login)
- **Doctor**: `doctor@example.com` / `Password123!` (Doctor role)
- **Patient**: `patient@example.com` / `Password123!` (Patient role)
- **Test User**: `test@test.com` / `Welcome123!` (No role)

### Sample Data (linked to users)
- **Doctor Entity**: "Dr. Example" (linked to doctor@example.com)
- **Patient Entity**: "Patient Example" (linked to patient@example.com)

The `UserId` field in Doctor and Patient tables stores the link to the Identity user account.

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
- **Email Confirmation**: Email confirmation is bypassed for seeded users (`EmailConfirmed = true`). Production should implement proper email verification.
- **User-Entity Linking**: The `UserId` field provides a conceptual link between Identity users and business entities, but there's no foreign key constraint enforced at the database level.
- **Role-Based UI**: The UI does not currently restrict features based on user roles (Doctor vs Patient). Authentication is configured but authorization is not fully implemented in the frontend.
- **No User Registration**: User registration through the UI is not implemented. Users must be seeded or created manually.

## License

[Your License Here]

## Contributing

[Your Contributing Guidelines Here]