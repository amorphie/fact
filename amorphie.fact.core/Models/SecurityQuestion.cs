using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using amorphie.core.Base;
using NpgsqlTypes;

public class SecurityQuestion : EntityBase
{
    public string Question { get; set; } = string.Empty;
    public ICollection<UserSecurityQuestion>? UserSecurityQuestion { get; set; }

}