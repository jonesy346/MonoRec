using System.IO;
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Visit))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetVisit(int visitId)
    {
        var visit = _monoRecRepository.GetVisit(visitId);

        if (visit == null) return NotFound();

        return Ok(visit);
    }

    [HttpPost("{patId}/{docId}")]
    public Visit? CreateNewVisit(int patId, int docId)
    {
        return _monoRecRepository.CreateNewVisit(patId, docId);
    }

    [HttpGet("patient/{patId}")]
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
    public IActionResult GetAllVistsByDoctorPatient(int docId, int patId)
    {
        var result = _monoRecRepository.GetAllVisitsByDoctorPatient(docId, patId);

        if (result == null) return NotFound();

        return Ok(result);
    }

    // Only doctors should be able to delete a visit. Implement authorization later

    [HttpDelete("{visitId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Visit))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteVisit(int visitId)
    {
        var result = _monoRecRepository.DeleteVisit(visitId);

        if (result == null) return NotFound();

        return Ok(result);
    }
}

