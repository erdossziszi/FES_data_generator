using FES_data_generator.Model;

namespace FES_data_generator.Utils;

public static class ConstraintGenerator
{
    public static AllConstraints GenerateAllConstraints(Random random, Exam exam, TimeSlots timeSlot)
    {
        var minInstructorsPerRoles = new int[exam.NumberOfRoles];
        for (int k = 0; k < exam.NumberOfRoles; k++)
        {
            minInstructorsPerRoles[k] = exam.NumberOfInstructors;

            for (int i = 0; i < exam.NumberOfProgrammes; i++)
            {
                if (exam.InstructorRolesPerProgramme![i, k] != null)
                {
                    minInstructorsPerRoles[k] = Math.Min(minInstructorsPerRoles[k], exam.InstructorRolesPerProgramme[i, k].Count);
                }
            }
        }

        Dictionary<int, int> degreeExamLenPairs = Enumerable.Range(1, exam.NumberOfDegrees)
            .ToDictionary(degree => degree, degree => random.Next(timeSlot.OptimalMinimumExamLength, timeSlot.OptimalMaximumExamLength + 1));
        int minimumExamLength = degreeExamLenPairs.Min(x => x.Value);
        AllConstraints testConstraints = new AllConstraints()
        {
            RolesDemands = new Constraint(
                required: true,
                arrayParam: Enumerable.Range(0, exam.NumberOfRoles)
                    .Select(x => random.Next(1, minInstructorsPerRoles[x] + 1))
                    .ToArray()),
            RolesContinuity = new Constraint(
                required: true,
                arrayParam: Enumerable.Range(1, exam.NumberOfRoles)
                    .OrderBy(x => random.Next())
                    .Take(random.Next(exam.NumberOfRoles))
                    .ToArray()),
            ExamLength = new Constraint(
                required: true,
                arrayParam: exam.Students
                    .Select(student => degreeExamLenPairs[student.Degree])
                    .ToArray()),
            MinimumLunchTimeSlotLength = new Constraint(
                required: true,
                singleParam: random.Next(minimumExamLength + 1)),
            LunchStarts = new Constraint(
                required: true,
                arrayDictParam: Enumerable.Range(1, exam.NumberOfDays)
                    .ToDictionary(day => day, day => new int[] {
                        random.Next(exam.SlotsPerDay / 4 + (day - 1) * exam.SlotsPerDay, exam.SlotsPerDay/2 + (day - 1) * exam.SlotsPerDay + 1),
                        random.Next(exam.SlotsPerDay / 2 + 1 + (day - 1) * exam.SlotsPerDay, exam.SlotsPerDay * 3/4 + (day - 1) * exam.SlotsPerDay + 1)
                    })),
            SupervisorAvailable = new Constraint(required: false, singleParam: random.Next(2) * 5),
            OptimalStartTs = new Constraint(required: false, arrayParam: [random.Next(3), random.Next(minimumExamLength)]),
            OptimalFinishTs = new Constraint(required: false, arrayParam: [random.Next(3), exam.SlotsPerDay - random.Next(minimumExamLength)]),
            MinimizeRooms = new Constraint(required: false, singleParam: random.Next(6) * 100),
            RolesSoftContinuity = new Constraint(
                required: false,
                dictParam: Enumerable.Range(1, exam.NumberOfRoles).ToDictionary(role => role, role => random.Next(5))),
            SameDegreeInRoom = new Constraint(required: false, singleParam: random.Next(3) * 10),
            Mergeability = new Constraint(required: false, singleParam: random.Next(3))
        };
        testConstraints.OptimalLunchLength = new Constraint(
            required: false,
            arrayParam: [random.Next(4), random.Next((int)testConstraints.MinimumLunchTimeSlotLength.SingleParam!, minimumExamLength * 2)]);
        return testConstraints;
    }
}
