using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseBoardModule.Data.Entities;

namespace LmsApplication.CourseBoardModule.Data.Models.Validation;

public class UpdatePostValidationModel
{
    public required string Content { get; set; }
    
    public required Post? Post { get; set; }
    
    public required UserExchangeModel? User { get; set; }
}