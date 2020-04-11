using MazikCareService.CRMRepository;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models
{
    public class Speciality
    {
        public string Description { get; set; }

        public string SpecialityId { get; set; }

        public long SpecialityRefRecId { get; set; }


        public List<Speciality> getSpecialityList(List<string> specialityIdList = null, mzk_locationtype _type = mzk_locationtype.All)
        {
            List<Speciality> Speciality = new List<Speciality>();
            #region Speciality Setup
            QueryExpression query = new QueryExpression("characteristic");

            if (_type == mzk_locationtype.External)
            {
                FilterExpression filterExpression = query.Criteria.AddFilter(LogicalOperator.Or);
                filterExpression.AddCondition("mzk_type", ConditionOperator.Equal, (int)mzk_locationtype.External);
                filterExpression.AddCondition("mzk_type", ConditionOperator.Equal, (int)mzk_locationtype.All);
            }
            else if (_type == mzk_locationtype.Internal)
            {
                FilterExpression filterExpression = query.Criteria.AddFilter(LogicalOperator.Or);
                filterExpression.AddCondition("mzk_type", ConditionOperator.Equal, (int)mzk_locationtype.Internal);
                filterExpression.AddCondition("mzk_type", ConditionOperator.Equal, (int)mzk_locationtype.All);
            }            

            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.Or);            

            if (specialityIdList != null)
            {
                foreach (string specialityId in specialityIdList)
                {
                    childFilter.AddCondition("characteristicid", ConditionOperator.Equal, new Guid(specialityId));
                }
            }

            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("name", "characteristicid", "mzk_axrefrecid");

            OrderExpression order = new OrderExpression();
            order.AttributeName = "name";
            order.OrderType = OrderType.Ascending;

            query.Orders.Add(order);

            #endregion
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            foreach (Entity entity in entitycollection.Entities)
            {
                Speciality model = new Speciality();
                model.SpecialityId = entity.Attributes["characteristicid"].ToString();
                model.Description = entity.Attributes["name"].ToString();

                if (entity.Attributes.Contains("mzk_axrefrecid"))
                {
                    model.SpecialityRefRecId = Convert.ToInt64(entity.Attributes["mzk_axrefrecid"]);
                }
                Speciality.Add(model);
            }
            return Speciality;
        }

        public List<string> getSpecialityList(string patientId, int caseType, int encounterType)
        {
            List<string> Speciality = new List<string>();

            QueryExpression query = new QueryExpression("incident");

            if (!string.IsNullOrEmpty(patientId))
            {
                query.Criteria.AddCondition("customerid", ConditionOperator.Equal, new Guid(patientId));
            }
            if (caseType != 0)
            {
                query.Criteria.AddCondition("mzk_casetype", ConditionOperator.Equal, caseType);
            }

            LinkEntity queryEncounter = new LinkEntity("incident", "mzk_patientencounter", "incidentid", "mzk_caseid", JoinOperator.Inner);
            if (encounterType != 0)
            {
                queryEncounter.LinkCriteria.AddCondition("mzk_encountertype", ConditionOperator.Equal, encounterType);
            }

            LinkEntity queryClinic = new LinkEntity("incident", "mzk_organizationalunit", "mzk_organizationalunit", "mzk_organizationalunitid", JoinOperator.Inner);
            queryClinic.Columns = new ColumnSet(false);
            queryClinic.EntityAlias = "clinic";

            LinkEntity querySpeciality = new LinkEntity("mzk_organizationalunit", "characteristic", "mzk_speciality", "characteristicid", JoinOperator.Inner);
            querySpeciality.Columns = new ColumnSet("characteristicid");
            querySpeciality.EntityAlias = "Speciality";

            query.LinkEntities.Add(queryEncounter);
            query.LinkEntities.Add(queryClinic);
            queryClinic.LinkEntities.Add(querySpeciality);

            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            foreach (Entity entity in entitycollection.Entities)
            {
                Speciality model = new Speciality();

                if (entity.Attributes.Contains("Speciality.characteristicid"))
                    model.SpecialityId = (entity.Attributes["Speciality.characteristicid"] as AliasedValue).Value.ToString();                
                
                Speciality.Add(model.SpecialityId);
            }
            return Speciality;
        }

        public string getSpeciality(string caseid)
        {
            string specialityName = string.Empty;

            QueryExpression query = new QueryExpression("incident");

            if (!string.IsNullOrEmpty(caseid))
            {
                query.Criteria.AddCondition("incidentid", ConditionOperator.Equal, new Guid(caseid));
            }

            LinkEntity queryClinic = new LinkEntity("incident", "mzk_organizationalunit", "mzk_organizationalunit", "mzk_organizationalunitid", JoinOperator.Inner);
            queryClinic.Columns = new ColumnSet("mzk_speciality");
            queryClinic.EntityAlias = "clinic";

            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            if(entitycollection.Entities[0] != null)
            {
                if (entitycollection.Entities[0].Attributes.Contains("clinic.mzk_speciality"))
                    specialityName = ((EntityReference)(entitycollection.Entities[0].Attributes["clinic.mzk_speciality"] as AliasedValue).Value).Name;              
            }

            return specialityName;
        }

    }
}
