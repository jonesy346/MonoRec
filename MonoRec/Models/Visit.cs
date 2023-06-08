namespace MonoRec.Models;

public class Visit
{
    public int VisitId { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    //public DateTime VisitDate { get; set; }
    //public string? Symptoms { get; set; }
    //public string? Diagnosis { get; set; }


    public Visit() { }

    public Visit(int patId, int docId)
    {
        PatientId = patId;
        DoctorId = docId;
    }
    
}


