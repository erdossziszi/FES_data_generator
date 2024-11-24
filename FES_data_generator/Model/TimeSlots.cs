namespace FES_data_generator.Model;

public class TimeSlots
{
    public required int OptimalSlotsPerDay { get; set; }
    public required int[] OptimalAvailabilityTimeSlotLengths { get; set; }
    public required int OptimalMinimumExamLength { get; set; }
    public required int OptimalMaximumExamLength { get; set; }
}
