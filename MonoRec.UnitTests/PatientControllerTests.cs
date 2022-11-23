using System;
using MonoRec.Models;
using MonoRec.Repositories;
using MonoRec.Controllers;
using Moq;
using Microsoft.AspNetCore.Mvc;
using static System.Collections.Specialized.BitVector32;
using MonoRec.Data;

namespace MonoRec.UnitTests;

[TestFixture]
public class PatientControllerTests
{
    Mock<IPatientRepository> _mockRepo;
    PatientController _controller;
    IList<Patient> patients;
    IList<Doctor> doctors;

    [SetUp]
    public void Setup()
    {
        _mockRepo = new Mock<IPatientRepository>();
        _controller = new PatientController(_mockRepo.Object);

        patients = new List<Patient>();
        doctors = new List<Doctor>();
    }


    // What should unit tests cover?
    // 1) The action calls the correct method on the repository or service layer.
    // 2) Invalid parameters return the correct error response.
    // 3) The action returns the correct type of response.
    // 4) If the response includes a domain model, verify the model type.

    // GetAllPatients
    [Test]
    public void GetAllPatients_CallsRepositoryFunction_VerifyCorrectFunctionIsCalled()
    {
        patients.Add(new Patient { PatientId = 1 });
        patients.Add(new Patient { PatientId = 2 });
        patients.Add(new Patient { PatientId = 3 });

        IEnumerable<Patient> patientTable = patients;

        _mockRepo.Setup(x => x.GetAllPatients()).Returns(patientTable);

        IEnumerable<Patient> actionResult = _controller.GetAllPatients();

        _mockRepo.Verify(x => x.GetAllPatients(), Times.Once());

    }

    [Test]
    public void GetAllPatients_CallsRepositoryFunction_ShouldReturnAllPatients()
    {
        patients.Add(new Patient { PatientId = 1 });
        patients.Add(new Patient { PatientId = 2 });
        patients.Add(new Patient { PatientId = 3 });

        IEnumerable<Patient> patientTable = patients;

        _mockRepo.Setup(x => x.GetAllPatients()).Returns(patientTable);

        IEnumerable<Patient> result = _controller.GetAllPatients();

        Assert.That(result, Is.EqualTo(patientTable)); 
    }

    // GetPatient
    [Test]
    public void GetPatient_CallsRepositoryFunction_VerifyCorrectFunctionIsCalled()
    {
        _mockRepo.Setup(x => x.GetPatient(42)).Returns(new Patient { PatientId = 42 });

        IActionResult actionResult = _controller.GetPatient(42);

        _mockRepo.Verify(x => x.GetPatient(42), Times.Once());
    }

