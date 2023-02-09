using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace amorphie.user.data;

public class User:BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string EMail { get; set; } = string.Empty;
    public Phone? Phone { get; set; }

    public string Reference { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;

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
