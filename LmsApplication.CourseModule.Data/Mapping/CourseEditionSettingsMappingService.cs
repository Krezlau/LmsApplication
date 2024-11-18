using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseModule.Data.Courses;
using LmsApplication.CourseModule.Data.Entities;

namespace LmsApplication.CourseModule.Data.Mapping;

public static class CourseEditionSettingsMappingService
{
    public static CourseEditionSettingsModel ToModel(this CourseEditionSettings settings)
    {
        return new CourseEditionSettingsModel
        {
            EditionId = settings.CourseEditionId,
            AllowAllToPost = settings.AllowAllToPost
        };
    }
    
    public static CourseEditionPublicSettingsModel ToPublicModel(this CourseEditionSettings settings)
    {
        return new CourseEditionPublicSettingsModel 
        {
            AllowAllToPost = settings.AllowAllToPost
        };
    }
}