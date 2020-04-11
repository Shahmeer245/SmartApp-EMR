using MazikCareService.AXRepository;
using MazikCareService.CRMRepository;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xrm;

namespace MazikCareService.Core.Models
{
    public class PatientInsurance
    {
        public string patientInsuranceId { get; set; }

        public string contactAxRefrecid { get; set; }

        public string insuranceCarrierAxRefrecid { get; set; }

        public async Task<long> createPatientInsurance(string _contactAxRefrecid, string _insuranceCarrierAxRefrecid)
        {
            try
            {
                long insuranceRecId = 0;
                long patientRecId = 0;

                insuranceRecId = (long)Convert.ToDecimal(_insuranceCarrierAxRefrecid);
                patientRecId = (long)Convert.ToDecimal(_contactAxRefrecid);

                PatientRepository patientRepo = new PatientRepository();

                //SoapEntityRepository repo = SoapEntityRepository.GetService();
                //QueryExpression query = new QueryExpression(mzk_patientinsurancecarrier.EntityLogicalName);

                //query.Criteria.AddCondition("mzk_patientinsurancecarrierid", ConditionOperator.Equal, new Guid(patientInsuranceId));
                //query.ColumnSet = new ColumnSet(false);

                //LinkEntity contact = new LinkEntity("mzk_patientinsurancecarrier", "contact", "mzk_customerid", "contactid", JoinOperator.Inner);
                //contact.Columns = new ColumnSet("mzk_axrefrecid");
                //contact.EntityAlias = "contact";

                //LinkEntity insuranceCarrier = new LinkEntity("mzk_patientinsurancecarrier", "mzk_insurancecarrier", "mzk_insurancecarrier", "mzk_insurancecarrierid", JoinOperator.Inner);
                //insuranceCarrier.Columns = new ColumnSet("mzk_axrefrecid");
                //insuranceCarrier.EntityAlias = "insuranceCarrier";

                //query.LinkEntities.Add(contact);
                //query.LinkEntities.Add(insuranceCarrier);                

                //EntityCollection entityCollection = repo.GetEntityCollection(query);

                //if (entityCollection != null && entityCollection.Entities != null && entityCollection.Entities.Count > 0)
                //{
                //    if (entityCollection.Entities[0].Attributes.Contains("contact.mzk_axrefrecid"))
                //    {
                //        patientRecId = (long) Convert.ToDecimal((entityCollection.Entities[0].Attributes["contact.mzk_axrefrecid"] as AliasedValue).Value);
                //    }
                //    if (entityCollection.Entities[0].Attributes.Contains("insuranceCarrier.mzk_axrefrecid"))
                //    {
                //        insuranceRecId = (long)Convert.ToDecimal((entityCollection.Entities[0].Attributes["insuranceCarrier.mzk_axrefrecid"] as AliasedValue).Value);
                //    } 
                //}

                return patientRepo.createPatientInsurance(insuranceRecId, patientRecId);

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
