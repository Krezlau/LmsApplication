using LmsApplication.Core.Shared.Enums;

namespace LmsApplication.Core.Shared.Extensions;

public static class CourseDurationExtensions
{
    public static TimeSpan ToTimeSpan(this CourseDuration duration) => duration switch
    {
        CourseDuration.OneSemester => TimeSpan.FromDays(90),
        CourseDuration.TwoSemesters => TimeSpan.FromDays(180),
        _ => throw new ArgumentOutOfRangeException(nameof(duration), duration, null)
    };
    
}