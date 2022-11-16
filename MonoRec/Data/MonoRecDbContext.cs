using Microsoft.EntityFrameworkCore;
using MonoRec.Models;

namespace MonoRec.Data;

public class MonoRecDbContext : DbContext
{

    // this first constructor was solely needed for the GetAll method in unit testing
    public MonoRecDbContext()
    {
    }

    public MonoRecDbContext(DbContextOptions<MonoRecDbContext> options) : base(options)
    {

    }
       
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<DoctorPatient> DoctorsPatients { get; set; }
    public DbSet<Visit> Visits { get; set; }
    public DbSet<DoctorNote> DoctorNotes { get; set; }
}

