using System.IO;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
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
        var doctors = _db.Doctors.ToList();
        return doctors;
    }

    public Doctor GetDoctor(int docId)
    {
        var doctor = _db.Doctors.Where(doctor => doctor.DoctorId == docId).First();
        return doctor;

        // create conditional to check if patient is in database, if not throw error
    }

    public Doctor CreateNewDoctor(string name)
    {
        var newDoctor = new Doctor(name);
        _db.Doctors.Add(newDoctor);
        _db.SaveChanges();
        return newDoctor;
    }

    public IEnumerable<Patient> GetAllPatientsByDoctor(int docId)
    {
        var patients = _db.Patients.ToList();
        var doctorsPatients = _db.DoctorsPatients.ToList();

        var innerJoin = from patient in patients
                        join doctorPatient in doctorsPatients
                        on patient.PatientId equals doctorPatient.PatientId
                        where doctorPatient.DoctorId == docId
                        select patient;

        return innerJoin.ToList();
    }

    public Patient AddNewPatientForDoctor(int docId, int patId)
    {
        var newDoctorPatient = new DoctorPatient(docId, patId);
        _db.DoctorsPatients.Add(newDoctorPatient);
        _db.SaveChanges();
	    
	    var patientToAdd = _db.Patients.Where(patient => patient.PatientId == patId).First();
	    return patientToAdd;
    }

    public Patient DeletePatientForDoctor(int docId, int patId)
    {
        var doctorPatientToDelete = _db.DoctorsPatients.Where(doctorPatient => doctorPatient.DoctorId == docId && doctorPatient.PatientId == patId).First();
        _db.DoctorsPatients.Remove(doctorPatientToDelete);
        _db.SaveChanges();

        var patientToDelete = _db.Patients.Where(patient => patient.PatientId == patId).First();
        return patientToDelete;
    }

}