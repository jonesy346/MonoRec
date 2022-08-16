using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonoRec.Models;
using MonoRec.Repositories;

namespace MonoRec.Controllers;

[ApiController]
[Route("[controller]")]
public class PatientController : ControllerBase
{
    // GET: 
    private IMonoRecRepository _monoRecRepository;

    public PatientController(IMonoRecRepository monoRecRepository)
    {
        _monoRecRepository = monoRecRepository;
    }

    [HttpGet]
    public IEnumerable<Patient> GetAllPatients()
    {
        return _monoRecRepository.GetAllPatients();
    }

    [HttpPost]
    public Patient CreateNewPatient(string name, int age)
    {
        return _monoRecRepository.CreateNewPatient(name, age);
    }

}
