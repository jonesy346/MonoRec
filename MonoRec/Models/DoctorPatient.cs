namespace MonoRec.Models;

public class DoctorPatient
{
    public int DoctorPatientId { get; set; }
    public int DoctorId { get; set; }
    public int PatientId { get; set; }

    public DoctorPatient() { }

    public DoctorPatient(int doctorId, int patientId)
    {
        DoctorId = doctorId;
        PatientId = patientId;
    }
}
