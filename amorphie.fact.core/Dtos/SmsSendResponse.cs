using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace amorphie.fact.core.Dtos
{
   public enum SendSmsResponseStatus 
    {
        Success = 200,        
        HasBlacklistRecord = 460,
        SimChange = 461,
        OperatorChange = 462,
        RejectedByOperator = 463,
        NotSubscriber = 464,
        ClientError = 465,
        ServerError = 466,
        MaximumCharactersCountExceed = 467,
    }
  public class SendSmsOtpResponse
    {
        public Guid TxnId { get; set; }
        public SendSmsResponseStatus Status { get; set; }

    }
}