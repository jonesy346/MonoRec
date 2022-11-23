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
public class DoctorControllerTests
{
    Mock<IDoctorRepository> _mockRepo;
    DoctorController _controller;
    IList<Doctor> doctors;
    IList<Patient> patients;

    [SetUp]
    public void Setup()
    {
        _mockRepo = new Mock<IDoctorRepository>();
        _controller = new DoctorController(_mockRepo.Object);

        doctors = new List<Doctor>();
        patients = new List<Patient>();
    }


    // What should unit tests cover?
    // 1) The action calls the correct method on the repository or service layer.
    // 2) Invalid parameters return the correct error response.
    // 3) The action returns the correct type of response.
    // 4) If the response includes a domain model, verify the model type.

    // GetAllDoctors
    [Test]
    public void GetAllDoctors_CallsRepositoryFunction_VerifyCorrectFunctionIsCalled()
    {
        doctors.Add(new Doctor { DoctorId = 1 });
        doctors.Add(new Doctor { DoctorId = 2 });
        doctors.Add(new Doctor { DoctorId = 3 });

        IEnumerable<Doctor> doctorTable = doctors;

        _mockRepo.Setup(x => x.GetAllDoctors()).Returns(doctorTable);

        IEnumerable<Doctor> actionResult = _controller.GetAllDoctors();

        _mockRepo.Verify(x => x.GetAllDoctors(), Times.Once());

    }

    [Test]
    public void GetAllDoctors_CallsRepositoryFunction_ShouldReturnAllDoctors()
    {
        doctors.Add(new Doctor { DoctorId = 1 });
        doctors.Add(new Doctor { DoctorId = 2 });
        doctors.Add(new Doctor { DoctorId = 3 });

        IEnumerable<Doctor> doctorTable = doctors;

        _mockRepo.Setup(x => x.GetAllDoctors()).Returns(doctorTable);

        IEnumerable<Doctor> result = _controller.GetAllDoctors();

        Assert.That(result, Is.EqualTo(doctorTable));
    }

    // Get Doctor
    [Test]
    public void GetDoctor_CallsRepositoryFunction_VerifyCorrectFunctionIsCalled()
    {
        _mockRepo.Setup(x => x.GetDoctor(42)).Returns(new Doctor { DoctorId = 42 });

        IActionResult actionResult = _controller.GetDoctor(42);

        _mockRepo.Verify(x => x.GetDoctor(42), Times.Once());
    }

