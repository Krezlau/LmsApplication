using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseBoardModule.Data.Entities;

namespace LmsApplication.CourseBoardModule.Data.Models.Validation;

public class UpdateCommentValidationModel
{
    public required string Content { get; set; }
    
    public required UserExchangeModel? User { get; set; }
    
    public required Comment? Comment { get; set; }
}