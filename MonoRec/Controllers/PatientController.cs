﻿using Microsoft.AspNetCore.Http;
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
    public IEnumerable<Patient> GetAllPatients()
    {
        Console.WriteLine("get method function ran");
        var controllerTest = _monoRecRepository.GetAllPatients();
        return controllerTest;
    }

    [HttpPost("{name}")]
    public Patient CreateNewPatient(string name)
    {
        return _monoRecRepository.CreateNewPatient(name);
    }

}

