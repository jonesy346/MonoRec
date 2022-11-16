﻿using System;
using MonoRec.Data;
using MonoRec.Models;
using System.Security.Claims;
using System.Runtime.Intrinsics.Arm;
using System.IO;

namespace MonoRec.Repositories
{
    public class VisitRepository : IVisitRepository
    {
        MonoRecDbContext _db;

        public VisitRepository(MonoRecDbContext db)
        {
            _db = db;
        }

        public IEnumerable<Visit> GetAllVisits()
        {
            var visits = _db.Visits.ToList();
            return visits;
        }

        public Visit? GetVisit(int VisitId)
        {
            var visit = _db.Visits.FirstOrDefault(visit => visit.VisitId == VisitId);

            if (visit == null) return null;

            return visit;
        }

        public Visit CreateNewVisit(int patId, int docId)
        {
            var newVisit = new Visit(patId, docId);
            _db.Visits.Add(newVisit);
            _db.SaveChanges();
            return newVisit;
        }

        public IEnumerable<Visit>? GetAllVisitsByPatient(int patId)
        {
            var patient = _db.Patients.FirstOrDefault(patient => patient.PatientId == patId);

            if (patient == null) return null;

            var visits = _db.Visits.ToList();
            var selectResult = from visit in visits
                               where visit.PatientId == patId
                               select visit;

            return selectResult;
        }

        public IEnumerable<Visit>? GetAllVisitsByDoctor(int docId)
        {
            var doctor = _db.Doctors.FirstOrDefault(doctor => doctor.DoctorId == docId);

            if (doctor == null) return null;

            var visits = _db.Visits.ToList();
            var selectResult = from visit in visits
                               where visit.DoctorId == docId
                               select visit;

            return selectResult;
        }

        public IEnumerable<Visit>? GetAllVisitsByDoctorPatient(int docId, int patId)
        {
            var doctor = _db.Doctors.FirstOrDefault(doctor => doctor.DoctorId == docId);
            var patient = _db.Patients.FirstOrDefault(patient => patient.PatientId == patId);

            if (doctor == null || patient == null) return null;

            var visits = _db.Visits.ToList();
            var selectResult = from visit in visits
                               where visit.DoctorId == docId && visit.PatientId == patId
                               select visit;

            return selectResult;
        }

        public Visit? DeleteVisit(int VisitId)
        {
	        var visitToDelete = _db.Visits.FirstOrDefault(visit => visit.VisitId == VisitId);

            if (visitToDelete == null) return null;

            _db.Visits.Remove(visitToDelete);
            _db.SaveChanges();
            return visitToDelete;
        }

    }
}
