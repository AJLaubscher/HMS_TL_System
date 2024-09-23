using System;
using HMS_API.Dtos.assignment;

namespace HMS_API.Mapping;

public static class AssignmentMapping
{
    public static Entities.Assignment ToEntity(this CreateAssignmentDto assignment)
    {
        return new ()
        {
            ModID = assignment.ModID, 
            Title = assignment.Title,
            Instructions = assignment.Instructions, 
            OpenDate = assignment.OpenDate, 
            DueDate = assignment.DueDate, 
            MaxMarks =assignment.MaxMarks, 
            SubPath = assignment.SubPath 
            // created by DB
            // int Id,
            // DateOnly Created, 
            // DateOnly Modified, 
            // int Deleted, 

        };
    }

    public static Entities.Assignment ToEntity(this UpdateAssignmentDto assignment, int id)
    {
        return new()
        {
            Id = id,
            ModID = assignment.ModID, 
            Title = assignment.Title,
            Instructions = assignment.Instructions, 
            OpenDate = assignment.OpenDate, 
            DueDate = assignment.DueDate, 
            MaxMarks =assignment.MaxMarks, 
            SubPath = assignment.SubPath,
            //Created                               not updated
            Modified = assignment.Modified, 
            Deleted = assignment.Deleted   
        };
    }


    public static AssignmentSummaryDto ToAssignmentSummaryDto(this Entities.Assignment assignment)
    {
        return new
        (
            assignment.Id,
            assignment.ModID,
            assignment.Module!.ModName,  
            assignment.Title,
            assignment.Instructions, 
            assignment.OpenDate, 
            assignment.DueDate, 
            assignment.MaxMarks, 
            assignment.SubPath,
            assignment.Created, 
            assignment.Modified, 
            assignment.Deleted  
        );
    }

    public static AssignmentDetailsDto ToAssignmentDetailsDto(this Entities.Assignment assignment)
    {
        return new
        (
            assignment.Id,
            assignment.ModID,
            assignment.Title,
            assignment.Instructions, 
            assignment.OpenDate, 
            assignment.DueDate, 
            assignment.MaxMarks, 
            assignment.SubPath,
            assignment.Created, 
            assignment.Modified, 
            assignment.Deleted 
        );
    }
}
