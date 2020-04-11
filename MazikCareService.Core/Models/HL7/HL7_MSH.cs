using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models.HL7
{
   public class HL7_MSH
    {
       public string MessageSendingApplication
       {
           get;
           set;
       }
       public string SendingFacility
       {
           get;
           set;
       }
       public string ReceivingApplication
       {
           get;
           set;
       }
       public string ReceivingFacility
       {
           get;
           set;
       }
       public string Datetime
       {
           get;
           set;
       }
       public string Security
       {
           get;
           set;
       }
       public string MessageType
       {
           get;
           set;
       }
       public string MessageCode
       {
           get;
           set;
       }
       public string TriggerEventCode
       {
           get;
           set;
       }
       public string MessageControlID
       {
           get;
           set;
       }
       public string ProcessingID
       {
           get;
           set;
       }
       public string VersionID
       {
           get;
           set;
       }


    }
}
