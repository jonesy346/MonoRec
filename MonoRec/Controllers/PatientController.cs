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

    /// <summary>
    /// Gets all patients with role-based filtering and optional current-user filtering.
    ///
    /// Security implementation:
    /// - When currentUserOnly=true: Returns only the patient record associated with the authenticated user's userId.
    ///   This allows patients to retrieve their own patientId without exposing other patients' data.
    /// - When currentUserOnly=false (or omitted): Requires Doctor role to access full patient list.
    ///   Non-doctors receive an empty list, protecting patient data from unauthorized access.
    ///
    /// Uses the "sub" claim from IdentityServer for user identification, which is consistent across
    /// cookie-based (Identity.Application) and JWT-based (IdentityServerJwt) authentication.
    /// </summary>
    /// <param name="currentUserOnly">When true, filters to only the current user's patient record</param>
    /// <returns>Filtered collection of patients based on authentication and authorization</returns>
    [HttpGet]
    [Authorize(AuthenticationSchemes = "Identity.Application,IdentityServerJwt")]
    public IEnumerable<Patient> GetAllPatients([FromQuery] bool currentUserOnly = false)
    {
        var result = _monoRecRepository.GetAllPatients();

        // If currentUserOnly is requested, filter to just the current user's patient
        if (currentUserOnly)
        {
            var userId = User.FindFirst("sub")?.Value;

            if (userId != null)
            {
                result = result.Where(p => p.UserId == userId);
            }
            else
            {
                result = Enumerable.Empty<Patient>();
            }

            return result;
        }

        // Otherwise, require Doctor role for full patient list
        if (!User.IsInRole("Doctor"))
        {
            return Enumerable.Empty<Patient>();
        }

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