    [Test]
    public void GetPatient_PatientNotFound_Throw404()
    {
        _mockRepo.Setup(x => x.GetPatient(-1)).Returns((Patient)null);

        IActionResult actionResult = _controller.GetPatient(-1);

        Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public void GetPatient_ValidPatientId_ReturnsOkResultWithPatient()
    {
        _mockRepo.Setup(x => x.GetPatient(42)).Returns(new Patient { PatientId = 42 });

        IActionResult actionResult = _controller.GetPatient(42);
        
	    var contentResult = actionResult as OkObjectResult;
        // Don't need to incorporate test that tests domain model type
        // because of next line that casts a patient model
        var patientObject = contentResult.Value as Patient;

        Assert.That(contentResult.StatusCode, Is.EqualTo(200));
        Assert.That(patientObject.PatientId, Is.EqualTo(42));
    }


    // GetAllDoctorsByPatient
    [Test]
    public void GetAllDoctorsByPatient_CallsRepositoryFunction_VerifyCorrectFunctionIsCalled()
    {
        doctors.Add(new Doctor { DoctorId = 1 });
        doctors.Add(new Doctor { DoctorId = 2 });
        doctors.Add(new Doctor { DoctorId = 3 });

        IEnumerable<Doctor> doctorTable = doctors;

        _mockRepo.Setup(x => x.GetAllDoctorsByPatient(42)).Returns(doctorTable);

        IActionResult actionResult = _controller.GetAllDoctorsByPatient(42);

        _mockRepo.Verify(x => x.GetAllDoctorsByPatient(42), Times.Once());
    }

    [Test]
    public void GetAllDoctorsByPatient_PatientNotFound_Throw404()
    {
        _mockRepo.Setup(x => x.GetAllDoctorsByPatient(-1)).Returns((IEnumerable<Doctor>)null);

        IActionResult actionResult = _controller.GetAllDoctorsByPatient(-1);

        Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public void GetAllDoctorsByPatient_ValidPatientId_ReturnsListOfDoctors()
    {
        doctors.Add(new Doctor { DoctorId = 1 });
        doctors.Add(new Doctor { DoctorId = 2 });
        doctors.Add(new Doctor { DoctorId = 3 });

        IEnumerable<Doctor> doctorTable = doctors; 

        _mockRepo.Setup(x => x.GetAllDoctorsByPatient(42)).Returns(doctorTable);

        IActionResult actionResult = _controller.GetAllDoctorsByPatient(42);
        
	    var contentResult = actionResult as OkObjectResult;
        var doctorObject = contentResult.Value as IEnumerable<Doctor>;

        Assert.That(doctorObject.Count, Is.EqualTo(3));
    }

    // AddNewDoctorForPatient
    [Test]
    [TestCase(1, 1)]
    [TestCase(1, 10)]
    [TestCase(10, 1)]
    [TestCase(10, 10)]
    public void AddNewDoctorForPatient_CallsRepositoryFunction_VerifyCorrectFunctionIsCalled(int patId, int docId)
    {
        _mockRepo.Setup(x => x.AddNewDoctorForPatient(patId, docId)).Returns(new Doctor { DoctorId = docId });

        IActionResult actionResult = _controller.AddNewDoctorForPatient(patId, docId);

        _mockRepo.Verify(x => x.AddNewDoctorForPatient(patId, docId), Times.Once());
    }

    [Test]
    [TestCase(-1, 5)]
    [TestCase(1, 1)]
    [TestCase(10000, 5)]
    [TestCase(5, -1)]
    [TestCase(5, 10000)]
    [TestCase(10000, 10000)]
    public void AddNewDoctorForPatient_PatientOrDoctorNotFound_Throw404(int patId, int docId)
    {
        _mockRepo.Setup(x => x.AddNewDoctorForPatient(patId, docId)).Returns((Doctor)null);

        IActionResult actionResult = _controller.AddNewDoctorForPatient(patId, docId);

        Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    [TestCase(1, 1)]
    [TestCase(1, 10)]
    [TestCase(10, 1)]
    [TestCase(10, 10)]
    public void AddNewDoctorForPatient_ValidPatientId_ReturnsOkResultWithDoctor(int patId, int docId)
    {
        _mockRepo.Setup(x => x.AddNewDoctorForPatient(patId, docId)).Returns(new Doctor { DoctorId = docId });

        IActionResult actionResult = _controller.AddNewDoctorForPatient(patId, docId);

        var contentResult = actionResult as OkObjectResult;
        // Don't need to incorporate test that tests domain model type
        // because of next line that casts a doctor model
        var doctorObject = contentResult.Value as Doctor;

        Assert.That(contentResult.StatusCode, Is.EqualTo(200));
        Assert.That(doctorObject.DoctorId, Is.EqualTo(docId));
    }

    // DeleteDoctorForPatient
    [Test]
    [TestCase(1, 1)]
    [TestCase(1, 10)]
    [TestCase(10, 1)]
    [TestCase(10, 10)]
    public void DeleteDoctorForPatient_CallsRepositoryFunction_VerifyCorrectFunctionIsCalled(int patId, int docId)
    {
        _mockRepo.Setup(x => x.DeleteDoctorForPatient(patId, docId)).Returns(new Doctor { DoctorId = 42 });

        IActionResult actionResult = _controller.DeleteDoctorForPatient(patId, docId);

        _mockRepo.Verify(x => x.DeleteDoctorForPatient(patId, docId), Times.Once());
    }

    [Test]
    [TestCase(-1, 5)]
    [TestCase(1, 1)]
    [TestCase(10000, 5)]
    [TestCase(5, -1)]
    [TestCase(5, 10000)]
    [TestCase(10000, 10000)]
    public void DeleteDoctorForPatient_PatientOrDoctorNotFound_Throw404(int patId, int docId)
    {
        _mockRepo.Setup(x => x.DeleteDoctorForPatient(patId, docId)).Returns((Doctor)null);

        IActionResult actionResult = _controller.DeleteDoctorForPatient(patId, docId);

        Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    [TestCase(1, 1)]
    [TestCase(1, 10)]
    [TestCase(10, 1)]
    [TestCase(10, 10)]
    public void DeleteDoctorForPatient_ValidPatientId_ReturnsOkResultWithDoctor(int patId, int docId)
    {
        _mockRepo.Setup(x => x.DeleteDoctorForPatient(patId, docId)).Returns(new Doctor { DoctorId = docId });

        IActionResult actionResult = _controller.DeleteDoctorForPatient(patId, docId);

        var contentResult = actionResult as OkObjectResult;
        // Don't need to incorporate test that tests domain model type
        // because of next line that casts a doctor model
        var doctorObject = contentResult.Value as Doctor;

        Assert.That(contentResult.StatusCode, Is.EqualTo(200));
        Assert.That(doctorObject.DoctorId, Is.EqualTo(docId));
    }

}

