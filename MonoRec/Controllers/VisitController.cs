using System.IO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonoRec.Models;
using MonoRec.Repositories;

namespace MonoRec.Controllers;

[Route("[controller]")]
[ApiController]
public class VisitController : ControllerBase
{
    // GET: 
    private IVisitRepository _monoRecRepository;

    public VisitController(IVisitRepository monoRecRepository)
    {
        _monoRecRepository = monoRecRepository;
    }

    [HttpGet]
    public IEnumerable<Visit> GetAllVisits()
    {
        var controllerTest = _monoRecRepository.GetAllVisits();
        return controllerTest;
    }

    [HttpGet("{visitId}")]
    [Authorize(AuthenticationSchemes = "IdentityServerJwt", Roles = "Patient,Doctor")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Visit))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetVisit(int visitId)
    {
        var visit = _monoRecRepository.GetVisit(visitId);

        if (visit == null) return NotFound();

        return Ok(visit);
    }

    [HttpPost("{patId}/{docId}")]
    [Authorize(AuthenticationSchemes = "IdentityServerJwt", Roles = "Doctor")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Visit))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult CreateNewVisit(int patId, int docId)
    {
        var result = _monoRecRepository.CreateNewVisit(patId, docId);

        if (result == null) return NotFound();

        return Ok(result);
    }

    [HttpGet("patient/{patId}")]
    [Authorize(AuthenticationSchemes = "IdentityServerJwt", Roles = "Patient,Doctor")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Visit>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetAllVisitsByPatient(int patId)
    {
        var result = _monoRecRepository.GetAllVisitsByPatient(patId);

        if (result == null) return NotFound();

        return Ok(result);
    }

    [HttpGet("doctor/{docId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Visit>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetAllVisitsByDoctor(int docId)
    {
        var result = _monoRecRepository.GetAllVisitsByDoctor(docId);

        if (result == null) return NotFound();

        return Ok(result);
    }

    [HttpGet("patient/{patId}/doctor/{docId}")]
    [HttpGet("doctor/{docId}/patient/{patId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Visit>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetAllVisitsByDoctorPatient(int docId, int patId)
    {
        var result = _monoRecRepository.GetAllVisitsByDoctorPatient(docId, patId);

        if (result == null) return NotFound();

        return Ok(result);
    }

    [HttpDelete("{visitId}")]
    [Authorize(AuthenticationSchemes = "IdentityServerJwt", Roles = "Doctor")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Visit))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteVisit(int visitId)
    {
        var result = _monoRecRepository.DeleteVisit(visitId);

        if (result == null) return NotFound();

        return Ok(result);
    }

    [HttpGet("upcoming")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Visit>))]
    public IActionResult GetUpcomingVisits()
    {
        var result = _monoRecRepository.GetUpcomingVisits();
        return Ok(result);
    }
}

