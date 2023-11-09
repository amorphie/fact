using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


public class UserSmsKey : amorphie.core.Base.EntityBase
{
    public string SmsKey { get; set; } = string.Empty;
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public User? User { get; set; }
}
