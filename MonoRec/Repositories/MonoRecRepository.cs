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
            var test =_db.Patients.ToList();
            return test;
        }

        public Patient CreateNewPatient(string name)
        {
            var newPatient = new Patient(name);
            _db.Patients.Add(newPatient);
            _db.SaveChanges();
            return newPatient;
        }
    }
}