using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FES_data_generator.Model
{
    internal class AllConstraints
    {
        public Constraint? OneExamPerRoom { get; set; }
        public Constraint? EachStudentScheduled { get; set; }
        public Constraint? OnePersonOneRoomAtOneTime { get; set; }
        public Constraint? OwnProgramForInstructors { get; set; }
        public Constraint? ExaminerInCourse { get; set; }
        public Constraint? MinimizeRooms { get; set; }
        public Constraint[]? RolesAvailable { get; set; }
        public Constraint? SupervisorAvailable { get; set; }
        public Constraint? ExaminerAvailable { get; set; }
        public Constraint[]? RolesContinuity { get; set; }
        public Constraint? InstructorContinuityPerDay { get; set; }
        public Constraint[]? RolesContinuityPerDay { get; set; }
        public Constraint? SameProgramInRoom { get; set; }
        public ConstraintWithParameter[]? RolesNr { get; set; }
        public ConstraintWithParameter[]? MergeableRolesWithSupervisor { get; set; }
        public ConstraintWithParameter[]? MergeableRolesWithExaminer { get; set; }
        public ConstraintWithParameter? InstructorLunchBreak { get; set; }
        public ConstraintWithParameter? OptimalStartTs { get; set; }
        public ConstraintWithParameter? OptimalFinishTs { get; set; }
        public ConstraintWithParameter? OptimalLunchStart { get; set; }
        public ConstraintWithParameter? OptimalLunchFinish { get; set; }
        public ConstraintWithParameter? OptimalLunchLenght { get; set; }
        public Constraint[]? SimilarWorkloadsInRoles { get; set; }
        public ConstraintWithParameter[]? ExamLenghtPerDegree { get; set; }
    }
}
