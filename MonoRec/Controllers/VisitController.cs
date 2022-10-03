﻿using Microsoft.AspNetCore.Http;
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
    public Visit GetVisit(int visitId)
    {
        return _monoRecRepository.GetVisit(visitId);
    }

    [HttpPost("{patId}/{docId}")]
    public Visit CreateNewVisit(int patId, int docId)
    {
        return _monoRecRepository.CreateNewVisit(patId, docId);
    }

    [HttpGet("patient/{patId}")]
    public IEnumerable<Visit> GetAllVisitsByPatient(int patId)
    {
        var controllerTest = _monoRecRepository.GetAllVisitsByPatient(patId);
        return controllerTest;
    }

    [HttpGet("doctor/{docId}")]
    public IEnumerable<Visit> GetAllVisitsByDoctor(int docId)
    {
        var controllerTest = _monoRecRepository.GetAllVisitsByDoctor(docId);
        return controllerTest;
    }

    [HttpGet("patient/{patId}/doctor/{docId}")]
    [HttpGet("doctor/{docId}/patient/{patId}")]
    public IEnumerable<Visit> GetAllVistsByDoctorPatient(int docId, int patId)
    {
        return _monoRecRepository.GetAllVisitsByDoctorPatient(docId, patId);
    }

    // Only doctors should be able to delete a visit. Implement authorization later

    [HttpDelete("{visitId}")]
    public Visit DeleteVisit(int visitId)
    {
        var controllerTest = _monoRecRepository.DeleteVisit(visitId);
        return controllerTest;
    }
}

