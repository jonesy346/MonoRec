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
    public IEnumerable<Patient> GetAllPatients()
    {
        var result = _monoRecRepository.GetAllPatients();
        return result;
    }

    [HttpGet("{patId}")]
    public Patient GetPatient(int patId)
    {
        // add if statement for if this returns null, do this for all parameters

        var patient = _monoRecRepository.GetPatient(patId);

        if (patient == null)
        {
            throw new ArgumentOutOfRangeException();
            //ask Jason if this is correct

            //return new HttpNotFoundResult(); 
            //above line is for rendering a view
        }

        return patient;
    }

    [HttpPost("{name}")]
    public Patient CreateNewPatient(string name)
    {

        return _monoRecRepository.CreateNewPatient(name);

        // should post route return patient that was created?
    }

    [HttpGet("{patId}/doctor")]
    public IEnumerable<Doctor> GetAllDoctorsByPatient(int patId)
    {
        return _monoRecRepository.GetAllDoctorsByPatient(patId);

        // throw error if patient not found, ask Jason how to do this in linq query

    }

    [HttpPost("{patId}/doctor/{docId}")]
    public Doctor AddNewDoctorForPatient(int patId, int docId)
    {
        return _monoRecRepository.AddNewDoctorForPatient(patId, docId);

        // ask if we need to declare method return types (in repository) as nullable, and edit interface/repository if necessary 
    }

    [HttpDelete("{patId}/doctor/{docId}")]
    public Doctor DeleteDoctorForPatient(int patId, int docId)
    {
        return _monoRecRepository.DeleteDoctorForPatient(patId, docId);

        // should delete route return something? the object that was deleted?
    }

}

