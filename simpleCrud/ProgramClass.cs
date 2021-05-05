using System.Text.Json;
using System.Text.Json.Serialization;

namespace simpleCrud
{
    public class ProgramClass
    {
        [JsonPropertyName("id")] public string Id { get; set; }
        public string ProgramName { get; set; }
        public Lecturer[] Lecturers { get; set; }
        public Student[] Students { get; set; }
        public Organisation Organisation { get; set; }
        public bool IsRegistered { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    public class Lecturer
    {
        public string ProgramName { get; set; }
        public string LecturersName { get; set; }
    }

    public class Student
    {
        public string ProgramName { get; set; }
        public string StudentsName { get; set; }
        public string Gender { get; set; }
        public double OverallGrade { get; set; }
        public double CreditsReceived { get; set; }
    }

    public class Organisation
    {
        public string University { get; set; }
        public string BelongingTo { get; set; }
        public string City { get; set; }
    }
}

