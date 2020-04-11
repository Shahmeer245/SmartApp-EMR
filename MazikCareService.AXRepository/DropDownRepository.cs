
using MazikCareService.AXRepository.AXServices;
using MazikLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.AXRepository
{
    public class DropDownRepository
    {
        public HMDropDownContract[] getDropDownList(string type)
        {
            HMDropDownContract[] contract = null;

            //HMDropDownServiceClient client = new HMDropDownServiceClient();

            //CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
            //client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };
            

            //switch (type)
            //{
            //    case "DocumentType":
            //        contract = client.getDocumentTypes(callContext);
            //        break;
            //    case "Gender":
            //        contract = client.getGender(callContext);
            //        break;
            //    case "NationalIdType":
            //        contract = client.getNationalIdType(callContext);
            //        break;
            //    case "SearchPatientFilter":
            //        contract = client.getSearchPatientFilter(callContext);
            //        break;
            //    case "ActivityStatus":
            //        contract = client.getActivityStatus(callContext, HMActivityStatus.None);
            //        break;
            //}

            //client.Close();

            //return contract;

            try
            {
                var endpointAddress = SoapHelper.GetEndPointAddress();
                var binding = SoapHelper.GetBinding();

                var client = new HMDropDownServiceClient(binding, endpointAddress);
                var channel = client.InnerChannel;

                using (OperationContextScope operationContextScope = new OperationContextScope(channel))
                {
                    SoapHelper.channelHelper();

                    switch (type)
                    {
                        case "DocumentType":
                            getDocumentTypesResponse objDoc = ((HMDropDownService)channel).getDocumentTypes(new getDocumentTypes() { });

                            if (objDoc.result != null)
                            {
                                contract = objDoc.result;
                            }
                            else
                            {
                                new ValidationException(CommonRepository.getErrorMessage(objDoc.Infolog));
                            }

                            break;

                        case "Gender":
                            getGenderResponse objGender = ((HMDropDownService)channel).getGender(new getGender() { });

                            if (objGender.result != null)
                            {
                                contract = objGender.result;
                            }
                            else
                            {
                                new ValidationException(CommonRepository.getErrorMessage(objGender.Infolog));
                            }

                            break;

                        case "NationalIdType":
                            getNationalIdTypeResponse objNationalIdType = ((HMDropDownService)channel).getNationalIdType(new getNationalIdType() { });

                            if (objNationalIdType.result != null)
                            {
                                contract = objNationalIdType.result;
                            }
                            else
                            {
                                new ValidationException(CommonRepository.getErrorMessage(objNationalIdType.Infolog));
                            }

                            break;

                        case "SearchPatientFilter":
                            getSearchPatientFilterResponse objPatientFilter = ((HMDropDownService)channel).getSearchPatientFilter(new getSearchPatientFilter() { });

                            if (objPatientFilter.result != null)
                            {
                                contract = objPatientFilter.result;
                            }
                            else
                            {
                                new ValidationException(CommonRepository.getErrorMessage(objPatientFilter.Infolog));
                            }

                            break;

                        case "ActivityStatus":
                            getActivityStatusResponse objActivityStatus = ((HMDropDownService)channel).getActivityStatus(new getActivityStatus() { });

                            if (objActivityStatus.result != null)
                            {
                                contract = objActivityStatus.result;
                            }
                            else
                            {
                                new ValidationException(CommonRepository.getErrorMessage(objActivityStatus.Infolog));
                            }

                            break;
                    }                                       
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return contract;
        }        
    }
}
