using FES_data_generator.Model;
using FES_data_generator.Utils;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FES_data_generator
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Random r = new Random();

            for (int j = 0; j < 10; j++)
            {
                Exam testExam = new Exam()
                {
                    StudentsNr = 5,//r.Next(5, 301),
                    InstructorsNr = 10,//r.Next(5, 101),
                    DaysNr = 1,//r.Next(1, 16), //TODO: Feasibility check
                    SlotsPerDay = 10,//r.Next(10, 145),
                    RoomNr = 1,//r.Next(1, 6),
                    ProgrammNr = 2,//r.Next(1, 5),
                    DegreeNr = 1,//r.Next(1, 4),
                    RolesNr = 1,//r.Next(5),  //nullable
                    CoursesNr = 3,//r.Next(30) //nullable
                };

                GenerateInstructors(r, testExam);
                GenerateStudents(r, testExam);
                GenerateCourses(r, testExam);

                AllConstraints testConstraints = new AllConstraints()
                {
                    OneExamPerRoom = new Constraint(true),
                    SupervisorAvailable = new Constraint(5),
                    OptimalLunchLenght = new ConstraintWithParameter(8, false),
                    MergeableRolesWithExaminer = [new ConstraintWithParameter(0, 2), new ConstraintWithParameter(1, 3)]
                };

                ExamAllData testExamAllData = new ExamAllData()
                {
                    Exam = testExam,
                    AllConstraints = testConstraints
                };

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                string fileName = @"..\..\..\DZN files\" + testExam.StudentsNr + "_" + testExam.InstructorsNr + "_" + j + ".dzn";
                //string json = JsonSerializer.Serialize(testExamAllData, options);
                //Console.WriteLine(json);
                //File.WriteAllText(@"..\..\..\JSON files\" + testExam.StudentsNr + "_" + testExam.InstructorsNr + "_" + j + ".json", json);
                var examToDzn = new ExamToDzn(testExam);
                examToDzn.SerializeToDzn(fileName);

            }
        }

        private static void GenerateInstructors(Random r, Exam testExam)
        {
            testExam.Instructors = new Instructor[testExam.InstructorsNr];
            for (int i = 0; i < testExam.InstructorsNr; i++)
            {
                var newInstructor = new Instructor()
                {
                    Id = i,
                    Programm = Enumerable.Range(0, r.Next(1, testExam.ProgrammNr + 1)).Select(_ => r.Next(testExam.ProgrammNr)).Distinct().Order().ToArray(),
                    Roles = Enumerable.Range(0, r.Next(testExam.RolesNr + 1)).Select(_ => r.Next(testExam.RolesNr)).Distinct().Order().ToArray(),
                    Availability = GenerateSmartAvailability(r, testExam, 0.8, 0.1, 0.3, 0.1)
                };
                testExam.Instructors[i] = newInstructor;
            }
        }

        private static void GenerateStudents(Random r, Exam testExam)
        {
            int[] coursesPerDegree = Enumerable.Range(0, testExam.DegreeNr).Select(_ => r.Next(Math.Max(testExam.CoursesNr + 1, 4))).ToArray();
            testExam.Students = new Student[testExam.StudentsNr];
            for (int i = 0; i < testExam.StudentsNr; i++)
            {
                var newStudent = new Student()
                {
                    Id = i,
                    Programm = r.Next(testExam.ProgrammNr),
                    Degree = r.Next(testExam.DegreeNr),
                    SupervisorId = r.Next(testExam.InstructorsNr),
                    Availability = GenerateSmartAvailability(r,testExam, 0.9, 0.1, 0.4, 0.1)
                };
                newStudent.CourseIds = Enumerable.Range(0, testExam.CoursesNr).OrderBy(_ => Guid.NewGuid()).Take(coursesPerDegree[newStudent.Degree]).Order().ToArray();
                testExam.Students[i] = newStudent;
            }
        }

        private static void GenerateCourses(Random r, Exam testExam)
        {
            testExam.Courses = new Course[testExam.CoursesNr];
            for (int i = 0; i < testExam.CoursesNr; i++)
            {
                var newCourse = new Course()
                {
                    Id = i,
                    // TODO: more realistic headcount (less instructors)
                    InstructorIds = Enumerable.Range(0, r.Next(1, testExam.InstructorsNr + 1)).Select(_ => r.Next(testExam.InstructorsNr)).Distinct().Order().ToArray(),
                };
                testExam.Courses[i] = newCourse;
            }
        }

        private static int[] GenerateSmartAvailability(Random r, Exam testExam, double Tmean, double Tvar, double Fmean, double Fvar)
        {
            double trueToTrue = GenerateRandomDouble(Tmean - Tvar, Tmean + Tvar);
            double falseToTrue = GenerateRandomDouble(Fmean - Fvar, Fmean + Fvar);
            var chain = new MarkovChain(trueToTrue, falseToTrue);
            return chain.GenerateStates(r.Next(2), testExam.DaysNr * testExam.SlotsPerDay);
        }

        public static double GenerateRandomDouble(double min, double max)
        {
            Random random = new Random();
            return random.NextDouble() * (max - min) + min;
        }
    }
}
