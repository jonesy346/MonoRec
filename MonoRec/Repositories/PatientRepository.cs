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
        // IEnumerable<Patient> patients; (look at this for testing)

        public PatientRepository(MonoRecDbContext db)
        {
            _db = db;
            // patients = db.Patients; (look at this for testing)
        }

        public IEnumerable<Patient> GetAllPatients()
        {
            var patients =_db.Patients.ToList();
            return patients;
        }

        public Patient? GetPatient(int patId)
        {
            var patient = _db.Patients.FirstOrDefault(patient => patient.PatientId == patId);
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
            var patient = _db.Patients.FirstOrDefault(patient => patient.PatientId == patId);

            if (patient == null) return null;

            var doctors = _db.Doctors.ToList();
            var doctorsPatients = _db.DoctorsPatients.ToList();

            var innerJoin = from doctor in doctors
                            join doctorPatient in doctorsPatients
                            on doctor.DoctorId equals doctorPatient.DoctorId
                            where doctorPatient.PatientId == patId
                            select doctor;

            return innerJoin.ToList();
            
            // create conditional to check if patient is in database, if not throw error - still works, but change response to error anyways
            // if no doctors are available for patient, then this currently returns empty list (which I think is fine to keep)
        }

        public Doctor AddNewDoctorForPatient(int patId, int docId)
        {
            var doctorToAdd = _db.Doctors.FirstOrDefault(doctor => doctor.DoctorId == docId);
            var patientToModify = _db.Patients.FirstOrDefault(patient => patient.PatientId == patId);

            if (doctorToAdd == null || patientToModify == null) return null;

            var newDoctorPatient = new DoctorPatient(docId, patId);
            _db.DoctorsPatients.Add(newDoctorPatient);
            _db.SaveChanges();

            return doctorToAdd;

            // create conditional to check if patient is in database, if not throw error
        }

        public Doctor DeleteDoctorForPatient(int patId, int docId)
        {
            var doctorToDelete = _db.Doctors.FirstOrDefault(doctor => doctor.DoctorId == docId);
            var patientToModify = _db.Patients.FirstOrDefault(patient => patient.PatientId == patId);

            if (doctorToDelete == null || patientToModify == null) return null;

            var doctorPatientToDelete = _db.DoctorsPatients.Where(doctorPatient => doctorPatient.DoctorId == docId && doctorPatient.PatientId == patId).First();
            _db.DoctorsPatients.Remove(doctorPatientToDelete);
            _db.SaveChanges();

            return doctorToDelete;

            // create conditional to check if patient is in database, if not throw error
        }
    }
}

