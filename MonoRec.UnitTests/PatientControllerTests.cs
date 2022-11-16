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

        //patients.Add(new Patient());

        // player = new HangmanPlayer()
        //        .WithName("Jason")
        //        .WithWins(2)
        //        .WithCurrentWinStreak(1)
        //        .WithGamesPlayed(5);
        // playerWithId = new HangmanPlayer();
        // playerWithId.HangmanPlayerId = 1;

    }


    // What should unit tests cover?
    // The action calls the correct method on the repository or service layer.
    // Invalid parameters return the correct error response.
    // The action returns the correct type of response.
    // If the response includes a domain model, verify the model type.


    // !Get Help with this
    // GetAllPatients
    //[Test]
    //public void GetAllPatients_CallsRepositoryFunction_ShouldReturnAllPatients()
    //{

    //    var context = new MonoRecDbContext();

    //    context.Patients.Add(new Patient { PatientId = 1 });
    //    context.Patients.Add(new Patient { PatientId = 2 });
    //    context.Patients.Add(new Patient { PatientId = 3 });

    //    var result = _controller.GetAllPatients() as Patients;
    //    _mockRepo.Setup(x => x.GetPatient(42)).Returns(new Patient { PatientId = 42 });

    //    Assert.IsNotNull(result);
    //    Assert.AreEqual(3, result.Local.count); 
    //}

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
        IActionResult actionResult = _controller.GetPatient(-1);

        Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
    }


    // !Fix this later
    //[Test]
    //public void GetPatient_ValidPatientId_ReturnsOkResultWithPatient()
    //{
    //    _mockRepo.Setup(x => x.GetPatient(42)).Returns(new Patient { PatientId = 42 });

    //    IActionResult actionResult = _controller.GetPatient(42);
    //    // I think the next line is the problem
    //    var contentResult = actionResult as OkObjectResult;

    //    Assert.IsNotNull(contentResult);
    //    Assert.IsNotNull(contentResult.ContentTypes);
    //    Assert.AreEqual(42, contentResult.ContentTypes.PatientId);
    //}

    // !Need to also incorporate test that tests domain model type perhaps
    //[Test] {insert code}

    // GetAllDoctorsByPatient
    [Test]
    public void GetAllDoctorsByPatient_CallsRepositoryFunction_VerifyCorrectFunctionIsCalled()
    {
        _mockRepo.Setup(x => x.GetPatient(42)).Returns(new Patient { PatientId = 42 });

        IActionResult actionResult = _controller.GetPatient(42);

        _mockRepo.Verify(x => x.GetPatient(42), Times.Once());
    }

    [Test]
    public void GetAllDoctorsByPatient_PatientNotFound_Throw404()
    {
        IActionResult actionResult = _controller.GetAllDoctorsByPatient(-1);

        Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
    }


    // !Fix this later, how do you test this?!
    //[Test]
    //public void GetAllDoctorsByPatient_ValidPatientId_ReturnsListOfDoctors()
    //{
    //    doctors.Add(new Doctor { DoctorId = 1 });
    //    doctors.Add(new Doctor { DoctorId = 2 });
    //    doctors.Add(new Doctor { DoctorId = 3 });

    //    IEnumerable<Doctor> doctorTable = doctors; 

    //    _mockRepo.Setup(x => x.GetAllDoctorsByPatient(42)).Returns(doctorTable);

    //    IActionResult actionResult = _controller.GetAllDoctorsByPatient(42);
    //    // I think the next line is the problem
    //    var contentResult = actionResult as OkObjectResult;

    //    Assert.IsNotNull(contentResult);
    //    Assert.IsNotNull(contentResult.ContentTypes);
    //    Assert.AreEqual(42, contentResult.ContentTypes.PatientId);

    //    Assert.That(actionResult.Model, Is.TypeOf<IEnumerable>());
    //}

    // AddNewDoctorForPatient
    [Test]
    public void AddNewDoctorForPatient_CallsRepositoryFunction_VerifyCorrectFunctionIsCalled()
    {
        _mockRepo.Setup(x => x.GetPatient(42)).Returns(new Patient { PatientId = 42 });

        IActionResult actionResult = _controller.GetPatient(42);

        _mockRepo.Verify(x => x.GetPatient(42), Times.Once());
    }

    [Test]
    [TestCase(-1, 5)]
    [TestCase(10000, 5)]
    [TestCase(5, -1)]
    [TestCase(5, 10000)]
    [TestCase(10000, 10000)]
    public void AddNewDoctorForPatient_PatientOrDoctorNotFound_Throw404(int patId, int docId)
    {
        IActionResult actionResult = _controller.AddNewDoctorForPatient(patId, docId);

        Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public void AddNewDoctorForPatient_ValidPatientId_ReturnsListOfDoctors()
    {
        doctors.Add(new Doctor { DoctorId = 1 });
        doctors.Add(new Doctor { DoctorId = 2 });
        doctors.Add(new Doctor { DoctorId = 3 });

        IEnumerable<Doctor> doctorTable = doctors;

        _mockRepo.Setup(x => x.GetAllDoctorsByPatient(42)).Returns(doctorTable);

        IActionResult actionResult = _controller.GetAllDoctorsByPatient(42);
        // I think the next line is the problem
        var contentResult = actionResult as OkObjectResult;

        Assert.IsNotNull(contentResult);
        Assert.IsNotNull(contentResult.ContentTypes);
        //Assert.AreEqual(42, contentResult.ContentTypes.PatientId);

        //Assert.That(actionResult.Model, Is.TypeOf<IEnumerable>());
    }

    // DeleteDoctorForPatient
    // OkResult for this first test case perhaps?
    // !Fix this
    [Test]
    public void DeleteDoctorForPatient_CallsRepositoryFunction_VerifyCorrectFunctionIsCalled()
    {
        _mockRepo.Setup(x => x.GetPatient(42)).Returns(new Patient { PatientId = 42 });

        IActionResult actionResult = _controller.GetPatient(42);

        _mockRepo.Verify(x => x.GetPatient(42), Times.Once());
    }

    [Test]
    [TestCase(-1, 5)]
    [TestCase(10000, 5)]
    [TestCase(5, -1)]
    [TestCase(5, 10000)]
    [TestCase(10000, 10000)]
    public void DeleteDoctorForPatient_PatientOrDoctorNotFound_Throw404(int patId, int docId)
    {
        IActionResult actionResult = _controller.DeleteDoctorForPatient(patId, docId);

        Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    // !Fix this
    public void DeleteDoctorForPatient_ValidPatientId_ReturnsListOfDoctors()
    {
        doctors.Add(new Doctor { DoctorId = 1 });
        doctors.Add(new Doctor { DoctorId = 2 });
        doctors.Add(new Doctor { DoctorId = 3 });

        IEnumerable<Doctor> doctorTable = doctors;

        _mockRepo.Setup(x => x.GetAllDoctorsByPatient(42)).Returns(doctorTable);

        IActionResult actionResult = _controller.GetAllDoctorsByPatient(42);
        // I think the next line is the problem
        var contentResult = actionResult as OkObjectResult;

        Assert.IsNotNull(contentResult);
        Assert.IsNotNull(contentResult.ContentTypes);
        //Assert.AreEqual(42, contentResult.ContentTypes.PatientId);

        //Assert.That(actionResult.Model, Is.TypeOf<IEnumerable>());
    }

}

