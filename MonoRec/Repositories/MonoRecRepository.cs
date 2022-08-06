using System;
using MonoRec.Data;
using MonoRec.Models;
using System.Security.Claims;

namespace MonoRec.Repositories
{
    public class MonoRecRepository : IMonoRecRepository
    {
        MonoRecDbContext _db;

        public MonoRecRepository(MonoRecDbContext db)
        {
            _db = db;
        }

        public IEnumerable<Patient> GetAllPatients()
        {
            return _db.Patients.ToList();
        }

        public Patient CreateNewPatient(string name, int age)
        {
            var newPatient = new Patient(name, age);
            _db.Patients.Add(newPatient);
            _db.SaveChanges();
            return newPatient;
        }
    }
}