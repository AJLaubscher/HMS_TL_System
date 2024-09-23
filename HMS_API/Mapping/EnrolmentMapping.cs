using System;
using HMS_API.Dtos.enrollment;

namespace HMS_API.Mapping;

public static class EnrolmentMapping
{
    public static Entities.Enrolment ToEntity(this CreateEnrolmentDto enrolment)
    {
        return new ()
        {
            ModID = enrolment.ModID,
            StudID = enrolment.StudID
        };
    }

    public static Entities.Enrolment ToEntity(this UpdateEnrolmentDto enrolment, int modId, int studId)
    {
        return new()
        {
            ModID = modId,
            StudID = studId,
            Enrol_year = enrolment.Enrol_year,
            // DateOnly Created,
            Modified = enrolment.Modified,
            Deleted = enrolment.Deleted 
        };
    }


    public static EnrolmentSummaryDto ToEnrolmentSummaryDto(this Entities.Enrolment enrolment)
    {
        return new
        (
            enrolment.ModID,
            enrolment.Module!.ModName,
            enrolment.StudID,
            enrolment.UserAccount!.FName,
            enrolment.Enrol_year,
            enrolment.Created,
            enrolment.Modified,
            enrolment.Deleted
        );
    }

    public static EnrolmentDetailsDto ToEnrolmentDetailsDto(this Entities.Enrolment enrolment)
    {
        return new
        (
            enrolment.ModID,
            enrolment.StudID,
            enrolment.Enrol_year,
            enrolment.Created,
            enrolment.Modified,
            enrolment.Deleted
        );
    }
}
