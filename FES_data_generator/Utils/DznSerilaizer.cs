using System.Data;
using FES_data_generator.Model;

namespace FES_data_generator.Utils;

public static class DznSerilaizer
{
    public static void SerializeExam(Exam exam, AllConstraints allConstraints, string fileName)
    {
        using var sw = new StreamWriter(fileName);

        sw.WriteLine("StudentsNr = {0};", exam.NumberOfStudents);
        sw.WriteLine("InstructorsNr = {0};", exam.NumberOfInstructors);
        sw.WriteLine("DaysNr = {0};", exam.NumberOfDays);
        sw.WriteLine("SlotsPerDay = {0};", exam.SlotsPerDay);
        sw.WriteLine("AvailabilitySlotsLenght = {0};", exam.AvailabilitySlotsLenght);
        sw.WriteLine("RoomNr = {0};", exam.NumberOfRooms);
        sw.WriteLine("ProgrammNr = {0};", exam.NumberOfProgrammes);
        sw.WriteLine("DegreeNr = {0};", exam.NumberOfDegrees);
        sw.WriteLine("RolesNr = {0};", exam.NumberOfRoles);
        sw.WriteLine("CoursesNr = {0};", exam.NumberOfCourses);

        string instructorsProgramm = ToArrayOfSets(exam.Instructors, "Programm");
        sw.WriteLine("InstructorsProgramm = {0};", instructorsProgramm);

        string instructorRolesPerProgramm = exam.NumberOfRoles > 0 ? To2DArrayOfSets(exam.InstructorRolesPerProgramme!) : "array2d(1..ProgrammNr,{},[])";
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
        if (allConstraints.ExamLength is not null) sw.WriteLine($"ExamLen = {allConstraints.ExamLength};");
        if (allConstraints.MinimumLunchTimeSlotLength is not null) sw.WriteLine($"LunchTsMinLen = {allConstraints.MinimumLunchTimeSlotLength};");
        if (allConstraints.LunchStarts is not null) sw.WriteLine($"LunchStarts = {allConstraints.LunchStarts};");
        if (allConstraints.SupervisorAvailable is not null) sw.WriteLine($"SupervisorAvailable = {allConstraints.SupervisorAvailable};");
        if (allConstraints.OptimalLunchLength is not null) sw.WriteLine($"OptimalLunchLenght = {allConstraints.OptimalLunchLength};");
        if (allConstraints.OptimalStartTs is not null) sw.WriteLine($"OptimalStartTs = {allConstraints.OptimalStartTs};");
        if (allConstraints.OptimalFinishTs is not null) sw.WriteLine($"OptimalFinishTs = {allConstraints.OptimalFinishTs};");
        if (allConstraints.MinimizeRooms is not null) sw.WriteLine($"MinimizeRooms = {allConstraints.MinimizeRooms};");
        string rolesSoftCont = exam.NumberOfRoles > 0 ? allConstraints.RolesSoftContinuity?.ToString() ?? string.Empty : "array2d(1..2,{},[])";
        if (allConstraints.RolesSoftContinuity is not null) sw.WriteLine($"RolesSoftContinuity = {rolesSoftCont};");
        if (allConstraints.SameDegreeInRoom is not null) sw.WriteLine($"SameDegreeInRoom = {allConstraints.SameDegreeInRoom};");
        if (allConstraints.Mergeability is not null) sw.WriteLine($"Mergeability = {allConstraints.Mergeability};");

        sw.Close();
    }


    private static string ToArrayOfSets(IEnumerable<object> group, string listPropertyName)
    {
        return "[" + string.Join(", ", group.Where(i => i != null)
                                            .Select(i => $"{{{string.Join(", ", (int[])(i!.GetType().GetProperty(listPropertyName)?.GetValue(i) ?? Array.Empty<int>()))}}}")) + "]";
    }

    private static string To2DArray(IEnumerable<object> group, string listPropertyName)
    {
        return "[" + string.Join(", ", group.Where(i => i != null)
                                            .Select(i => $"|{string.Join(", ", (int[])(i!.GetType().GetProperty(listPropertyName)?.GetValue(i) ?? Array.Empty<int>()))}")) + "|]";
    }

    private static string To2DArrayOfSets(List<int>[,] property)
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

    private static string ToArray(IEnumerable<object> group, string propertyName)
    {
        return "[" + string.Join(", ", group.Where(i => i != null)
                                            .Select(i => (int)i!.GetType()!.GetProperty(propertyName)!.GetValue(i)!)) + "]";
    }
}
