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
public class VisitControllerTests
{
    Mock<IVisitRepository> _mockRepo;
    VisitController _controller;
    IList<Visit> visits;
    IList<Doctor> doctors;
    IList<Patient> patients;

    [SetUp]
    public void Setup()
    {
        _mockRepo = new Mock<IVisitRepository>();
        _controller = new VisitController(_mockRepo.Object);

        visits = new List<Visit>();
        patients = new List<Patient>();
        doctors = new List<Doctor>();
    }


    // What should unit tests cover?
    // 1) The action calls the correct method on the repository or service layer.
    // 2) Invalid parameters return the correct error response.
    // 3) The action returns the correct type of response.
    // 4) If the response includes a domain model, verify the model type.

    // GetAllVisits
    [Test]
    public void GetAllVisits_CallsRepositoryFunction_VerifyCorrectFunctionIsCalled()
    {
        visits.Add(new Visit { VisitId = 1 });
        visits.Add(new Visit { VisitId = 2 });
        visits.Add(new Visit { VisitId = 3 });

        IEnumerable<Visit> visitTable = visits;

        _mockRepo.Setup(x => x.GetAllVisits()).Returns(visitTable);

        IEnumerable<Visit> actionResult = _controller.GetAllVisits();

        _mockRepo.Verify(x => x.GetAllVisits(), Times.Once());

    }

    [Test]
    public void GetAllVisits_CallsRepositoryFunction_ShouldReturnAllVisits()
    {
        visits.Add(new Visit { VisitId = 1 });
        visits.Add(new Visit { VisitId = 2 });
        visits.Add(new Visit { VisitId = 3 });

        IEnumerable<Visit> visitTable = visits;

        _mockRepo.Setup(x => x.GetAllVisits()).Returns(visitTable);

        IEnumerable<Visit> result = _controller.GetAllVisits();

        Assert.That(result, Is.EqualTo(visitTable));
    }

    // GetVisit
    [Test]
    public void GetVisit_CallsRepositoryFunction_VerifyCorrectFunctionIsCalled()
    {
        _mockRepo.Setup(x => x.GetVisit(42)).Returns(new Visit { VisitId = 42 });

        IActionResult actionResult = _controller.GetVisit(42);

        _mockRepo.Verify(x => x.GetVisit(42), Times.Once());
    }

