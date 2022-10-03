using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonoRec.Models;
using MonoRec.Repositories;

namespace MonoRec.Controllers;

[Route("[controller]")]
[ApiController]
public class PatientController : ControllerBase
{
    // GET: 
    private IPatientRepository _monoRecRepository;

    public PatientController(IPatientRepository monoRecRepository)
    {
        _monoRecRepository = monoRecRepository;
    }

    [HttpGet]
    public IEnumerable<Patient> GetAllPatients()
    {
        return _monoRecRepository.GetAllPatients();
    }

    [HttpGet("{patId}")]
    public Patient GetPatient(int patId)
    {
        return _monoRecRepository.GetPatient(patId);
    }

    [HttpPost("{name}")]
    public Patient CreateNewPatient(string name)
    {
        return _monoRecRepository.CreateNewPatient(name);
    }

    [HttpGet("{patId}/doctor")]
    public IEnumerable<Doctor> GetAllDoctorsByPatient(int patId)
    {
        return _monoRecRepository.GetAllDoctorsByPatient(patId);

    }

    [HttpPost("{patId}/doctor/{docId}")]
    public Doctor AddNewDoctorForPatient(int patId, int docId)
    {
        return _monoRecRepository.AddNewDoctorForPatient(patId, docId);
    }

    [HttpDelete("{patId}/doctor/{docId}")]
    public Doctor DeleteDoctorForPatient(int patId, int docId)
    {
        return _monoRecRepository.DeleteDoctorForPatient(patId, docId);
    }

}

