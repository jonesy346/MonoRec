using MonoRec.Models;
using System.Security.Claims;

namespace MonoRec.Repositories
{
    public interface IDoctorRepository
    {
        IEnumerable<Doctor> GetAllDoctors();
        Doctor CreateNewDoctor(string name);
    }
}