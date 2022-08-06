using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonoRec.Models;
using MonoRec.Repositories;

namespace MonoRec.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PatientController : Controller
    {
        // GET: 
        private IMonoRecRepository _monoRecRepository;

        public PatientController(IMonoRecRepository monoRecRepository)
        {
            _monoRecRepository = monoRecRepository;
        }

        [HttpGet]
        public IEnumerable<Patient> GetAllPatients()
        {
            return _monoRecRepository.GetAllPatients();
        }

        [HttpPost]
        public IEnumerable<Patient> CreateNewPatient()
        {
            return _monoRecRepository.GetAllPatients();
        }

    }
}
