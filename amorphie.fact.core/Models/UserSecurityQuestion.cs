using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using amorphie.core.Base;
using NpgsqlTypes;

public class UserSecurityQuestion:EntityBase
{
    public string SecurityAnswer { get; set; } = string.Empty;

    [ForeignKey("SecurityQuestion")]
    public Guid SecurityQuestionId { get; set; }
     [ForeignKey("User")]
    public Guid UserId { get; set; }
    public User? Users { get; set; }
    public SecurityQuestion? SecurityQuestions { get; set; }

   

}