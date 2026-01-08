namespace MonoRec.Models;

public class Patient
{
    public int PatientId { get; set; }
    public string PatientName { get; set; }

    // Link to Identity user - stores the ApplicationUser.Id
    // This creates a relationship between the Patient entity and their login account
    public string? UserId { get; set; }

    //public string? PatientEmail { get; set; }
    //private string? _patientPassword;
    //public Gender? PatientSex { get; set; }
    //public int PatientAge { get; set; }
    //public IList<Doctor>? Doctors { get; set; }
    //public IList<Visit>? Visits { get; set; }

    public Patient() { }

    public Patient(string Name)
    {
        PatientName = Name;
        //PatientAge = Age;
    }

    
}


