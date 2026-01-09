using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(AuthenticationSchemes = "Identity.Application,IdentityServerJwt")]
    public IEnumerable<Patient> GetAllPatients()
    {
        var result = _monoRecRepository.GetAllPatients();
        return result;
    }

    [HttpGet("me")]
    [Authorize(AuthenticationSchemes = "Identity.Application,IdentityServerJwt", Roles = "Patient")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Patient))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetCurrentPatient()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var patient = _monoRecRepository.GetAllPatients().FirstOrDefault(p => p.UserId == userId);
        if (patient == null) return NotFound();

        return Ok(patient);
    }

    [HttpGet("{patId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Patient))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetPatient(int patId)
    {
        // add if statement for if this returns null, do this for all parameters

        var patient = _monoRecRepository.GetPatient(patId);

        if (patient == null) return NotFound();

        return Ok(patient);
    }

    [HttpPost("{name}")]
    public Patient CreateNewPatient(string name)
    {

        return _monoRecRepository.CreateNewPatient(name);

        // should post route return patient that was created?
    }

    [HttpGet("{patId}/doctor")]
    [Authorize(AuthenticationSchemes = "Identity.Application,IdentityServerJwt", Roles = "Patient,Doctor")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Doctor>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetAllDoctorsByPatient(int patId)
    {
        var result = _monoRecRepository.GetAllDoctorsByPatient(patId);

        if (result == null) return NotFound();

        return Ok(result);

        // throw error if patient not found, ask Jason how to do this in linq query

    }

    [HttpPost("{patId}/doctor/{docId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Doctor))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult AddNewDoctorForPatient(int patId, int docId)
    {
        var result = _monoRecRepository.AddNewDoctorForPatient(patId, docId);

        if (result == null) return NotFound();

        return Ok(result);

        // ask if we need to declare method return types (in repository) as nullable, and edit interface/repository if necessary 
    }

    [HttpDelete("{patId}/doctor/{docId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Doctor))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteDoctorForPatient(int patId, int docId)
    {
        var result = _monoRecRepository.DeleteDoctorForPatient(patId, docId);

        if (result == null) return NotFound();

        return Ok(result);

        // should delete route return something? the object that was deleted?
    }

}

