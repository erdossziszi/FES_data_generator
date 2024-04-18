using FES_data_generator.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FES_data_generator.Utils
{
    internal class ExamToDzn
    {
        Exam exam;
        AllConstraints allConstraints;

        public ExamToDzn(Exam Exam, AllConstraints AllConstraints)
        {
            exam = Exam;
            allConstraints = AllConstraints;
        }

        public void SerializeToDzn(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {

                sw.WriteLine("StudentsNr = {0};", exam.StudentsNr);
                sw.WriteLine("InstructorsNr = {0};", exam.InstructorsNr);
                sw.WriteLine("DaysNr = {0};", exam.DaysNr);
                sw.WriteLine("SlotsPerDay = {0};", exam.SlotsPerDay);
                sw.WriteLine("AvailabilitySlotsLenght = {0};", exam.AvailabilitySlotsLenght);
                sw.WriteLine("RoomNr = {0};", exam.RoomNr);
                sw.WriteLine("ProgrammNr = {0};", exam.ProgrammNr);
                sw.WriteLine("DegreeNr = {0};", exam.DegreeNr);
                sw.WriteLine("RolesNr = {0};", exam.RolesNr);
                sw.WriteLine("CoursesNr = {0};", exam.CoursesNr);

                string instructorsProgramm = ToArrayOfSets(exam.Instructors, "Programm");
                sw.WriteLine("InstructorsProgramm = {0};", instructorsProgramm);

                string instructorRolesPerProgramm = To2DArrayOfSets(exam.InstructorRolesPerProgramm);
                sw.WriteLine("InstructorRolesPerProgramm = {0};", instructorRolesPerProgramm);

                string instructorsAvailability = To2DArray(exam.Instructors, "Availability");
                sw.WriteLine("InstructorsAvailability = {0};", instructorsAvailability);

                string studentsProgramm = ToArray(exam.Students, "Programm");
                sw.WriteLine("StudentsProgramm = {0};", studentsProgramm);

                string studentsDegree = ToArray(exam.Students, "Degree");
                sw.WriteLine("StudentsDegree = {0};", studentsDegree);

                string studentsSupervisorId = ToArray(exam.Students, "SupervisorId");
                sw.WriteLine("StudentsSupervisorId = {0};", studentsSupervisorId);

                string studentsCourseIds = ToArrayOfSets(exam.Students, "CourseIds");
                sw.WriteLine("StudentsCourseIds = {0};", studentsCourseIds);

                string studentsAvailability = To2DArray(exam.Students, "Availability");
                sw.WriteLine("StudentsAvailability = {0};", studentsAvailability);

                string theoryticalMinCard = ToArray(exam.Students, "TheoryticalMinCard");
                sw.WriteLine("TheoryticalMinCard = {0};", theoryticalMinCard);

                string coursesInstructorIds = ToArrayOfSets(exam.Courses, "InstructorIds");
                sw.WriteLine("CoursesInstructorIds = {0};", coursesInstructorIds);

                if (allConstraints.RolesDemands is not null) sw.WriteLine($"RolesDemands = {allConstraints.RolesDemands};");
                if (allConstraints.RolesContinuity is not null) sw.WriteLine($"RolesContinuity = {allConstraints.RolesContinuity};");
                if (allConstraints.ExamLen is not null) sw.WriteLine($"ExamLen = {allConstraints.ExamLen};");
                if (allConstraints.LunchTsMinLen is not null) sw.WriteLine($"LunchTsMinLen = {allConstraints.LunchTsMinLen};");
                if (allConstraints.LunchStarts is not null) sw.WriteLine($"LunchStarts = {allConstraints.LunchStarts};");
                if (allConstraints.SupervisorAvailable is not null) sw.WriteLine($"SupervisorAvailable = {allConstraints.SupervisorAvailable};");
                if (allConstraints.OptimalLunchLenght is not null) sw.WriteLine($"OptimalLunchLenght = {allConstraints.OptimalLunchLenght};");
                if (allConstraints.OptimalStartTs is not null) sw.WriteLine($"OptimalStartTs = {allConstraints.OptimalStartTs};");
                if (allConstraints.OptimalFinishTs is not null) sw.WriteLine($"OptimalFinishTs = {allConstraints.OptimalFinishTs};");
                if (allConstraints.MinimizeRooms is not null) sw.WriteLine($"MinimizeRooms = {allConstraints.MinimizeRooms};");
                if (allConstraints.RolesSoftContinuity is not null) sw.WriteLine($"RolesSoftContinuity = {allConstraints.RolesSoftContinuity};");
                if (allConstraints.SameDegreeInRoom is not null) sw.WriteLine($"SameDegreeInRoom = {allConstraints.SameDegreeInRoom};");
                if (allConstraints.Mergeability is not null) sw.WriteLine($"Mergeability = {allConstraints.Mergeability};");

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

        private string To2DArrayOfSets(List<int>[,] property)
        {
            string result = "[";
            for (int row = 0; row < property.GetLength(0); row++)
            {
                result += "|";
                for (int col = 0; col < property.GetLength(1); col++)
                {
                    if (col > 0) result += ", ";
                    result += "{";
                    result += string.Join(", ", property[row, col]);
                    result += "}";
                }
            }
            result += "|]";

            return result;
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
