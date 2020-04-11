using Helper;
using MazikCareService.AXRepository;
using MazikCareService.AXRepository.AXServices;
using MazikCareService.Core.Interfaces;
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
    public class Clinic : IClinic
    {
        public string id
        {
            get; set;
        }

        public string clinicName
        {
            get; set;
        }

        public string clinicCode
        {
            get; set;
        }

        public string speciality
        {
            get; set;
        }

        public List<Clinic> subClinics
        {
            get; set;
        }

        public long mzk_axclinicrefrecid { get; set; }

        public async Task<List<Clinic>> getUserClinicsTree(string resourceId = null)
        {
            List<Clinic> listModel = new List<Clinic>();
            Clinic model;

            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();
                QueryExpression query = new QueryExpression(mzk_resourceclinic.EntityLogicalName);

                query.Criteria.AddCondition("mzk_resource", ConditionOperator.Equal, new Guid(resourceId));

                query.ColumnSet = new ColumnSet(true);

                EntityCollection entityCollection = repo.GetEntityCollection(query);

                foreach (Entity entity in entityCollection.Entities)
                {
                    mzk_resourceclinic resourceClinic = (mzk_resourceclinic)entity;

                    model = new Clinic();

                    if (entity.Attributes.Contains("mzk_organizationalunit"))
                    {
                        model.id = resourceClinic.mzk_OrganizationalUnit.Id.ToString();
                        model.clinicName = resourceClinic.mzk_OrganizationalUnit.Name;

                        listModel.Add(model);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listModel;
        }

        public List<Clinic> getClinicsList(mzk_orgunittype clinicType)
        {
            List<Clinic> listModel = new List<Clinic>();
            Clinic model;

            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();
                QueryExpression query = new QueryExpression(msdyn_organizationalunit.EntityLogicalName);

                query.Criteria.AddCondition("mzk_type", ConditionOperator.Equal, (int)clinicType);
                query.Criteria.AddCondition("mzk_isscheduled", ConditionOperator.Equal, true);
                
                query.ColumnSet = new ColumnSet("msdyn_organizationalunitid", "msdyn_name");

                EntityCollection entityCollection = repo.GetEntityCollection(query);

                foreach (Entity entity in entityCollection.Entities)
                {
                    msdyn_organizationalunit orgUnit = (msdyn_organizationalunit)entity;
                    
                    model = new Clinic();

                    if (entity.Attributes.Contains("mzk_organizationalunitid"))
                    {
                        model.id = orgUnit.msdyn_organizationalunitId.Value.ToString();
                        model.clinicName = orgUnit.msdyn_name;

                        listModel.Add(model);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listModel;
        }

        public string getClinicName(string clinicId = null)
        {
            return Convert.ToString(this.getClinicDetails(clinicId).clinicName);
        }

        public Clinic getClinicDetails(string clinicId = null)
        {
            Clinic clinic = new Clinic();

            SoapEntityRepository repo = SoapEntityRepository.GetService();
            QueryExpression query = new QueryExpression(msdyn_organizationalunit.EntityLogicalName);

            query.Criteria.AddCondition("msdyn_organizationalunitid", ConditionOperator.Equal, new Guid(clinicId));

            query.ColumnSet = new ColumnSet(true);

            EntityCollection entityCollection = repo.GetEntityCollection(query);

            if (entityCollection != null && entityCollection.Entities != null && entityCollection.Entities.Count > 0)
            {
                if (entityCollection.Entities[0].Attributes.Contains("msdyn_name"))
                {
                    clinic.clinicName = entityCollection.Entities[0].Attributes["msdyn_name"].ToString();
                }
                if (entityCollection.Entities[0].Attributes.Contains("mzk_speciality"))
                {
                    clinic.speciality = entityCollection.Entities[0].GetAttributeValue<EntityReference>("mzk_speciality").Name;
                }

                clinic.mzk_axclinicrefrecid = 0;

                if (entityCollection.Entities[0].Attributes.Contains("mzk_axclinicrefrecid"))
                {
                    clinic.mzk_axclinicrefrecid = Int64.Parse(entityCollection.Entities[0].Attributes["mzk_axclinicrefrecid"].ToString());
                }
            }

            return clinic;
        }

        List<Clinic> fillClinicTreeModel(HMClinicDataContract[] _contract, long _parentRecId)
        {
            List<Clinic> subClinicList = new List<Clinic>();
            Clinic model;

            var childClinics = _contract.Where(item => item.parmClinicRecIdParent == _parentRecId);

            foreach (HMClinicDataContract contract in childClinics)
            {
                var parentClinics = _contract.Where(item => item.parmClinicRecIdParent == contract.parmClinicRecId);

                if (parentClinics != null && parentClinics.Count() > 0)
                {
                    model = new Clinic();

                    model.id = contract.parmClinicRecId.ToString();
                    model.clinicName = contract.parmClinicName;
                    model.subClinics = this.fillClinicTreeModel(_contract, contract.parmClinicRecId);

                    subClinicList.Add(model);
                }
                else
                {
                    model = new Clinic();

                    model.id = contract.parmClinicRecId.ToString();
                    model.clinicName = contract.parmClinicName;

                    subClinicList.Add(model);
                }
            }

            return subClinicList;
        }
    }
}
