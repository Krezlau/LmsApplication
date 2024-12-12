using LmsApplication.Core.Shared.Models;

namespace LmsApplication.CourseBoardModule.Data.Models.Validation;

public class CreatePostValidationModel
{
    public required string Content { get; set; }
    
    public required UserExchangeModel? User { get; set; }
}