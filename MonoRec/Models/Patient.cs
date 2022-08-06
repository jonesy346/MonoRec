namespace MonoRec.Models;

public class Patient
{
    public int PatientId { get; set; }
    public string? PatientEmail { get; set; }
    private string? _patientPassword;
    public string PatientName { get; set; }
    public Gender? PatientSex { get; set; }
    public int PatientAge { get; set; }
    public IList<Doctor>? Doctors { get; set; }
    public IList<Visit>? Visits { get; set; }

    public Patient() { }

    public Patient(string Name, int Age)
    {
        PatientName = Name;
        PatientAge = Age;
    }

    
}


