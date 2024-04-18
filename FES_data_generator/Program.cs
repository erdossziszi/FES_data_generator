using FES_data_generator.Model;
using FES_data_generator.Utils;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FES_data_generator
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Random r = new Random();

            for (int j = 0; j < 1; j++)
            {
                Exam testExam = new Exam()
                {
                    StudentsNr = 15,//r.Next(5, 301),
                    InstructorsNr = 10,//r.Next(5, 101),
                    DaysNr = 2,//r.Next(1, 16), //TODO: Feasibility check
                    SlotsPerDay = 10,//r.Next(10, 145),
                    AvailabilitySlotsLenght = 2, //must be dividable with SlotsPerDay
                    RoomNr = 2,//r.Next(1, 6),
                    ProgrammNr = 2,//r.Next(1, 5),
                    DegreeNr = 1,//r.Next(1, 4),
                    RolesNr = 2,//r.Next(5),  //nullable
                    CoursesNr = 3,//r.Next(30) //nullable
                };
                Dictionary<int, HashSet<int>> instructorsOfProgramms = new Dictionary<int, HashSet<int>>();
                for (int i = 1; i < testExam.ProgrammNr + 1; i++)
                {
                    instructorsOfProgramms.Add(i, new HashSet<int>());
                }

                GenerateInstructors(r, testExam, instructorsOfProgramms);
                GetInstructorRolesPerProgram(testExam);
                GenerateCourses(r, testExam);
                GenerateStudents(r, testExam, instructorsOfProgramms);

                Dictionary<int, int> degreeExamLenPairs = Enumerable.Range(1, testExam.DegreeNr).ToDictionary(degree => degree, degree => new Random().Next(1, 3));
                AllConstraints testConstraints = new AllConstraints()
                {
                    RolesDemands = new Constraint(true, Enumerable.Range(0, testExam.RolesNr).Select(x => r.Next(4)).ToArray()),
                    RolesContinuity = new Constraint(true, Enumerable.Range(1, testExam.RolesNr).OrderBy(x => r.Next()).Take(r.Next(testExam.RolesNr)).ToArray()),
                    ExamLen = new Constraint(true, testExam.Students.Select(student => degreeExamLenPairs[student.Degree]).ToArray()),
                    LunchTsMinLen = new Constraint(true, r.Next(3)),
                    LunchStarts = new Constraint(true, Enumerable.Range(1, testExam.DaysNr).ToDictionary(day => day, day => new int[] { 5 + (day - 1) * 10, 6 + (day - 1) * 10 })),
                    SupervisorAvailable = new Constraint(false, 5),
                    OptimalStartTs = new Constraint(false, [1, 2]),
                    OptimalFinishTs = new Constraint(false, [1, testExam.SlotsPerDay - 1]),
                    MinimizeRooms = new Constraint(false, r.Next(3,6)*100),
                    RolesSoftContinuity = new Constraint(false, Enumerable.Range(1, testExam.RolesNr).ToDictionary(role => role, role => r.Next(3))),
                    SameDegreeInRoom = new Constraint(false, r.Next(3) * 10),
                    Mergeability = new Constraint(false, r.Next(3))
                };
                testConstraints.OptimalLunchLenght = new Constraint(false, [2, r.Next((int)testConstraints.LunchTsMinLen.SingleParam,3)]);

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
                
                string fileNameJson = @"..\..\..\JSON files\" + testExam.StudentsNr + "_" + testExam.InstructorsNr + "_" + j + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".json";
                string fileNameDzn = @"..\..\..\DZN files\" + testExam.StudentsNr + "_" + testExam.InstructorsNr + "_" + j + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".dzn";

                string json = JsonSerializer.Serialize(testExamAllData, options);
                Console.WriteLine(json);
                File.WriteAllText(fileNameJson, json);

                var examToDzn = new ExamToDzn(testExam,testConstraints);
                examToDzn.SerializeToDzn(fileNameDzn);
            }
        }

        private static void GenerateInstructors(Random r, Exam testExam, Dictionary<int, HashSet<int>> instructorsOfProgramms)
        {
            testExam.Instructors = new Instructor[testExam.InstructorsNr];
            for (int i = 0; i < testExam.InstructorsNr; i++)
            {
                var newInstructor = new Instructor()
                {
                    Id = i + 1,
                    Programm = Enumerable.Range(0, r.Next(1, testExam.ProgrammNr + 1)).Select(_ => r.Next(1, testExam.ProgrammNr + 1)).Distinct().Order().ToArray(),
                    Roles = Enumerable.Range(0, r.Next(testExam.RolesNr + 1)).Select(_ => r.Next(1, testExam.RolesNr + 1)).Distinct().Order().ToArray(),
                    Availability = GenerateSmartAvailability(r, testExam, 0.85, 0.1, 0.6, 0.1)
                };

                testExam.Instructors[i] = newInstructor;
                foreach (var p in newInstructor.Programm)
                {
                    instructorsOfProgramms[p].Add(newInstructor.Id);
                }
            }
        }

        private static void GenerateStudents(Random r, Exam testExam, Dictionary<int, HashSet<int>> instructorsOfProgramms)
        {
            int[] coursesPerDegree = Enumerable.Range(0, testExam.DegreeNr).Select(_ => r.Next(Math.Max(testExam.CoursesNr + 1, 4))).ToArray();
            testExam.Students = new Student[testExam.StudentsNr];
            for (int i = 0; i < testExam.StudentsNr; i++)
            {
                var newStudent = new Student()
                {
                    Id = i + 1,
                    Programm = r.Next(1, testExam.ProgrammNr + 1),
                    Degree = r.Next(1, testExam.DegreeNr + 1),
                    //SupervisorId = r.Next(1, testExam.InstructorsNr + 1),
                    Availability = GenerateSmartAvailability(r,testExam, 0.9, 0.1, 0.5, 0.1)
                };
                var instructorSet = instructorsOfProgramms[newStudent.Programm];
                newStudent.SupervisorId = instructorSet.ElementAt(r.Next(instructorSet.Count));
                newStudent.CourseIds = Enumerable.Range(1, testExam.CoursesNr).OrderBy(_ => Guid.NewGuid()).Take(coursesPerDegree[newStudent.Degree-1]).Order().ToArray();

                int minCard = 0;
                var allCourseInstructors = new List<int>();
                var rolesInstructors = Enumerable.Range(0, testExam.InstructorRolesPerProgramm.GetLength(1))
                                 .Select(x => testExam.InstructorRolesPerProgramm[newStudent.Programm - 1, x]).ToArray()
                                 .SelectMany(list => list).ToArray();

                foreach (int courseId in newStudent.CourseIds)
                {
                    int[] courseInstructors = testExam.Courses[courseId - 1].InstructorIds;
                    allCourseInstructors.AddRange(courseInstructors);
                    if (!courseInstructors.Intersect(rolesInstructors).Any()) minCard++;
                }
                if (!allCourseInstructors.Any(x => x == newStudent.SupervisorId) && !rolesInstructors.Any(x => x == newStudent.SupervisorId)) minCard++;

                newStudent.TheoryticalMinCard = minCard;
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
                    Id = i + 1,
                    // TODO: more realistic headcount (less instructors)
                    InstructorIds = Enumerable.Range(0, r.Next(1, testExam.InstructorsNr + 1)).Select(_ => r.Next(1, testExam.InstructorsNr + 1)).Distinct().Order().ToArray(),
                };
                testExam.Courses[i] = newCourse;
            }
        }

        public static void GetInstructorRolesPerProgram(Exam testExam)
        {
            testExam.InstructorRolesPerProgramm = new List<int>[testExam.ProgrammNr,testExam.RolesNr];

            for (int i = 0; i < testExam.ProgrammNr; i++)
            {
                for (int j = 0; j < testExam.RolesNr; j++)
                {
                    testExam.InstructorRolesPerProgramm[i, j] = new List<int>();
                }
            }

            foreach (var instructor in testExam.Instructors)
            {
                if (instructor.Programm != null && instructor.Roles != null)
                {
                    foreach (var program in instructor.Programm)
                    {
                        foreach (var role in instructor.Roles)
                        {
                            testExam.InstructorRolesPerProgramm[program - 1,role - 1].Add(instructor.Id);
                        }                      
                    }
                }
            }
        }

        private static int[] GenerateSmartAvailability(Random r, Exam testExam, double Tmean, double Tvar, double Fmean, double Fvar)
        {
            double trueToTrue = GenerateRandomDouble(Tmean - Tvar, Tmean + Tvar);
            double falseToTrue = GenerateRandomDouble(Fmean - Fvar, Fmean + Fvar);
            var chain = new MarkovChain(trueToTrue, falseToTrue);
            return chain.GenerateStates(r.Next(2), testExam.DaysNr * testExam.SlotsPerDay / testExam.AvailabilitySlotsLenght);
        }

        public static double GenerateRandomDouble(double min, double max)
        {
            Random random = new Random();
            return random.NextDouble() * (max - min) + min;
        }
    }
}
