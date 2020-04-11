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
    public class UITemplate
    {
        public string controlName { get; set; }

        public int order { get; set; }

        public mzk_uitemplatemzk_DefaultVitals defaultVitals { get; set; }

        public mzk_uitemplatemzk_DefaultClinicalTemplates defaultClinicalTemplate { get; set; }

        public async Task<List<UITemplate>> getChartTemplate(string userId)
        {
            List<UITemplate> listModel = new List<UITemplate>();
            UITemplate model;

            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();

                QueryExpression query = new QueryExpression(mzk_uitemplatesmenulist.EntityLogicalName);

                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_ordering");
                query.AddOrder("mzk_ordering", OrderType.Ascending);

                LinkEntity linkEntity = new LinkEntity(mzk_uitemplatesmenulist.EntityLogicalName, mzk_uitemplate.EntityLogicalName, "mzk_uitemplateid", "mzk_uitemplateid", JoinOperator.Inner);
                
                linkEntity.LinkCriteria.AddCondition("mzk_type", ConditionOperator.Equal, 2);

                FilterExpression childFilter = linkEntity.LinkCriteria.AddFilter(LogicalOperator.Or);

                childFilter.AddCondition("mzk_role", ConditionOperator.Null);

                List<Role> roleList = new Role().getRoles(userId);

                foreach (Role entity in roleList)
                {
                    if (entity.roleid != null && entity.roleid != Guid.Empty)
                    {
                        childFilter.AddCondition("mzk_role", ConditionOperator.Equal, entity.roleid);
                    }                    
                }

                query.LinkEntities.Add(linkEntity);                                              

                LinkEntity EntityList = new LinkEntity(mzk_uitemplatesmenulist.EntityLogicalName, mzk_menulist.EntityLogicalName, "mzk_menulist", "mzk_menulistid", JoinOperator.Inner);

                EntityList.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                EntityList.EntityAlias = mzk_menulist.EntityLogicalName;

                query.LinkEntities.Add(EntityList);

                EntityCollection entitycollection = repo.GetEntityCollection(query);

                foreach (Entity entity in entitycollection.Entities)
                {
                    model = new UITemplate();

                    if (entity.Attributes.Contains("mzk_menulist.mzk_chartmenulist"))
                    {
                        model.controlName = entity.FormattedValues["mzk_menulist.mzk_chartmenulist"].ToString();

                        if (entity.Attributes.Contains("mzk_ordering"))
                        {
                            model.order = Convert.ToInt32(entity.Attributes["mzk_ordering"].ToString());
                        }
                        else
                        {
                            model.order = 0;
                        }

                        listModel.Add(model);
                    }
                }
            }
            catch (CustomException ex)
            {
                throw ex;
            }

            return listModel;
        }

        public async Task<UITemplate> getUITemplateDetails(string uiTempalteId)
        {
            UITemplate model = null;

            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();

                mzk_uitemplate uitemplate = (mzk_uitemplate )repo.GetEntity(mzk_uitemplate.EntityLogicalName, new Guid(uiTempalteId), new ColumnSet(true));
                
                if (uitemplate != null)
                {
                    model = new UITemplate();

                    model.defaultClinicalTemplate = uitemplate.mzk_DefaultClinicalTemplates == null ? mzk_uitemplatemzk_DefaultClinicalTemplates.AddManually : (mzk_uitemplatemzk_DefaultClinicalTemplates)uitemplate.mzk_DefaultClinicalTemplates.Value;
                    model.defaultVitals = uitemplate.mzk_DefaultVitals == null ? mzk_uitemplatemzk_DefaultVitals.AddManually : (mzk_uitemplatemzk_DefaultVitals)uitemplate.mzk_DefaultVitals.Value;
                }
            }
            catch (CustomException ex)
            {
                throw ex;
            }

            return model;
        }

    }
}
