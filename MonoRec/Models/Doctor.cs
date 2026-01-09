namespace MonoRec.Models;

public class Doctor
{
    public int DoctorId { get; set; }
    public string DoctorName { get; set; }

    // Link to Identity user - stores the ApplicationUser.Id
    // This creates a relationship between the Doctor entity and their login account
    public string? UserId { get; set; }

    public string? DoctorEmail { get; set; }
    //private string? _doctorPassword;
    //public Gender? DoctorSex { get; set; }
    //public int DoctorAge { get; set; }
    //public string? Specialty { get; set; }
    //public IList<Patient>? Patients { get; set; }
    //public IList<Visit>? Visits { get; set; }
    //public IList<DoctorNote>? DoctorNotes { get; set; }

    public Doctor() { }

    public Doctor(string Name)
    {
        DoctorName = Name;
        //PatientAge = Age;
    }


}


