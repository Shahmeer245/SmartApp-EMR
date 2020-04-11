using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazikCareService.BiztalkRepository.RISADTA04Service;
using MazikCareService.BiztalkRepository.RISADTA40Service;
using MazikCareService.BiztalkRepository.RISADTA08Service;
using MazikCareService.BiztalkRepository.RISORMO01Service;
namespace MazikCareService.BiztalkRepository
{
    public class RISRepository
    {
        string msgcontolId;
        public string ADT_A04(string aptRecId, string caseId, string patientId)
       {
          
            try
            {
                RISADTA04Client BiztalkClient = new RISADTA04Client();
                MazikCareService.BiztalkRepository.RISADTA04Service.GetPatient param = new MazikCareService.BiztalkRepository.RISADTA04Service.GetPatient();
                param.CaseId = caseId;
                param.PatientId = patientId;
                param.AppointmentRecId = aptRecId;
                param.MessageControlId= Library.SequentialGuid();
                msgcontolId = param.MessageControlId;
                BiztalkClient.GetPatientDetail(param);
            }
            catch (Exception ex)
            {

                return string.Empty;
            }
            return msgcontolId;

        }
       public string ADT_A08(string aptRecId, string caseId, string patientId)
       {
            try
            {
                RISADTA08Client BiztalkClient = new RISADTA08Client();
                MazikCareService.BiztalkRepository.RISADTA08Service.GetPatient param = new MazikCareService.BiztalkRepository.RISADTA08Service.GetPatient();
                param.CaseId = caseId;
                param.PatientId = patientId;
                param.AppointmentRecId = aptRecId;
                param.MessageControlId = Library.SequentialGuid();
                msgcontolId = param.MessageControlId;
                BiztalkClient.UpdatePatientDetail(param);
            }
            catch (Exception)
            {

                return string.Empty;
            }
            return msgcontolId;

        }
       public string ADT_A40(string patientId, string mergepatientId)
       {
            try
            {
                RISADTA40Client BiztalkClient = new RISADTA40Client();
                MazikCareService.BiztalkRepository.RISADTA40Service.GetPatient param = new MazikCareService.BiztalkRepository.RISADTA40Service.GetPatient();
                param.CaseId = string.Empty;
                param.PatientId = patientId;
                param.MergePatientId = mergepatientId;
                param.AppointmentRecId = string.Empty;
                param.MessageControlId = Library.SequentialGuid();
                msgcontolId = param.MessageControlId;
                BiztalkClient.MergePatientDetail(param);
            }
            catch (Exception)
            {

                return string.Empty;
            }
            return msgcontolId;

        }
       public string ORM_O01(string patientId, string OrderId, string apptRecId, string caseId, string OrderStatus)
       {
            try
            {
                RISORMO01Client BiztalkClient = new RISORMO01Client();
                GetOrderDetail param = new GetOrderDetail();
                param.PatientId = patientId;
                param.OrderId = OrderId;
                param.OrderStatus = OrderStatus;
                param.AppointmentRecId = apptRecId;
                param.CaseId = caseId;
                param.MessageControlId = Library.SequentialGuid();
                msgcontolId = param.MessageControlId;
                BiztalkClient.GetOrder(param);
            }
            catch (Exception)
            {

                return string.Empty;
            }
            return msgcontolId;

        }
    }
}
