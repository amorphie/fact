using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace amorphie.user.data;

public class User 
{
    [Key]
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string?  Password { get; set; }
   [Required]
   [StringLength(11, MinimumLength = 11 , ErrorMessage = "TcNo cannot be longer than 11 characters")]
    public string?  TcNo { get; set; }
    public int? State{get;set;}
    public DateTime? LastLoginDate{get;set;}
    public DateTime? ModifiedDate{get;set;}

    public ICollection<UserSecurityQuestion>? UserSecurityQuestions { get; set; }
    public ICollection<UserTag>? UserTags { get; set; }


}