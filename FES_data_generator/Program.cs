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
            TimeSlots[] timeSlots =
            {
                new TimeSlots(10, [1,2,5,10], 1, 2),
                new TimeSlots(15, [1,3,5,15], 1, 3),
                new TimeSlots(20, [2,5,10,20], 2, 4),
                new TimeSlots(40, [4,8,20,40], 4, 8),
                new TimeSlots(60, [6,12,30,60], 6, 10)
            };

            for (int j = 0; j < 50; j++)
            {
                Run(r, j);
            }
        }

        private static void Run(Random r, int j)
        {
            Exam testExam = new Exam()
            {
                StudentsNr = 10,//r.Next(5, 301),    /////////////////////////////////////////////////////
                InstructorsNr = r.Next(3, 101),
                DaysNr = r.Next(1, 3), //TODO: Feasibility check
                SlotsPerDay = 60,//r.Next(10, 145),  /////////////////////////////////////////////////////
                AvailabilitySlotsLenght = 60, //must be dividable with SlotsPerDay  ///////////////////////
                RoomNr = r.Next(1, 3),
                ProgrammNr = r.Next(1, 3),
                DegreeNr = r.Next(1, 4),
                RolesNr = r.Next(4),  //nullable
                CoursesNr = r.Next(11) //nullable
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
            AllConstraints testConstraints = GetAllConstraints(r, testExam);

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
            string fileName = $"S_{testExam.StudentsNr}_Ts_{testExam.SlotsPerDay}_A_{testExam.AvailabilitySlotsLenght}_Nr_{j}";
            string fileNameJson = @"..\..\..\JSON files\" + fileName + ".json";
            string fileNameDzn = @"..\..\..\DZN files\" + fileName + ".dzn";

            string json = JsonSerializer.Serialize(testExamAllData, options);
            //Console.WriteLine(json);
            File.WriteAllText(fileNameJson, json);

            var examToDzn = new ExamToDzn(testExam, testConstraints);
            examToDzn.SerializeToDzn(fileNameDzn);
        }

        private static AllConstraints GetAllConstraints(Random r, Exam testExam)
        {
            int[] minInstructorsPerRoles = new int[testExam.RolesNr];
            for (int k = 0; k < testExam.RolesNr; k++)
            {
                minInstructorsPerRoles[k] = testExam.InstructorsNr;

                for (int i = 0; i < testExam.ProgrammNr; i++)
                {
                    if (testExam.InstructorRolesPerProgramm[i, k] != null)
                    {
                        minInstructorsPerRoles[k] = Math.Min(minInstructorsPerRoles[k], testExam.InstructorRolesPerProgramm[i, k].Count);
                    }
                }
            }
            Dictionary<int, int> degreeExamLenPairs = Enumerable.Range(1, testExam.DegreeNr).ToDictionary(degree => degree, degree => new Random().Next(1, 3)); ///////////
            int minExamLen = degreeExamLenPairs.Min(x => x.Value);
            AllConstraints testConstraints = new AllConstraints()
            {
                RolesDemands = new Constraint(true, Enumerable.Range(0, testExam.RolesNr).Select(x => r.Next(1, minInstructorsPerRoles[x] + 1)).ToArray()),
                RolesContinuity = new Constraint(true, Enumerable.Range(1, testExam.RolesNr).OrderBy(x => r.Next()).Take(r.Next(testExam.RolesNr)).ToArray()),
                ExamLen = new Constraint(true, testExam.Students.Select(student => degreeExamLenPairs[student.Degree]).ToArray()),
                LunchTsMinLen = new Constraint(true, r.Next(minExamLen + 1)),
                LunchStarts = new Constraint(true, Enumerable.Range(1, testExam.DaysNr)
                                  .ToDictionary(day => day, day => new int[] {
                                          r.Next(testExam.SlotsPerDay / 4 + (day - 1) * testExam.SlotsPerDay, testExam.SlotsPerDay/2 + (day - 1) * testExam.SlotsPerDay + 1),
                                          r.Next(testExam.SlotsPerDay / 2 + 1 + (day - 1) * testExam.SlotsPerDay, testExam.SlotsPerDay * 3/4 + (day - 1) * testExam.SlotsPerDay + 1)
                                  })),
                SupervisorAvailable = new Constraint(false, r.Next(2) * 5),
                OptimalStartTs = new Constraint(false, [r.Next(3), r.Next(minExamLen)]),
                OptimalFinishTs = new Constraint(false, [r.Next(3), testExam.SlotsPerDay - r.Next(minExamLen)]),
                MinimizeRooms = new Constraint(false, r.Next(6) * 100),
                RolesSoftContinuity = new Constraint(false, Enumerable.Range(1, testExam.RolesNr).ToDictionary(role => role, role => r.Next(5))),
                SameDegreeInRoom = new Constraint(false, r.Next(3) * 10),
                Mergeability = new Constraint(false, r.Next(3))
            };
            testConstraints.OptimalLunchLenght = new Constraint(false, [r.Next(4), r.Next((int)testConstraints.LunchTsMinLen.SingleParam, minExamLen * 2)]);
            return testConstraints;
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
                    Availability = GenerateSmartAvailability(r,testExam, 0.9, 0.1, 0.7, 0.1)
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
            var initState = r.NextDouble() < ((Tmean + Fmean) / 2.0) ? 1 : 0;
            return chain.GenerateStates(initState, testExam.DaysNr * testExam.SlotsPerDay / testExam.AvailabilitySlotsLenght);
        }

        public static double GenerateRandomDouble(double min, double max)
        {
            Random random = new Random();
            return random.NextDouble() * (max - min) + min;
        }
    }
}
