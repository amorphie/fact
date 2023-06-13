
public class PostWorkflow
    {
        public Guid recordId {get;set;}
        public dynamic? entityData {get;set;}
        public string newStatus {get;set;}=default!;
        public Guid? user {get;set;}
        public Guid? behalfOfUser {get;set;}
        public string  workflowName {get;set;}=default!;

    }
    public class PostWorkflowDtoUser:PostWorkflow
    {
        public PostUserRequest? data {get;set;}
    }

