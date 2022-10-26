using MonoRec.Models;
using MonoRec.Repositories;
using MonoRec.Controllers;
using Moq;

namespace MonoRec.UnitTests;

[TestFixture]
public class PatientControllerTests
{
    PatientController controller;
    Mock<IPatientRepository> mockPatientRepository;
    IEnumerable<Patient> patients;

    [SetUp]
    public void Setup()
    {
        mockPatientRepository = new Mock<IPatientRepository>();
        controller = new PatientController(mockPatientRepository.Object);

        patients = new List<Patient>();

        patients.Add(new Patient())

        // player = new HangmanPlayer()
        //        .WithName("Jason")
        //        .WithWins(2)
        //        .WithCurrentWinStreak(1)
        //        .WithGamesPlayed(5);
        // playerWithId = new HangmanPlayer();
        // playerWithId.HangmanPlayerId = 1;

    }

    [Test]
    public void GetPatient_PatientNotFound_Throw404()
    {
        
        mockPatientRepository.Setup(x => x.GetPatient())

        var result = 
        
        Assert.Pass();
    }
}

