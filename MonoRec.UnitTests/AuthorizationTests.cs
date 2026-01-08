using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonoRec.Controllers;
using MonoRec.Models;
using MonoRec.Repositories;
using Moq;

namespace MonoRec.UnitTests;

[TestFixture]
public class AuthorizationTests
{
    // Test that authorization attributes are present on endpoints
    // This ensures the authorization policy is declared, even though
    // we can't test the runtime enforcement without integration tests

    [Test]
    public void DoctorController_GetAllDoctors_HasPatientRoleAuthorization()
    {
        // Arrange
        var method = typeof(DoctorController).GetMethod("GetAllDoctors");

        // Act
        var authorizeAttribute = method?.GetCustomAttributes(typeof(AuthorizeAttribute), false)
            .FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "GetAllDoctors should have Authorize attribute");
        Assert.That(authorizeAttribute?.Roles, Is.EqualTo("Patient"));
    }

    [Test]
    public void DoctorController_GetAllPatientsByDoctor_HasDoctorRoleAuthorization()
    {
        // Arrange
        var method = typeof(DoctorController).GetMethod("GetAllPatientsByDoctor");

        // Act
        var authorizeAttribute = method?.GetCustomAttributes(typeof(AuthorizeAttribute), false)
            .FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "GetAllPatientsByDoctor should have Authorize attribute");
        Assert.That(authorizeAttribute?.Roles, Is.EqualTo("Doctor"));
    }

    [Test]
    public void PatientController_GetAllPatients_HasDoctorRoleAuthorization()
    {
        // Arrange
        var method = typeof(PatientController).GetMethod("GetAllPatients");

        // Act
        var authorizeAttribute = method?.GetCustomAttributes(typeof(AuthorizeAttribute), false)
            .FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "GetAllPatients should have Authorize attribute");
        Assert.That(authorizeAttribute?.Roles, Is.EqualTo("Doctor"));
    }

    [Test]
    public void PatientController_GetAllDoctorsByPatient_HasPatientOrDoctorRoleAuthorization()
    {
        // Arrange
        var method = typeof(PatientController).GetMethod("GetAllDoctorsByPatient");

        // Act
        var authorizeAttribute = method?.GetCustomAttributes(typeof(AuthorizeAttribute), false)
            .FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "GetAllDoctorsByPatient should have Authorize attribute");
        Assert.That(authorizeAttribute?.Roles, Is.EqualTo("Patient,Doctor"));
    }

    [Test]
    public void VisitController_GetVisit_HasPatientOrDoctorRoleAuthorization()
    {
        // Arrange
        var method = typeof(VisitController).GetMethod("GetVisit");

        // Act
        var authorizeAttribute = method?.GetCustomAttributes(typeof(AuthorizeAttribute), false)
            .FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "GetVisit should have Authorize attribute");
        Assert.That(authorizeAttribute?.Roles, Is.EqualTo("Patient,Doctor"));
    }

    [Test]
    public void VisitController_CreateNewVisit_HasDoctorRoleAuthorization()
    {
        // Arrange
        var method = typeof(VisitController).GetMethod("CreateNewVisit");

        // Act
        var authorizeAttribute = method?.GetCustomAttributes(typeof(AuthorizeAttribute), false)
            .FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "CreateNewVisit should have Authorize attribute");
        Assert.That(authorizeAttribute?.Roles, Is.EqualTo("Doctor"));
    }

    [Test]
    public void VisitController_DeleteVisit_HasDoctorRoleAuthorization()
    {
        // Arrange
        var method = typeof(VisitController).GetMethod("DeleteVisit");

        // Act
        var authorizeAttribute = method?.GetCustomAttributes(typeof(AuthorizeAttribute), false)
            .FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "DeleteVisit should have Authorize attribute");
        Assert.That(authorizeAttribute?.Roles, Is.EqualTo("Doctor"));
    }

    [Test]
    public void VisitController_GetAllVisitsByPatient_HasPatientOrDoctorRoleAuthorization()
    {
        // Arrange
        var method = typeof(VisitController).GetMethod("GetAllVisitsByPatient");

        // Act
        var authorizeAttribute = method?.GetCustomAttributes(typeof(AuthorizeAttribute), false)
            .FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "GetAllVisitsByPatient should have Authorize attribute");
        Assert.That(authorizeAttribute?.Roles, Is.EqualTo("Patient,Doctor"));
    }

    [Test]
    public void DoctorController_GetDoctor_NoAuthorizationRequired()
    {
        // Arrange
        var method = typeof(DoctorController).GetMethod("GetDoctor");

        // Act
        var authorizeAttribute = method?.GetCustomAttributes(typeof(AuthorizeAttribute), false)
            .FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.That(authorizeAttribute, Is.Null, "GetDoctor should not require authorization");
    }

    [Test]
    public void PatientController_GetPatient_NoAuthorizationRequired()
    {
        // Arrange
        var method = typeof(PatientController).GetMethod("GetPatient");

        // Act
        var authorizeAttribute = method?.GetCustomAttributes(typeof(AuthorizeAttribute), false)
            .FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.That(authorizeAttribute, Is.Null, "GetPatient should not require authorization");
    }

    [Test]
    public void VisitController_GetAllVisits_NoAuthorizationRequired()
    {
        // Arrange
        var method = typeof(VisitController).GetMethod("GetAllVisits");

        // Act
        var authorizeAttribute = method?.GetCustomAttributes(typeof(AuthorizeAttribute), false)
            .FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.That(authorizeAttribute, Is.Null, "GetAllVisits should not require authorization");
    }

    [Test]
    public void DoctorController_CreateNewDoctor_NoAuthorizationRequired()
    {
        // Arrange
        var method = typeof(DoctorController).GetMethod("CreateNewDoctor");

        // Act
        var authorizeAttribute = method?.GetCustomAttributes(typeof(AuthorizeAttribute), false)
            .FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.That(authorizeAttribute, Is.Null, "CreateNewDoctor should not require authorization");
    }

    [Test]
    public void PatientController_CreateNewPatient_NoAuthorizationRequired()
    {
        // Arrange
        var method = typeof(PatientController).GetMethod("CreateNewPatient");

        // Act
        var authorizeAttribute = method?.GetCustomAttributes(typeof(AuthorizeAttribute), false)
            .FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.That(authorizeAttribute, Is.Null, "CreateNewPatient should not require authorization");
    }

    [Test]
    public void VisitController_GetAllVisitsByDoctor_NoAuthorizationRequired()
    {
        // Arrange
        var method = typeof(VisitController).GetMethod("GetAllVisitsByDoctor");

        // Act
        var authorizeAttribute = method?.GetCustomAttributes(typeof(AuthorizeAttribute), false)
            .FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.That(authorizeAttribute, Is.Null, "GetAllVisitsByDoctor should not require authorization");
    }
}
