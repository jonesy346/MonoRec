using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonoRec.Models;
using MonoRec.Repositories;

namespace MonoRec.Controllers;

[Route("api/[controller]")]
[ApiController]
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
        Console.WriteLine("get method function ran");
        var controllerTest = _monoRecRepository.GetAllPatients();
        return controllerTest;
    }

    [HttpPost]
    public Patient CreateNewPatient(string name)
    {
        return _monoRecRepository.CreateNewPatient(name);
    }

}
