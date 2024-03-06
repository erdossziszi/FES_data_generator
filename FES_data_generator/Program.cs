﻿using FES_data_generator.Model;
using System.Text.Json;

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

            testExam.Instructors = new Instructor[testExam.InstructorsNr];
            for (int i = 0; i < testExam.InstructorsNr; i++)
            {
                var newInstructor = new Instructor()
                {
                    Id = i,
                    Programm = Enumerable.Range(0, r.Next(1, testExam.ProgrammNr + 1)).Select(_ => r.Next(testExam.ProgrammNr)).Distinct().Order().ToArray(),
                    Roles = Enumerable.Range(0, r.Next(testExam.RolesNr + 1)).Select(_ => r.Next(testExam.RolesNr)).Distinct().Order().ToArray(),
                    Availability = Enumerable.Repeat(false, testExam.DaysNr * testExam.SlotsPerDay).Select(x => new Random().Next(2) == 1).ToArray()
                };
                testExam.Instructors[i] = newInstructor;
            }

            int[] coursesPerDegree = Enumerable.Range(0, testExam.DegreeNr).Select(_ => r.Next(testExam.CoursesNr + 1)).ToArray();
            Console.WriteLine(String.Join(", ", coursesPerDegree));
            testExam.Students = new Student[testExam.StudentsNr];
            for (int i = 0; i < testExam.StudentsNr; i++)
            {
                var newStudent = new Student()
                {
                    Id = i,
                    Programm = r.Next(testExam.ProgrammNr),
                    Degree = r.Next(testExam.DegreeNr),
                    SupervisorId = r.Next(testExam.InstructorsNr),
                    Availability = Enumerable.Repeat(false, testExam.DaysNr * testExam.SlotsPerDay).Select(x => new Random().Next(2) == 1).ToArray()
                };
                //newStudent.CourseIds = Enumerable.Range(0, coursesPerDegree[newStudent.Degree]).Select(_ => r.Next(testExam.CoursesNr)).Distinct().Order().ToArray();
                newStudent.CourseIds = Enumerable.Range(0, testExam.CoursesNr).OrderBy(_ => Guid.NewGuid()).Take(coursesPerDegree[newStudent.Degree]).Order().ToArray();
                testExam.Students[i] = newStudent;
            }

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

            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(testExam, options);
            Console.WriteLine(json);
            File.WriteAllText(@"..\..\..\JSON files\test.json",json);
        }
    }
}
