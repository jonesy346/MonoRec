using System;
namespace MonoRec.Models;

public class DoctorPatient
{
    public int DoctorPatientId { get; set; }
    public int DoctorId { get; set; }
    public int PatientId { get; set; }

    public DoctorPatient() { }
    
    public DoctorPatient(int docId, int patId)
    {
        DoctorId = docId;
        PatientId = patId;
	}
}

