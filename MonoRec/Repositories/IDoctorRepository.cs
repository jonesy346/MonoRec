using MonoRec.Models;
using System.Security.Claims;

namespace MonoRec.Repositories
{
    public interface IDoctorRepository
    {
        IEnumerable<Doctor> GetAllDoctors();
        Doctor GetDoctor(int docId);
        Doctor CreateNewDoctor(string name);
        IEnumerable<Patient> GetAllPatientsByDoctor(int docId);
        Patient AddNewPatientForDoctor(int docId, int patId);
        Patient DeletePatientForDoctor(int docId, int patId);
    }
}