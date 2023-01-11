using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace amorphie.user.data;

public class UserSecurityQuestion
{
[Key]
public Guid Id { get; set; }
public string? SecurityQuestion{get;set;}
  [ForeignKey("User")]
public Guid  UserId{get;set;}
public User? User { get; set; }
}