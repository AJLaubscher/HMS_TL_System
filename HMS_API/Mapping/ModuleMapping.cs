using System;
using System.Reflection;
using HMS_API.Dtos.module;

namespace HMS_API.Mapping;

public static class ModuleMapping
{
    public static Entities.Module ToEntity(this CreateModuleDto module)
    {
        return new ()
        {
            Code = module.Code,
            ModName = module.ModName,
            LectID = module.LectID
        };
    }

    public static Entities.Module ToEntity(this UpdateModuleDto module, int id)
    {
        return new()
        {
            Id = id,
            Code = module.Code,
            ModName = module.ModName,
            LectID = module.LectID,
            //DateOnly Created,         non changing value
            Modified = module.Modified,
            Deleted = module.Deleted    
        };
    }


    public static ModuleSummaryDto ToModuleSummaryDto(this Entities.Module module)
    {
        return new
        (
	        module.Id,
            module.Code,
            module.ModName,
            module.LectID,
            module.UserAccount!.LName,         
            module.Created, 
            module.Modified,
            module.Deleted
        );
    }

    public static ModuleDetailsDto ToModuleDetailsDto(this Entities.Module module)
    {
        return new
        (
	        module.Id,
            module.Code,
            module.ModName,
            module.LectID,
            module.Created, 
            module.Modified,
            module.Deleted
        );
    }
}
