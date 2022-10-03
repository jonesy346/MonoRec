using Microsoft.EntityFrameworkCore;
using MonoRec.Models;

namespace MonoRec.Data;

public class MonoRecDbContext : DbContext
{
    public MonoRecDbContext(DbContextOptions<MonoRecDbContext> options) : base(options)
    {

    }
       
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<DoctorPatient> DoctorsPatients { get; set; }
    public DbSet<Visit> Visits { get; set; }
    public DbSet<DoctorNote> DoctorNotes { get; set; }
}

