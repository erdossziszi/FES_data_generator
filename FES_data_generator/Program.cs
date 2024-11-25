using System.Text.Json;
using System.Text.Json.Serialization;
using FES_data_generator.Model;
using FES_data_generator.Utils;

namespace FES_data_generator;

internal class Program
{
    private const string RootFolder = @"..\..\..";
    private const string JsonFolderName = "JsonFiles";
    private const string DznFolderName = "DznFiles";

    private static readonly JsonSerializerOptions _jsonOptions = new ()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    static void Main(string[] args)
    {
        // Global random generator to potentially set seed
        var random = new Random();
        TimeSlots[] timeSlots =
            [
                new TimeSlots
                {
                    OptimalSlotsPerDay = 10,
                    OptimalAvailabilityTimeSlotLengths = [1, 2, 5, 10],
                    OptimalMinimumExamLength = 1,
                    OptimalMaximumExamLength = 2,
                },
                new TimeSlots
                {
                    OptimalSlotsPerDay = 15,
                    OptimalAvailabilityTimeSlotLengths = [1, 3, 5, 15],
                    OptimalMinimumExamLength = 1,
                    OptimalMaximumExamLength = 3,
                },
                new TimeSlots
                {
                    OptimalSlotsPerDay = 20,
                    OptimalAvailabilityTimeSlotLengths = [2, 5, 10, 20],
                    OptimalMinimumExamLength = 2,
                    OptimalMaximumExamLength = 4,
                },
                new TimeSlots
                {
                    OptimalSlotsPerDay = 40,
                    OptimalAvailabilityTimeSlotLengths = [4, 8, 20, 40],
                    OptimalMinimumExamLength = 4,
                    OptimalMaximumExamLength = 8,
                },
                new TimeSlots
                {
                    OptimalSlotsPerDay = 60,
                    OptimalAvailabilityTimeSlotLengths = [6, 12, 30, 60],
                    OptimalMinimumExamLength = 6,
                    OptimalMaximumExamLength = 10,
                },
            ];

        foreach (var timeSlot in timeSlots)
        {
            for (int index = 0; index < 50; index++)
            {
                Run(random, index, timeSlot);
            }
        }
    }

    private static void Run(Random random, int index, TimeSlots timeSlot)
    {
        foreach (var slotLength in timeSlot.OptimalAvailabilityTimeSlotLengths)
        {
            var testExam = new Exam()
            {
                NumberOfStudents = 50,
                NumberOfInstructors = random.Next(3, 101),
                NumberOfDays = random.Next(6, 11),
                SlotsPerDay = timeSlot.OptimalSlotsPerDay,
                AvailabilitySlotsLenght = slotLength,
                NumberOfRooms = random.Next(2, 5),
                NumberOfProgrammes = random.Next(1, 4),
                NumberOfDegrees = random.Next(1, 4),
                NumberOfRoles = random.Next(4),
                NumberOfCourses = random.Next(1, 19)
            };

            var examPopulator = new ExamPopulator(testExam, random);
            examPopulator.PopulateInstructors();
            examPopulator.PopulateInstructorRolesPerProgramme();
            examPopulator.PopulateCourses();
            examPopulator.PopulateStudents();

            var testConstraints = ConstraintGenerator.GenerateAllConstraints(random, testExam, timeSlot);

            var testExamAllData = new ExamAllData()
            {
                Exam = testExam,
                AllConstraints = testConstraints
            };

            var fileName = $"S_{testExam.NumberOfStudents}_Ts_{testExam.SlotsPerDay}_A_{testExam.AvailabilitySlotsLenght}_Nr_{index}";
            var fileNameJson = $@"{RootFolder}\{JsonFolderName}\{fileName}.json";
            var fileNameDzn = $@"{RootFolder}\{DznFolderName}\{fileName}.dzn";

            var json = JsonSerializer.Serialize(testExamAllData, _jsonOptions);
            File.WriteAllText(fileNameJson, json);

            DznSerilaizer.SerializeExam(testExam, testConstraints, fileNameDzn);
        }
    }
}
