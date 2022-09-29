using MonoRec.Models;
using System.Security.Claims;

namespace MonoRec.Repositories
{
    public interface IPatientRepository
    {
        IEnumerable<Patient> GetAllPatients();
        Patient CreateNewPatient(string name);
    }
}