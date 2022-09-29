using MonoRec.Models;
using System.Security.Claims;

namespace MonoRec.Repositories
{
    public interface IVisitRepository
    {
        IEnumerable<Visit> GetAllVisits();
        Visit CreateNewVisit(int PatientId, int DoctorId);
        IEnumerable<Visit> GetAllVisitsByPatient(int PatientId);
        IEnumerable<Visit> GetAllVisitsByDoctor(int PatientId);
        Visit DeleteVisit(int VisitId);
	}
}