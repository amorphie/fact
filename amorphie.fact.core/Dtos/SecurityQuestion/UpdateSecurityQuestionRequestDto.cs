using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace amorphie.fact.core.Dtos.SecurityQuestion
{
    public class UpdateSecurityQuestionRequestDto
    {
        public string OldAnswer { get; set; }
        public Guid NewQuestionDefinitionId { get; set; }

        public string NewAnswer { get; set; }
    }
}