using System.ComponentModel.DataAnnotations;

namespace LmsApplication.Core.Data.Entities;

public class User
{
    [Key]
    public Guid Id { get; set; }
}