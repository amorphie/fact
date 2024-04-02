using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace amorphie.fact.core.Dtos.SecurityImage
{
    public class SecurityImageDto
    {
        public Guid Id{get;set;}
        public bool IsSelected{get;set;}
        public string ImagePath{get;set;}
        public string Title{get;set;}
    }
}