﻿using FES_data_generator.Model;
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

            Exam testExam = new Exam()
            {
                StudentsNr = 12,
                InstructorsNr = 5,
                DaysNr = 1,
                SlotsPerDay = 60,
                RoomNr = 1,
                ProgrammNr = 2,
                DegreeNr = 2,
                RolesNr = 2,  //nullable
                CoursesNr = 5 //nullable
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
            string json = JsonSerializer.Serialize(testExamAllData, options);
            Console.WriteLine(json);
            File.WriteAllText(@"..\..\..\JSON files\test.json", json);
            
        }

        private static void GenerateInstructors(Random r, Exam testExam)
        {
            //var chain = new MarkovChain(0.7, 0.4);
            testExam.Instructors = new Instructor[testExam.InstructorsNr];
            for (int i = 0; i < testExam.InstructorsNr; i++)
            {
                var newInstructor = new Instructor()
                {
                    Id = i,
                    Programm = Enumerable.Range(0, r.Next(1, testExam.ProgrammNr + 1)).Select(_ => r.Next(testExam.ProgrammNr)).Distinct().Order().ToArray(),
                    Roles = Enumerable.Range(0, r.Next(testExam.RolesNr + 1)).Select(_ => r.Next(testExam.RolesNr)).Distinct().Order().ToArray(),
                    Availability = GenerateSmartAvailability(r, testExam)
                };
                testExam.Instructors[i] = newInstructor;
            }
        }

        private static void GenerateStudents(Random r, Exam testExam)
        {
            int[] coursesPerDegree = Enumerable.Range(0, testExam.DegreeNr).Select(_ => r.Next(testExam.CoursesNr + 1)).ToArray();
            //Console.WriteLine(String.Join(", ", coursesPerDegree));
            testExam.Students = new Student[testExam.StudentsNr];
            for (int i = 0; i < testExam.StudentsNr; i++)
            {
                var newStudent = new Student()
                {
                    Id = i,
                    Programm = r.Next(testExam.ProgrammNr),
                    Degree = r.Next(testExam.DegreeNr),
                    SupervisorId = r.Next(testExam.InstructorsNr),
                    Availability = GenerateSmartAvailability(r,testExam)
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
                    InstructorIds = Enumerable.Range(0, r.Next(1, testExam.InstructorsNr + 1)).Select(_ => r.Next(testExam.InstructorsNr)).Distinct().Order().ToArray(),
                };
                testExam.Courses[i] = newCourse;
            }
        }

        private static bool[] GenerateSmartAvailability(Random r, Exam testExam)
        {
            var chain = new MarkovChain(0.8, 0.3);
            return chain.GenerateStates(r.Next(2) == 1, testExam.DaysNr * testExam.SlotsPerDay);
        }
    }
}
