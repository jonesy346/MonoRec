using System.IO;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

    [HttpGet("me")]
    [Authorize(AuthenticationSchemes = "Identity.Application,IdentityServerJwt", Roles = "Doctor")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Doctor))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetCurrentDoctor()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var doctor = _monoRecRepository.GetAllDoctors().FirstOrDefault(d => d.UserId == userId);
        if (doctor == null) return NotFound();

        return Ok(doctor);
    }

    /// <summary>
    /// Gets all doctors. Requires authentication but no specific role.
    ///
    /// Security note: This endpoint is accessible to all authenticated users (both patients and doctors).
    /// Patients use this to fetch the list of doctors, filter by their userId to find their doctor entity,
    /// and then retrieve their associated doctors via the patient/{patId}/doctor endpoint.
    /// This design allows patients to find their own doctor without exposing sensitive patient data.
    /// </summary>
    [HttpGet]
    [Authorize(AuthenticationSchemes = "Identity.Application,IdentityServerJwt")]
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
    [Authorize(AuthenticationSchemes = "Identity.Application,IdentityServerJwt")]
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

