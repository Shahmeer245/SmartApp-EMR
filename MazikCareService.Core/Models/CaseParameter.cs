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
    public class CaseParameter
    {
        public bool diagnosisRequired
        {
            get; set;
        }

        public string urgencyId
        {
            get; set;
        }
        public string urgencyName
        {
            get; set;
        }
        public CaseParameter getParameters(mzk_casetype caseType)
        {
            CaseParameter model = new CaseParameter();

            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();
                QueryExpression query = new QueryExpression(mzk_caseparamter.EntityLogicalName);

                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                query.Criteria.AddCondition("mzk_casetype", ConditionOperator.Equal, (int)caseType);

                EntityCollection entitycollection = repo.GetEntityCollection(query);

                if (entitycollection != null && entitycollection.Entities != null && entitycollection.Entities.Count > 0)
                {
                    mzk_caseparamter entity = (mzk_caseparamter)entitycollection.Entities[0];
                    model.diagnosisRequired = entity.mzk_DiagnosisRequired.HasValue ? entity.mzk_DiagnosisRequired.Value : false;
                    model.urgencyId = entity.mzk_Urgency != null ? entity.mzk_Urgency.Value.ToString() : "";
                    model.urgencyName = entity.FormattedValues["mzk_urgency"].ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return model;
        }

        public static CaseParameter getDefaultUrgency(mzk_casetype caseType)
        {
            try
            {
                CaseParameter model = new CaseParameter();

                model = model.getParameters(caseType);

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool getDiagnosisRequired(mzk_casetype caseType)
        {
            try
            {
                CaseParameter model = new CaseParameter();

                model = model.getParameters(caseType);

                return model.diagnosisRequired;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