    [Test]
    public void GetVisit_VisitNotFound_Throw404()
    {
        _mockRepo.Setup(x => x.GetVisit(-1)).Returns((Visit)null);

        IActionResult actionResult = _controller.GetVisit(-1);

        Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public void GetVisit_ValidVisitId_ReturnsOkResultWithVisit()
    {
        _mockRepo.Setup(x => x.GetVisit(42)).Returns(new Visit { VisitId = 42 });

        IActionResult actionResult = _controller.GetVisit(42);

        var contentResult = actionResult as OkObjectResult;
        // Don't need to incorporate test that tests domain model type
        // because of next line that casts a Visit model
        var visitObject = contentResult.Value as Visit;

        Assert.That(contentResult.StatusCode, Is.EqualTo(200));
        Assert.That(visitObject.VisitId, Is.EqualTo(42));
    }

    // CreateNewVisit
    [Test]
    [TestCase(1, 1)]
    [TestCase(1, 10)]
    [TestCase(10, 1)]
    [TestCase(10, 10)]
    public void CreateNewVisit_CallsRepositoryFunction_VerifyCorrectFunctionIsCalled(int patId, int docId)
    {
        _mockRepo.Setup(x => x.CreateNewVisit(patId, docId)).Returns(new Visit { PatientId = patId, DoctorId = docId });

        IActionResult actionResult = _controller.CreateNewVisit(patId, docId);

        _mockRepo.Verify(x => x.CreateNewVisit(patId, docId), Times.Once());
    }

    [Test]
    [TestCase(-1, 5)]
    [TestCase(1, 1)]
    [TestCase(10000, 5)]
    [TestCase(5, -1)]
    [TestCase(5, 10000)]
    [TestCase(10000, 10000)]
    public void CreateNewVisit_VisitNotFound_Throw404(int patId, int docId)
    {
        _mockRepo.Setup(x => x.CreateNewVisit(patId, docId)).Returns((Visit)null);

        IActionResult actionResult = _controller.CreateNewVisit(patId, docId);

        Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    [TestCase(1, 1)]
    [TestCase(1, 10)]
    [TestCase(10, 1)]
    [TestCase(10, 10)]
    public void CreateNewVisit_ValidVisitId_ReturnsOkResultWithVisit(int patId, int docId)
    {
        _mockRepo.Setup(x => x.CreateNewVisit(patId, docId)).Returns(new Visit { PatientId = patId, DoctorId = docId });

        IActionResult actionResult = _controller.CreateNewVisit(patId, docId);

        var contentResult = actionResult as OkObjectResult;
        // Don't need to incorporate test that tests domain model type
        // because of next line that casts a Visit model
        var visitObject = contentResult.Value as Visit;

        Assert.That(contentResult.StatusCode, Is.EqualTo(200));
        Assert.That(visitObject.PatientId, Is.EqualTo(patId));
        Assert.That(visitObject.DoctorId, Is.EqualTo(docId));
    }


    // GetAllVisitsByPatient
    [Test]
    public void GetAllVisitsByPatient_CallsRepositoryFunction_VerifyCorrectFunctionIsCalled()
    {
        visits.Add(new Visit { VisitId = 1 });
        visits.Add(new Visit { VisitId = 2 });
        visits.Add(new Visit { VisitId = 3 });

        IEnumerable<Visit> visitTable = visits;

        _mockRepo.Setup(x => x.GetAllVisitsByPatient(42)).Returns(visitTable);

        IActionResult actionResult = _controller.GetAllVisitsByPatient(42);

        _mockRepo.Verify(x => x.GetAllVisitsByPatient(42), Times.Once());
    }

    [Test]
    public void GetAllVisitsByPatient_PatientNotFound_Throw404()
    {
        _mockRepo.Setup(x => x.GetAllVisitsByPatient(-1)).Returns((IEnumerable<Visit>)null);

        IActionResult actionResult = _controller.GetAllVisitsByPatient(-1);

        Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public void GetAllVisitsByPatient_ValidPatientId_ReturnsListOfVisits()
    {
        visits.Add(new Visit { VisitId = 1 });
        visits.Add(new Visit { VisitId = 2 });
        visits.Add(new Visit { VisitId = 3 });

        IEnumerable<Visit> visitTable = visits;

        _mockRepo.Setup(x => x.GetAllVisitsByPatient(42)).Returns(visitTable);

        IActionResult actionResult = _controller.GetAllVisitsByPatient(42);

        var contentResult = actionResult as OkObjectResult;
        var visitObject = contentResult.Value as IEnumerable<Visit>;

        Assert.That(visitObject.Count, Is.EqualTo(3));
    }

    // GetAllVisitsByDoctor
    [Test]
    public void GetAllVisitsByDoctor_CallsRepositoryFunction_VerifyCorrectFunctionIsCalled()
    {
        visits.Add(new Visit { VisitId = 1 });
        visits.Add(new Visit { VisitId = 2 });
        visits.Add(new Visit { VisitId = 3 });

        IEnumerable<Visit> visitTable = visits;

        _mockRepo.Setup(x => x.GetAllVisitsByDoctor(42)).Returns(visitTable);

        IActionResult actionResult = _controller.GetAllVisitsByDoctor(42);

        _mockRepo.Verify(x => x.GetAllVisitsByDoctor(42), Times.Once());
    }

    [Test]
    public void GetAllVisitsByDoctor_DoctorNotFound_Throw404()
    {
        _mockRepo.Setup(x => x.GetAllVisitsByDoctor(-1)).Returns((IEnumerable<Visit>)null);

        IActionResult actionResult = _controller.GetAllVisitsByDoctor(-1);

        Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public void GetAllVisitsByDoctor_ValidDoctorId_ReturnsListOfVisits()
    {
        visits.Add(new Visit { VisitId = 1 });
        visits.Add(new Visit { VisitId = 2 });
        visits.Add(new Visit { VisitId = 3 });

        IEnumerable<Visit> visitTable = visits;

        _mockRepo.Setup(x => x.GetAllVisitsByDoctor(42)).Returns(visitTable);

        IActionResult actionResult = _controller.GetAllVisitsByDoctor(42);

        var contentResult = actionResult as OkObjectResult;
        var visitObject = contentResult.Value as IEnumerable<Visit>;

        Assert.That(visitObject.Count, Is.EqualTo(3));
    }

    // GetAllVisitsByDoctorPatient
    [Test]
    [TestCase(1, 1)]
    [TestCase(1, 10)]
    [TestCase(10, 1)]
    [TestCase(10, 10)]
    public void GetAllVisitsByDoctorPatient_CallsRepositoryFunction_VerifyCorrectFunctionIsCalled(int docId, int patId)
    {
        visits.Add(new Visit { VisitId = 1, DoctorId = docId, PatientId = patId });
        visits.Add(new Visit { VisitId = 2, DoctorId = docId, PatientId = patId });
        visits.Add(new Visit { VisitId = 3, DoctorId = docId, PatientId = patId });

        IEnumerable<Visit> visitTable = visits;

        _mockRepo.Setup(x => x.GetAllVisitsByDoctorPatient(docId, patId)).Returns(visitTable);

        IActionResult actionResult = _controller.GetAllVisitsByDoctorPatient(docId, patId);

        _mockRepo.Verify(x => x.GetAllVisitsByDoctorPatient(docId, patId), Times.Once());
    }

    [Test]
    [TestCase(-1, 5)]
    [TestCase(1, 1)]
    [TestCase(10000, 5)]
    [TestCase(5, -1)]
    [TestCase(5, 10000)]
    [TestCase(10000, 10000)]
    public void GetAllVisitsByDoctorPatient_DoctorNotFound_Throw404(int docId, int patId)
    {
        _mockRepo.Setup(x => x.GetAllVisitsByDoctorPatient(docId, patId)).Returns((IEnumerable<Visit>)null);

        IActionResult actionResult = _controller.GetAllVisitsByDoctorPatient(docId, patId);

        Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    [TestCase(1, 1)]
    [TestCase(1, 10)]
    [TestCase(10, 1)]
    [TestCase(10, 10)]
    public void GetAllVisitsByDoctorPatient_ValidDoctorIdAndPatientId_ReturnsListOfVisits(int docId, int patId)
    {
        visits.Add(new Visit { VisitId = 1, DoctorId = docId, PatientId = patId });
        visits.Add(new Visit { VisitId = 2, DoctorId = docId, PatientId = patId });
        visits.Add(new Visit { VisitId = 3, DoctorId = docId, PatientId = patId });

        IEnumerable<Visit> visitTable = visits;

        _mockRepo.Setup(x => x.GetAllVisitsByDoctorPatient(docId, patId)).Returns(visitTable);

        IActionResult actionResult = _controller.GetAllVisitsByDoctorPatient(docId, patId);

        var contentResult = actionResult as OkObjectResult;
        var visitObject = contentResult.Value as IEnumerable<Visit>;

        Assert.That(visitObject.Count, Is.EqualTo(3));
    }

    // DeleteVisit
    [Test]
    public void DeleteVisit_CallsRepositoryFunction_VerifyCorrectFunctionIsCalled()
    {
        _mockRepo.Setup(x => x.DeleteVisit(42)).Returns(new Visit { VisitId = 42 });

        IActionResult actionResult = _controller.DeleteVisit(42);

        _mockRepo.Verify(x => x.DeleteVisit(42), Times.Once());
    }

    [Test]
    public void DeleteVisit_VisitNotFound_Throw404()
    {
        _mockRepo.Setup(x => x.DeleteVisit(-1)).Returns((Visit)null);

        IActionResult actionResult = _controller.DeleteVisit(-1);

        Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public void DeleteVisit_ValidVisitId_ReturnsOkResultWithVisit()
    {
        _mockRepo.Setup(x => x.DeleteVisit(42)).Returns(new Visit { VisitId = 42 });

        IActionResult actionResult = _controller.DeleteVisit(42);

        var contentResult = actionResult as OkObjectResult;
        // Don't need to incorporate test that tests domain model type
        // because of next line that casts a doctor model
        var visitObject = contentResult.Value as Visit;

        Assert.That(contentResult.StatusCode, Is.EqualTo(200));
        Assert.That(visitObject.VisitId, Is.EqualTo(42));
    }

}

