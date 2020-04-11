using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazikCareService.BiztalkRepository.LISORMO01Service;
namespace MazikCareService.BiztalkRepository
{
    public class LISRepository
    {
        string msgcontolId;
    
       public string ORM_O01(string patientId, string OrderId, string apptId, string caseId, string OrderStatus)
       {
            try
            {
                BizTalkLISORMO01Client BiztalkClient = new BizTalkLISORMO01Client();
                GetOrderDetail param = new GetOrderDetail();
                param.PatientId = patientId;
                param.OrderId = OrderId;
                param.OrderStatus = OrderStatus;
                param.AppointmentRecId = apptId;
                param.CaseId = caseId;
                param.MessageControlId = Library.SequentialGuid();
                msgcontolId = param.MessageControlId;
                BiztalkClient.GetPatientOrder(param);
            }
            catch (Exception)
            {

                return string.Empty;
            }
            return msgcontolId;

        }
    }
}
