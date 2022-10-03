using System;
using MonoRec.Data;
using MonoRec.Models;
using System.Security.Claims;
using System.Runtime.Intrinsics.Arm;

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

        public Visit GetVisit(int VisitId)
        {
            var visit = _db.Visits.Where(visit => visit.VisitId == VisitId).First();
            return visit;
        }

        public Visit CreateNewVisit(int PatientId, int DoctorId)
        {
            var newVisit = new Visit(PatientId, DoctorId);
            _db.Visits.Add(newVisit);
            _db.SaveChanges();
            return newVisit;
        }

        public IEnumerable<Visit> GetAllVisitsByPatient(int PatientId)
        {
            var visits = _db.Visits.ToList();
            var selectResult = from visit in visits
                               where visit.PatientId == PatientId
                               select visit;

            return selectResult;
        }

        public IEnumerable<Visit> GetAllVisitsByDoctor(int DoctorId)
        {
            var visits = _db.Visits.ToList();
            var selectResult = from visit in visits
                               where visit.DoctorId == DoctorId
                               select visit;

            return selectResult;
        }

        public IEnumerable<Visit> GetAllVisitsByDoctorPatient(int DoctorId, int PatientId)
        {
            var visits = _db.Visits.ToList();
            var selectResult = from visit in visits
                               where visit.DoctorId == DoctorId && visit.PatientId == PatientId
                               select visit;

            return selectResult;
        }

        public Visit DeleteVisit(int VisitId)
        {
            var visitToDelete = _db.Visits.Where(visit => visit.VisitId == VisitId).First();
            _db.Visits.Remove(visitToDelete);
            _db.SaveChanges();
            return visitToDelete;
        }

    }
}
