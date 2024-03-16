using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace amorphie.fact.core.Dtos.SecurityQuestion
{
    public class SecurityQuestionDto
    {
        public Guid Id{get;set;}
        public string Description{get;set;}
        public string Key{get;set;}
        public string ValueTypeClr{get;set;}
        public int Priority{get;set;}
    }
}