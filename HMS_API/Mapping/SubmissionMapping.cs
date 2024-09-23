using System;
using HMS_API.Dtos.submission;

namespace HMS_API.Mapping;

public static class SubmissionMapping
{
    public static Entities.Submission ToEntity(this CreateSubmissionDto submission)
    {
        return new ()
        {
            StudID = submission.StudID,             
            AssignID = submission.AssignID,                     
            FilePath = submission.FilePath                     
            //created by DB
            // int Id
            //DateOnly SubDate,
            //int Marked,
            //DateOnly Created,
            //DateOnly Modified,
            //int Deleted
        };
    }

    public static Entities.Submission ToEntity(this UpdateSubmissionDto submission, int id)
    {
        return new()
        {
            Id = id,
            StudID = submission.StudID,               //stay the same on update
            AssignID = submission.AssignID,           //stay the same on update
            SubDate = submission.SubDate,                    
            FilePath = submission.FilePath,                       
            Marked = submission.Marked,
            //DateOnly Created,                         not updated                 
            Modified = submission.Modified, 
            Deleted = submission.Deleted   
        };
    }


    public static SubmissionSummaryDto ToSubmissionSummaryDto(this Entities.Submission submission)
    {
        return new
        (
            submission.Id,
            submission.StudID,
            submission.UserAccount!.FName,                // student name
            submission.AssignID,               
            submission.Assignment!.Title,               // assignment title
            submission.SubDate,
            submission.FilePath,
            submission.Marked,
            submission.Created,
            submission.Modified,
            submission.Deleted 
        );
    }

    public static SubmissionDetailsDto ToSubmissionDetailsDto(this Entities.Submission submission)
    {
        return new
        (
            submission.Id,
            submission.StudID,
            submission.AssignID,               
            submission.SubDate,
            submission.FilePath,
            submission.Marked,
            submission.Created,
            submission.Modified,
            submission.Deleted 
        );
    }
}
