namespace amorphie.user.data;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class UserTag
{
[Key]
public Guid Id { get; set; }
public string? Name{get;set;}
 [ForeignKey("User")]
 public Guid  UserId{get;set;}
 public User? User { get; set; }
}