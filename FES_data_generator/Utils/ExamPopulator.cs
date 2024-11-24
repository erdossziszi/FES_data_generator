using FES_data_generator.Model;

namespace FES_data_generator.Utils;

public class ExamPopulator(Exam exam, Random random)
{
    private readonly Dictionary<int, HashSet<int>> _instructorsOfProgramms = Enumerable.Range(1, exam.NumberOfProgrammes)
        .ToDictionary(programmeId => programmeId, _ => new HashSet<int>());

    public void PopulateInstructors()
    {
        exam.Instructors = new Instructor[exam.NumberOfInstructors];
        for (int i = 0; i < exam.NumberOfInstructors; i++)
        {
            var newInstructor = new Instructor()
            {
                Id = i + 1,
                Programm = Enumerable.Range(0, random.Next(1, exam.NumberOfProgrammes + 1))
                    .Select(_ => random.Next(1, exam.NumberOfProgrammes + 1))
                    .Distinct()
                    .Order()
                    .ToArray(),
                Roles = Enumerable.Range(0, random.Next(exam.NumberOfRoles + 1))
                    .Select(_ => random.Next(1, exam.NumberOfRoles + 1))
                    .Distinct()
                    .Order()
                    .ToArray(),
                Availability = GenerateSmartAvailability(0.85, 0.1, 0.6, 0.1)
            };

            exam.Instructors[i] = newInstructor;
            foreach (var p in newInstructor.Programm)
            {
                _instructorsOfProgramms[p].Add(newInstructor.Id);
            }
        }
    }

    public void PopulateStudents()
    {
        int[] coursesPerDegree = Enumerable.Range(0, exam.NumberOfDegrees)
            .Select(_ => random.Next(Math.Max(exam.NumberOfCourses + 1, 4)))
            .ToArray();
        exam.Students = new Student[exam.NumberOfStudents];
        for (int i = 0; i < exam.NumberOfStudents; i++)
        {
            var newStudent = new Student()
            {
                Id = i + 1,
                Programm = random.Next(1, exam.NumberOfProgrammes + 1),
                Degree = random.Next(1, exam.NumberOfDegrees + 1),
                Availability = GenerateSmartAvailability(0.9, 0.1, 0.7, 0.1)
            };

            var instructorSet = _instructorsOfProgramms[newStudent.Programm];
            // If there's no instructor on the programme, we choose a random from the whole pool
            newStudent.SupervisorId = instructorSet.Count > 0
                ? instructorSet.ElementAt(random.Next(instructorSet.Count))
                : exam.Instructors[random.Next(exam.Instructors.Length)].Id;

            newStudent.CourseIds = Enumerable.Range(1, exam.NumberOfCourses)
                .OrderBy(_ => Guid.NewGuid())
                .Take(coursesPerDegree[newStudent.Degree - 1])
                .Order()
                .ToArray();

            int minCard = 0;
            var allCourseInstructors = new List<int>();
            var rolesInstructors = Enumerable.Range(0, exam.InstructorRolesPerProgramme!.GetLength(1))
                             .Select(x => exam.InstructorRolesPerProgramme[newStudent.Programm - 1, x]).ToArray()
                             .SelectMany(list => list).ToArray();

            foreach (int courseId in newStudent.CourseIds)
            {
                int[] courseInstructors = exam.Courses[courseId - 1].InstructorIds;
                allCourseInstructors.AddRange(courseInstructors);
                if (!courseInstructors.Intersect(rolesInstructors).Any()) minCard++;
            }
            if (!allCourseInstructors.Any(x => x == newStudent.SupervisorId) && !rolesInstructors.Any(x => x == newStudent.SupervisorId)) minCard++;

            newStudent.TheoryticalMinCard = minCard;
            exam.Students[i] = newStudent;
        }
    }

    public void PopulateCourses()
    {
        exam.Courses = new Course[exam.NumberOfCourses];
        for (int i = 0; i < exam.NumberOfCourses; i++)
        {
            var newCourse = new Course()
            {
                Id = i + 1,
                InstructorIds = Enumerable.Range(0, random.Next(1, exam.NumberOfInstructors + 1))
                    .Select(_ => random.Next(1, exam.NumberOfInstructors + 1))
                    .Distinct()
                    .Order()
                    .ToArray(),
            };
            exam.Courses[i] = newCourse;
        }
    }

    public void PopulateInstructorRolesPerProgramme()
    {
        exam.InstructorRolesPerProgramme = new List<int>[exam.NumberOfProgrammes, exam.NumberOfRoles];

        for (int i = 0; i < exam.NumberOfProgrammes; i++)
        {
            for (int j = 0; j < exam.NumberOfRoles; j++)
            {
                exam.InstructorRolesPerProgramme[i, j] = new List<int>();
            }
        }

        foreach (var instructor in exam.Instructors)
        {
            if (instructor.Programm is null || instructor.Roles is null) continue;

            foreach (var program in instructor.Programm)
            {
                foreach (var role in instructor.Roles)
                {
                    exam.InstructorRolesPerProgramme[program - 1, role - 1].Add(instructor.Id);
                }
            }
        }
    }

    private int[] GenerateSmartAvailability(double Tmean, double Tvar, double Fmean, double Fvar)
    {
        double trueToTrue = GenerateRandomDouble(Tmean - Tvar, Tmean + Tvar);
        double falseToTrue = GenerateRandomDouble(Fmean - Fvar, Fmean + Fvar);
        var chain = new MarkovChain(trueToTrue, falseToTrue);
        var initState = random.NextDouble() < ((Tmean + Fmean) / 2.0) ? 1 : 0;
        return chain.GenerateStates(initState, exam.NumberOfDays * exam.SlotsPerDay / exam.AvailabilitySlotsLenght);
    }

    private double GenerateRandomDouble(double min, double max)
    {
        return random.NextDouble() * (max - min) + min;
    }
}
