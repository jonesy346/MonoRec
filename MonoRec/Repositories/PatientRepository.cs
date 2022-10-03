using System;
using MonoRec.Data;
using MonoRec.Models;
using System.Security.Claims;
using System.Security.Cryptography;

namespace MonoRec.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        MonoRecDbContext _db;

        public PatientRepository(MonoRecDbContext db)
        {
            _db = db;
        }

        public IEnumerable<Patient> GetAllPatients()
        {
            var patients =_db.Patients.ToList();
            return patients;
        }

        public Patient GetPatient(int patId)
        {
            var patient = _db.Patients.Where(patient => patient.PatientId == patId).First();
            return patient;

            // create conditional to check if patient is in database, if not throw error
        }

        public Patient CreateNewPatient(string name)
        {
            var newPatient = new Patient(name);
            _db.Patients.Add(newPatient);
            _db.SaveChanges();
            return newPatient;
        }

        public IEnumerable<Doctor> GetAllDoctorsByPatient(int patId)
        {
            var doctors = _db.Doctors.ToList();
            var doctorsPatients = _db.DoctorsPatients.ToList();

            var innerJoin = from doctor in doctors
                            join doctorPatient in doctorsPatients
                            on doctor.DoctorId equals doctorPatient.DoctorId
                            where doctorPatient.PatientId == patId
                            select doctor;

            return innerJoin.ToList();

        }

        public Doctor AddNewDoctorForPatient(int patId, int docId)
        {
            var newDoctorPatient = new DoctorPatient(docId, patId);
            _db.DoctorsPatients.Add(newDoctorPatient);
            _db.SaveChanges();

            var doctorToAdd = _db.Doctors.Where(doctor => doctor.DoctorId == docId).First();
            return doctorToAdd;
        }

        public Doctor DeleteDoctorForPatient(int patId, int docId)
        {
            var doctorPatientToDelete = _db.DoctorsPatients.Where(doctorPatient => doctorPatient.DoctorId == docId && doctorPatient.PatientId == patId).First();
            _db.DoctorsPatients.Remove(doctorPatientToDelete);
            _db.SaveChanges();

            var doctorToDelete = _db.Doctors.Where(doctor => doctor.DoctorId == docId).First();
            return doctorToDelete;

        }
    }
}

