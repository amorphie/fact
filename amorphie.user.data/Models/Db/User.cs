using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace amorphie.user.data;

public class User
{
    [Key]
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string EMail { get; set; } = string.Empty;
    public Phone? Phone { get; set; }

    public string Reference { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;

    public Guid CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public Guid ModifiedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }

    public Guid CretedByBehalfOf { get; set; }

    public Guid ModifiedByBehalof { get; set; }
    public string Salt{get;set;}= string.Empty;
   
    public ICollection<UserTag>? UserTags { get; set; }

    public ICollection<UserSecurityQuestion>? UserSecurityQuestion { get; set; }

    public ICollection<UserSecurityImage>? UserSecurityImages { get; set; }


}
public record Phone
{
    public int CountryCode { get; set; }
    public int Prefix { get; set; }
    public int Number { get; set; }

}
