using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using amorphie.fact.core.Enum;

namespace amorphie.fact.core.Models
{
    public class DeviceInfo
    {
        public Guid Id { get; set; }
        public string Model { get; set; }
        public string Platform { get; set; }
        public string Version { get; set; }
        public string Manufacturer { get; set; }
        public string Description { get; set; }
        public DeviceStatusType Status { get; set; }
        public string DeviceId { get; set; }
        public DateTime? LastLogonDate { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? ActivationRemovalDate { get; set; }
        public string CreatedByUserName { get; set; }
    }
}