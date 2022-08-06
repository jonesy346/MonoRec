namespace MonoRec.Models
{
    public class DoctorNote
    { 
        public int DoctorNoteId { get; set; }
        public int DoctorId { get; set; }
        public int VisitId { get; set; }
        public string Note { get; set; }
    }
}