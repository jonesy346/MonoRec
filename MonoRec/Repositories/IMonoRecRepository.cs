using MonoRec.Models;
using System.Security.Claims;

namespace MonoRec.Repositories
{
    public interface IMonoRecRepository
    {
        IEnumerable<Patient> GetAllPatients();
        Patient CreateNewPatient(string name, int age);
    }
}