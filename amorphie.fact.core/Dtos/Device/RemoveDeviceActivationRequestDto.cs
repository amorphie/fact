using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace amorphie.fact.core.Dtos.Device
{
    public class RemoveDeviceActivationRequestDto
    {
        public Guid Id{get;set;}
        public string Description{get;set;}
    }
}