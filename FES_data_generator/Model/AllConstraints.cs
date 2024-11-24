namespace FES_data_generator.Model;

public class AllConstraints
{
    public Constraint? RolesDemands { get; set; }
    public Constraint? RolesContinuity { get; set; }
    public Constraint? ExamLength { get; set; }
    public Constraint? MinimumLunchTimeSlotLength { get; set; }
    public Constraint? LunchStarts { get; set; }
    public Constraint? SupervisorAvailable { get; set; }
    public Constraint? OptimalLunchLength { get; set; }
    public Constraint? OptimalStartTs { get; set; }
    public Constraint? OptimalFinishTs { get; set; }
    public Constraint? MinimizeRooms { get; set; }
    public Constraint? RolesSoftContinuity { get; set; }
    public Constraint? SameDegreeInRoom { get; set; }
    public Constraint? Mergeability { get; set; }
}