    [Test]
    public void GetDoctor_DoctorNotFound_Throw404()
    {
        _mockRepo.Setup(x => x.GetDoctor(-1)).Returns((Doctor)null);

        IActionResult actionResult = _controller.GetDoctor(-1);

        Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public void GetDoctor_ValidDoctorId_ReturnsOkResultWithDoctor()
    {
        _mockRepo.Setup(x => x.GetDoctor(42)).Returns(new Doctor { DoctorId = 42 });

        IActionResult actionResult = _controller.GetDoctor(42);

        var contentResult = actionResult as OkObjectResult;
        // Don't need to incorporate test that tests domain model type
        // because of next line that casts a Doctor model
        var doctorObject = contentResult.Value as Doctor;

        Assert.That(contentResult.StatusCode, Is.EqualTo(200));
        Assert.That(doctorObject.DoctorId, Is.EqualTo(42));
    }


    // GetAllPatientsByDoctor
    [Test]
    public void GetAllPatientsByDoctor_CallsRepositoryFunction_VerifyCorrectFunctionIsCalled()
    {
        patients.Add(new Patient { PatientId = 1 });
        patients.Add(new Patient { PatientId = 2 });
        patients.Add(new Patient { PatientId = 3 });

        IEnumerable<Patient> patientTable = patients;

        _mockRepo.Setup(x => x.GetAllPatientsByDoctor(42)).Returns(patientTable);

        IActionResult actionResult = _controller.GetAllPatientsByDoctor(42);

        _mockRepo.Verify(x => x.GetAllPatientsByDoctor(42), Times.Once());
    }

    [Test]
    public void GetAllPatientsByDoctor_PatientNotFound_Throw404()
    {
        _mockRepo.Setup(x => x.GetAllPatientsByDoctor(-1)).Returns((IEnumerable<Patient>)null);

        IActionResult actionResult = _controller.GetAllPatientsByDoctor(-1);

        Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public void GetAllPatientsByDoctor_ValidDoctorId_ReturnsListOfDoctors()
    {
        patients.Add(new Patient { PatientId = 1 });
        patients.Add(new Patient { PatientId = 2 });
        patients.Add(new Patient { PatientId = 3 });

        IEnumerable<Patient> patientTable = patients;

        _mockRepo.Setup(x => x.GetAllPatientsByDoctor(42)).Returns(patientTable);

        IActionResult actionResult = _controller.GetAllPatientsByDoctor(42);

        var contentResult = actionResult as OkObjectResult;
        var patientObject = contentResult.Value as IEnumerable<Patient>;

        Assert.That(patientObject.Count, Is.EqualTo(3));
    }

    // AddNewPatientForDoctor
    [Test]
    [TestCase(1, 1)]
    [TestCase(1, 10)]
    [TestCase(10, 1)]
    [TestCase(10, 10)]
    public void AddNewPatientForDoctor_CallsRepositoryFunction_VerifyCorrectFunctionIsCalled(int docId, int patId)
    {
        _mockRepo.Setup(x => x.AddNewPatientForDoctor(docId, patId)).Returns(new Patient { PatientId = patId });

        IActionResult actionResult = _controller.AddNewPatientForDoctor(docId, patId);

        _mockRepo.Verify(x => x.AddNewPatientForDoctor(docId, patId), Times.Once());
    }

    [Test]
    [TestCase(-1, 5)]
    [TestCase(1, 1)]
    [TestCase(10000, 5)]
    [TestCase(5, -1)]
    [TestCase(5, 10000)]
    [TestCase(10000, 10000)]
    public void AddNewPatientForDoctor_PatientOrDoctorNotFound_Throw404(int docId, int patId)
    {
        _mockRepo.Setup(x => x.AddNewPatientForDoctor(docId, patId)).Returns((Patient)null);

        IActionResult actionResult = _controller.AddNewPatientForDoctor(docId, patId);

        Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    [TestCase(1, 1)]
    [TestCase(1, 10)]
    [TestCase(10, 1)]
    [TestCase(10, 10)]
    public void AddNewPatientForDoctor_ValidDoctorId_ReturnsOkResultWithPatient(int docId, int patId)
    {
        _mockRepo.Setup(x => x.AddNewPatientForDoctor(docId, patId)).Returns(new Patient { PatientId = patId });

        IActionResult actionResult = _controller.AddNewPatientForDoctor(docId, patId);

        var contentResult = actionResult as OkObjectResult;
        // Don't need to incorporate test that tests domain model type
        // because of next line that casts a doctor model
        var patientObject = contentResult.Value as Patient;

        Assert.That(contentResult.StatusCode, Is.EqualTo(200));
        Assert.That(patientObject.PatientId, Is.EqualTo(patId));
    }

    // DeletePatientForDoctor
    [Test]
    [TestCase(1, 1)]
    [TestCase(1, 10)]
    [TestCase(10, 1)]
    [TestCase(10, 10)]
    public void DeletePatientForDoctor_CallsRepositoryFunction_VerifyCorrectFunctionIsCalled(int docId, int patId)
    {
        _mockRepo.Setup(x => x.DeletePatientForDoctor(docId, patId)).Returns(new Patient { PatientId = patId });

        IActionResult actionResult = _controller.DeletePatientForDoctor(docId, patId);

        _mockRepo.Verify(x => x.DeletePatientForDoctor(docId, patId), Times.Once());
    }

    [Test]
    [TestCase(-1, 5)]
    [TestCase(1, 1)]
    [TestCase(10000, 5)]
    [TestCase(5, -1)]
    [TestCase(5, 10000)]
    [TestCase(10000, 10000)]
    public void DeletePatientForDoctor_PatientOrDoctorNotFound_Throw404(int docId, int patId)
    {
        _mockRepo.Setup(x => x.DeletePatientForDoctor(docId, patId)).Returns((Patient)null);

        IActionResult actionResult = _controller.DeletePatientForDoctor(docId, patId);

        Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    [TestCase(1, 1)]
    [TestCase(1, 10)]
    [TestCase(10, 1)]
    [TestCase(10, 10)]
    public void DeletePatientForDoctor_ValidPatientId_ReturnsOkResultWithPatient(int docId, int patId)
    {
        _mockRepo.Setup(x => x.DeletePatientForDoctor(docId, patId)).Returns(new Patient { PatientId = patId });

        IActionResult actionResult = _controller.DeletePatientForDoctor(docId, patId);

        var contentResult = actionResult as OkObjectResult;
        // Don't need to incorporate test that tests domain model type
        // because of next line that casts a doctor model
        var doctorObject = contentResult.Value as Patient;

        Assert.That(contentResult.StatusCode, Is.EqualTo(200));
        Assert.That(doctorObject.PatientId, Is.EqualTo(patId));
    }

}

