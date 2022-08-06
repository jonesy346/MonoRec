using Microsoft.EntityFrameworkCore;
using MonoRec.Models;

namespace MonoRec.Data;

public class MonoRecDbContext : DbContext
{
    public MonoRecDbContext(DbContextOptions<MonoRecDbContext> options) : base(options)
    {

    }

    //protected override void OnConfiguring(DbContextOptionsBuilder options)
    //        => options.UseSqlServer("Server=localhost,1433; Database=MonoRec;User=SA; Password=c8Phup&u");

    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Visit> Visits { get; set; }
    public DbSet<DoctorNote> DoctorNotes { get; set; }
}

