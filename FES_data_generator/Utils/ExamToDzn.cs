using FES_data_generator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FES_data_generator.Utils
{
    internal class ExamToDzn
    {
        Exam exam;

        public ExamToDzn(Exam Exam)
        {
            exam = Exam;
        }

        public void SerializeToDzn(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {

                sw.WriteLine("StudentsNr = {0};", exam.StudentsNr);
                sw.WriteLine("InstructorsNr = {0};", exam.InstructorsNr);
                sw.WriteLine("DaysNr = {0};", exam.DaysNr);
                sw.WriteLine("SlotsPerDay = {0};", exam.SlotsPerDay);
                sw.WriteLine("RoomNr = {0};", exam.RoomNr);
                sw.WriteLine("ProgrammNr = {0};", exam.ProgrammNr);
                sw.WriteLine("DegreeNr = {0};", exam.DegreeNr);
                sw.WriteLine("RolesNr = {0};", exam.RolesNr);
                sw.WriteLine("CoursesNr = {0};", exam.CoursesNr);

                string instructorsId = ToEnum(exam.Instructors, "Id");
                sw.WriteLine("InstructorsId = {0};", instructorsId);

                string instructorsProgramm = ToArrayOfSets(exam.Instructors, "Programm");
                sw.WriteLine("InstructorsProgramm = {0};", instructorsProgramm);

                string instructorsRoles = ToArrayOfSets(exam.Instructors, "Roles");
                sw.WriteLine("InstructorsRoles = {0};", instructorsRoles);

                string instructorsAvailability = To2DArray(exam.Instructors, "Availability");
                sw.WriteLine("InstructorsAvailability = {0};", instructorsAvailability);

                sw.Close();
            }
        }


        private string ToArrayOfSets(IEnumerable<object> group, string listPropertyName)
        {
            return "[" + string.Join(", ", group.Where(i => i != null)
                                                .Select(i => $"{{{string.Join(", ", (int[])i.GetType().GetProperty(listPropertyName).GetValue(i))}}}")) + "]";
        }

        private string To2DArray(IEnumerable<object> group, string listPropertyName)
        {
            return "[" + string.Join(", ", group.Where(i => i != null)
                                                .Select(i => $"|{string.Join(", ", (int[])i.GetType().GetProperty(listPropertyName).GetValue(i))}")) + "|]";
        }

        private string ToArray(IEnumerable<object> group, string propertyName)
        {
            return "[" + string.Join(", ", group.Where(i => i != null)
                                                .Select(i => (int)i.GetType().GetProperty(propertyName).GetValue(i))) + "]";
        }

        private string ToEnum(IEnumerable<object> group, string propertyName)
        {
            return "{" + string.Join(", ", group.Where(i => i != null)
                                                .Select(i => (int)i.GetType().GetProperty(propertyName).GetValue(i))) + "}";
        }
    }
}
