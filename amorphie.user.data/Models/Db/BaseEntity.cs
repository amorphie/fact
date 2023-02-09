using System.ComponentModel.DataAnnotations;

public class BaseEntity
{
     [Key]
    public Guid Id { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public Guid ModifiedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public Guid CretedByBehalfOf { get; set; }
    public Guid ModifiedByBehalof { get; set; }


}