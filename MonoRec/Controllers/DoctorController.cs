using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonoRec.Models;
using MonoRec.Repositories;

namespace MonoRec.Controllers;

[Route("[controller]")]
[ApiController]
public class DoctorController : ControllerBase
{
    // GET: 
    private IDoctorRepository _monoRecRepository;

    public DoctorController(IDoctorRepository monoRecRepository)
    {
        _monoRecRepository = monoRecRepository;
    }

    [HttpGet]
    public IEnumerable<Doctor> GetAllDoctors()
    {
        return _monoRecRepository.GetAllDoctors();
    }

    [HttpGet("{docId}")]
    public Doctor GetDoctor(int docId)
    {
        return _monoRecRepository.GetDoctor(docId);
    }


    [HttpPost("{name}")]
    public Doctor CreateNewDoctor(string name)
    {
        return _monoRecRepository.CreateNewDoctor(name);
    }

    [HttpGet("{docId}/patient")]
    public IEnumerable<Patient> GetAllPatientsByDoctor(int docId)
    {
        return _monoRecRepository.GetAllPatientsByDoctor(docId);

    }

    [HttpPost("{docId}/patient/{patId}")]
    public Patient AddNewPatientForDoctor(int docId, int patId)
    {
        return _monoRecRepository.AddNewPatientForDoctor(docId, patId);
    }

    [HttpDelete("{docId}/patient/{patId}")]
    public Patient DeletePatientForDoctor(int docId, int patId)
    {
        return _monoRecRepository.DeletePatientForDoctor(docId, patId);
    }
}

