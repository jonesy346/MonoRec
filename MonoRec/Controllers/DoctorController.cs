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
        Console.WriteLine("get method function ran");
        var controllerTest = _monoRecRepository.GetAllDoctors();
        return controllerTest;
    }

    [HttpPost("{name}")]
    public Doctor CreateNewDoctor(string name)
    {
        return _monoRecRepository.CreateNewDoctor(name);
    }

}

