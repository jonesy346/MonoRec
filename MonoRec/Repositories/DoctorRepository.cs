using MonoRec.Data;
using MonoRec.Models;
using MonoRec.Repositories;

public class DoctorRepository : IDoctorRepository
{
    MonoRecDbContext _db;

    public DoctorRepository(MonoRecDbContext db)
    {
        _db = db;
    }

    public IEnumerable<Doctor> GetAllDoctors()
    {
        var test = _db.Doctors.ToList();
        return test;
    }

    public Doctor CreateNewDoctor(string name)
    {
        var newDoctor = new Doctor(name);
        _db.Doctors.Add(newDoctor);
        _db.SaveChanges();
        return newDoctor;
    }
}