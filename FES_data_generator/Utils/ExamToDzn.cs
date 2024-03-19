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
        string dzn;
        public ExamToDzn(Exam Exam)
        {
            exam = Exam;
        }

        public string SerializeToDzn(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                // Sablon kitöltése
                sw.WriteLine("StudentsNr = {0};", exam.StudentsNr);
                sw.WriteLine("InstructorsNr = {0};", exam.InstructorsNr);
                sw.WriteLine("DaysNr = {0};", exam.DaysNr);
                sw.WriteLine("SlotsPerDay = {0};", exam.SlotsPerDay);
                sw.WriteLine("RoomNr = {0};", exam.RoomNr);
                sw.WriteLine("ProgrammNr = {0};", exam.ProgrammNr);
                sw.WriteLine("DegreeNr = {0};", exam.DegreeNr);
                sw.WriteLine("RolesNr = {0};", exam.RolesNr);
                sw.WriteLine("CoursesNr = {0};", exam.CoursesNr);

                // Tömbök kezelése
                var InstructorsRoles = "InstructorsRoles = [";
                var InstructorsAvailability = "InstructorsAvailability = [";

                string instructorsProgramm = ToArrayOfSets(exam.Instructors, "Programm");
                sw.WriteLine("InstructorsProgramm = {0};", instructorsProgramm);
                Console.WriteLine(instructorsProgramm);

                /*sw.WriteLine("SimultaneousSessions = [|");
                for (int i = 0; i < exam.DaysNr; i++)
                {
                    for (int j = 0; j < exam.SlotsPerDay; j++)
                    {
                        sw.Write("{0}, {1}|", i * exam.SlotsPerDay + j + 1, i * exam.SlotsPerDay + j + 2);
                    }
                }
                sw.WriteLine("]");

                sw.WriteLine("AcademicLevel = [");
                for (int i = 0; i < exam.StudentsNr; i++)
                {
                    sw.Write("{0}, ", exam.DegreeNr);
                }
                sw.WriteLine("]");*/

                sw.Close();
            }


            return dzn;
        }

        private string ToArrayOfSets()
        {
            return "[" + string.Join(", ", exam.Instructors.Where(i => i != null).Select(i => $"{{{string.Join(", ", i.Programm)}}}")) + "]";
        }

        private string ToArrayOfSets(IEnumerable<object> instructors, string programPropertyName)
        {
            return "[" + string.Join(", ", instructors.Where(i => i != null)
                                                    .Select(i => $"{{{string.Join(", ", (int[])i.GetType().GetProperty(programPropertyName).GetValue(i))}}}")) + "]";
        }
    }
}
