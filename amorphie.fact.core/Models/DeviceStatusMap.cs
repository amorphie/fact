using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using amorphie.fact.core.Enum;

namespace amorphie.fact.core.Models
{
    public static class DeviceStatusConstants
    {
        public static Dictionary<int, DeviceStatusType> DeviceStatusMap = new Dictionary<int, DeviceStatusType>()
        {
            {1,DeviceStatusType.Active},
            {2,DeviceStatusType.Passive},
            {3,DeviceStatusType.Locked}
        };
    }
}