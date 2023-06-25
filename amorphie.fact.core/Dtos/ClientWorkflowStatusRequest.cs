using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


    public class ClientWorkflowStatusRequest
    {
        public string? name {get;set;}
        public string? loginUrl {get;set;}
        public string? returnUrl {get;set;}
        public string? logoutUrl {get;set;}
        public string? pkce {get;set;}
        public ClientWorkflowFlows[]? flows{get;set;}
        public ClientWorkflowTokens[]? tokens{get;set;}
        public string[]? allowedGrantTypes {get;set;}
        public string[]? allowedScopeTags {get;set;}
        public string? idempotencyMode {get;set;}
        public string? secret {get;set;}
        public string[]? tags {get;set;}

    }
    public class ClientWorkflowFlows {
        public string? type {get;set;}
        public string? workflow {get;set;}
        public string? tokenDuration {get;set;}
    }
    
    public class ClientWorkflowTokens {
        public string? type {get;set;}
        public string[]? publicClaims {get;set;}
        public string? tokenDuration {get;set;}
    }
