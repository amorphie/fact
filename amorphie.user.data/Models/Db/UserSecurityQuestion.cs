using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using amorphie.user.data;

public class UserSecurityQuestion
{
    public Guid Id { get; set; }
    public string SecurityAnswer { get; set; } = string.Empty;

    [ForeignKey("SecurityQuestion")]
    public Guid SecurityQuestionId { get; set; }
     [ForeignKey("User")]
    public Guid UserId { get; set; }
    
    public User? Users { get; set; }
     public Guid CreatedBy { get; set; }
      public DateTime? CreatedAt { get; set; }
    public Guid ModifiedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }

    public Guid CretedByBehalfOf { get; set; }

    public Guid ModifiedByBehalof { get; set; }
    public SecurityQuestion? SecurityQuestions { get; set; }

   

}