
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using amorphie.user.data;

public class SecurityImage:BaseEntity
{
    public string Image { get; set; } = string.Empty;
    
}