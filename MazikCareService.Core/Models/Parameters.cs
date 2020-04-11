using MazikCareService.CRMRepository;
using MazikLogger;
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
    public class Parameters
    {
        public string clinicalTemplateId
        {
            get; set;
        }

        public string mmtgroupId
        {
            get; set;
        }

        public Parameters getParameters()
        {
            Parameters model = new Parameters();

            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();
                QueryExpression query = new QueryExpression(mzk_parameter.EntityLogicalName);

                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                query.AddOrder("createdon", OrderType.Ascending);

                EntityCollection entitycollection = repo.GetEntityCollection(query);
               
                if(entitycollection != null && entitycollection.Entities != null && entitycollection.Entities.Count > 0)
                {
                    mzk_parameter entity = (mzk_parameter) entitycollection.Entities[0];
                    model.clinicalTemplateId = entity.mzk_Clinicaltemplate != null ? entity.mzk_Clinicaltemplate.Id.ToString() : "";
                    model.mmtgroupId = entity.mzk_Measurementgroup != null ? entity.mzk_Measurementgroup.Id.ToString() : "";
                }                
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return model;
        }
    }
}
