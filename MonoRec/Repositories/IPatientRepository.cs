using MonoRec.Models;
using System.Security.Claims;

namespace MonoRec.Repositories
{
    public interface IPatientRepository
    {
        IEnumerable<Patient> GetAllPatients();
        Patient? GetPatient(int patId);
        Patient CreateNewPatient(string name);
        IEnumerable<Doctor>? GetAllDoctorsByPatient(int patId);
        Doctor? AddNewDoctorForPatient(int patId, int docId);
        Doctor? DeleteDoctorForPatient(int patId, int docId);
    }
}