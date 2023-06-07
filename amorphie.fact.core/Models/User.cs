using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using amorphie.core.Base;
using NpgsqlTypes;

public class User : EntityBase
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    // public string Password { get; set; } = string.Empty;
    public string EMail { get; set; } = string.Empty;
    public Phone? Phone { get; set; } = new Phone();

    public string Reference { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;

    public string Salt { get; set; } = string.Empty;

    public NpgsqlTsVector SearchVector { get; set; }
    public ICollection<UserTag>? UserTags { get; set; } = new List<UserTag>();

    public ICollection<UserSecurityQuestion>? UserSecurityQuestion { get; set; } = new List<UserSecurityQuestion>();

    public ICollection<UserSecurityImage>? UserSecurityImages { get; set; } = new List<UserSecurityImage>();

    public ICollection<UserPassword>? UserPasswords { get; set; } = new List<UserPassword>();
}

public record Phone
{
    [Column("CountryCode")]
    public int CountryCode { get; set; }
    [Column("Prefix")]
    public int Prefix { get; set; }
    [Column("Number")]
    public string Number { get; set; } = string.Empty;

}

