
public class PostWorkflow
{
    public Guid recordId { get; set; }
    public dynamic? entityData { get; set; }
    public string newStatus { get; set; } = default!;
    public Guid? user { get; set; }
    public Guid? behalfOfUser { get; set; }
    public string workflowName { get; set; } = default!;

}
public class PostWorkflowDtoUser : PostWorkflow
{
    public PostUserRequest? data { get; set; }
}
public class OpenBankingUser
{
    public string? reference { get; set; }
    public Phone? phone { get; set; }
    public string? smsKey { get; set; }
    public string? firstName { get; set; }
    public string? lastName { get; set; }
    public string? eMail { get; set; }
    public string? password { get; set; }
    public Guid? question { get; set; }
    public string? answer { get; set; }
    public Guid? imageId { get; set; }
    public bool? contract1 { get; set; }
    public bool? contract2 { get; set; }
    public bool? contract3 { get; set; }

}

