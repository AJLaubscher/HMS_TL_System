using System;
using HMS_API.Dtos.feedback;

namespace HMS_API.Mapping;

public static class FeedbackMapping
{
    public static Entities.Feedback ToEntity(this CreateFeedbackDto feedback)
    {
        return new ()
        {
            SubID = feedback.SubID,              
            LectID = feedback.LectID,             
            Comment = feedback.Comment,
            MarkAchieved = feedback.MarkAchieved
        };
    }

    public static Entities.Feedback ToEntity(this UpdateFeedbackDto feedback, int id)
    {
        return new()
        {
            Id = id,
            SubID = feedback.SubID,             
            LectID = feedback.LectID,             
            Comment = feedback.Comment,
            ReturnDate = feedback.ReturnDate,
            MarkAchieved = feedback.MarkAchieved,
            // DateOnly Created,
            Modified = feedback.Modified,
            Deleted = feedback.Deleted 
        };
    }


    public static FeedbackSummaryDto ToFeedbackSummaryDto(this Entities.Feedback feedback)
    {
        return new
        (
            feedback.Id,
            feedback.SubID,              
            feedback.LectID,            
            feedback.UserAccount!.FName,       
            feedback.Comment,
            feedback.ReturnDate,
            feedback.MarkAchieved,
            feedback.Created,
            feedback.Modified,
            feedback.Deleted
        );
    }

    public static FeedbackDetailsDto ToFeedbackDetailsDto(this Entities.Feedback feedback)
    {
        return new
        (
            feedback.Id,
            feedback.SubID,              
            feedback.LectID,             
            feedback.Comment,
            feedback.ReturnDate,
            feedback.MarkAchieved,
            feedback.Created,
            feedback.Modified,
            feedback.Deleted
        );
    }
}
