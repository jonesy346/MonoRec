using System.IO;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Patient")]
    public IEnumerable<Doctor> GetAllDoctors()
    {
        return _monoRecRepository.GetAllDoctors();
    }

    [HttpGet("{docId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Doctor))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetDoctor(int docId)
    {
        var doctor = _monoRecRepository.GetDoctor(docId);

        if (doctor == null) return NotFound();

        return Ok(doctor);
    }


    [HttpPost("{name}")]
    public Doctor CreateNewDoctor(string name)
    {
        return _monoRecRepository.CreateNewDoctor(name);
    }

    [HttpGet("{docId}/patient")]
    [Authorize(Roles = "Doctor")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Patient>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetAllPatientsByDoctor(int docId)
    {
        var result = _monoRecRepository.GetAllPatientsByDoctor(docId);

        if (result == null) return NotFound();

        return Ok(result);

    }

    [HttpPost("{docId}/patient/{patId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Patient))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult AddNewPatientForDoctor(int docId, int patId)
    {
        var result = _monoRecRepository.AddNewPatientForDoctor(docId, patId);

        if (result == null) return NotFound();

        return Ok(result);
    }

    [HttpDelete("{docId}/patient/{patId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Patient))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeletePatientForDoctor(int docId, int patId)
    {
        var result = _monoRecRepository.DeletePatientForDoctor(docId, patId);

        if (result == null) return NotFound();

        return Ok(result);
    }
}

