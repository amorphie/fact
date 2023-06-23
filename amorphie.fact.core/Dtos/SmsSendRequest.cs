using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace amorphie.fact.core.Dtos
{
    public class SmsRequest
    {
        public string sender { get; set; }
        public string smsType { get; set; }
        public Phone? phone { get; set; }
        public string? content { get; set; }
        public long? customerNo { get; set; }
        public string? citizenshipNo { get; set; }
        public string[]? tags { get; set; }
        public bool? instantReminder { get; set; } = false;
        public Process? process { get; set; }
    }
    public enum SmsTypes
    {
        Bulk,
        Fast,
        Otp
    }
    public enum SenderType
    {
        AutoDetect = 0,
        Burgan = 1,
        On = 2
    }
    public class Process
    {
        public string? name { get; set; }
        public string? itemId { get; set; }
        public string? action { get; set; }
        public string? identity { get; set; }
    }
    public class Phone
    {
        public int countryCode { get; set; }
        public int prefix { get; set; }
        public int number { get; set; }

        public Phone()
        {

        }

        public Phone(string phone)
        {
            countryCode = Convert.ToInt32(phone.Substring(0, 2));
            prefix = Convert.ToInt32(phone.Substring(2, 3));
            number = Convert.ToInt32(phone.Substring(5, 7));
        }

        public override string ToString()
        {
            return $"+{countryCode}{prefix}{number}";
        }

        public string Concatenate()
        {
            return $"{countryCode}{prefix}{number}";
        }
    }
}