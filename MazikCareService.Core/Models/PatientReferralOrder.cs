using Helper;
using MazikCareService.AXRepository.AXServices;
using MazikCareService.Core.Interfaces;
using MazikCareService.CRMRepository;
using MazikCareService.CRMRepository.Microsoft.Dynamics.CRM;
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
    public class PatientReferralOrder : PatientOrder, IPatientReferralOrder
    {

        private EntityRepository<Mzk_patientorder> _entityRep;

        public PatientReferralOrder()
        {
            _entityRep = new EntityRepository<Mzk_patientorder>();
        }
        public string ReferralNumber { get; set; }
        public string Diagnosis { get; set; }
        public string DiagnosisName { get; set; }
        public string ReferralId { get; set; }
        public string Referral { get; set; }
        public string SpecialtyName { get; set; }
        public string SpecialtyId { get; set; }
        public string SpecialtyRecId { get; set; }
        public string PatientRecId { get; set; }
        public string ResourceRecId { get; set; }
        public string RefPhysician { get; set; }
        public string RefPhysicianResourceId { get; set; }
        public string Referralto { get; set; }
        public string ReferringPhysician { get; set; }
        public DateTime ReferralDate { get; set; }
        public string ReferralComment { get; set; }
        public string PatientAware { get; set; }
        public int Apptrecommendation { get; set; }
        public string ApptrecommendationName { get; set; }
        public string Address { get; set; }
        public int ReferralType { get; set; }
        public string ReferralTypeName { get; set; }
        public int ReferralName { get; set; }
        public int Category { get; set; }
        public string CategoryName { get; set; }
        public int Type { get; set; }
        public int TypeName { get; set; }
        public int currentpage { get; set; }
        public string patientEncounterId { get; set; }
        public string UserId { get; set; }
        public string HospitalId { get; set; }
        public string HospitalName { get; set; }
        public List<ReferringPhysician> referringPhysicianList
        {
            get; set;
        }
        public List<User> usersList
        {
            get; set;
        }

        public async Task<List<PatientReferralOrder>> getPatientOrder(string patientguid, string patientEncounter, string SearchFilters, string searchOrder, DateTime startDate, DateTime endDate, bool forFulfillment, string orderId, string caseId = null, int pageNumber = 0, string fulfillmentAppointmentId = null, string orderingAppointmentId = null)
        {
            try
            {
                List<PatientReferralOrder> PatientReferral = new List<PatientReferralOrder>();
                #region Patient Referral Query
                QueryExpression query = new QueryExpression(mzk_patientorder.EntityLogicalName);
                FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);

                if (SearchFilters != mzk_orderstatus.Cancelled.ToString())
                {
                    childFilter.AddCondition("mzk_orderstatus", ConditionOperator.NotEqual, (int)mzk_orderstatus.Cancelled);
                }
                if (!string.IsNullOrEmpty(orderId))
                {
                    childFilter.AddCondition("mzk_patientorderid", ConditionOperator.Equal, new Guid(orderId));
                }
                if (!string.IsNullOrEmpty(fulfillmentAppointmentId))
                {
                    childFilter.AddCondition("mzk_fulfillmentappointment", ConditionOperator.Equal, new Guid(fulfillmentAppointmentId));
                }
                if (!string.IsNullOrEmpty(orderingAppointmentId))
                {
                    childFilter.AddCondition("mzk_orderingappointment", ConditionOperator.Equal, new Guid(orderingAppointmentId));
                }
                if (!string.IsNullOrEmpty(caseId))
                {
                    childFilter.AddCondition("mzk_caseid", ConditionOperator.Equal, new Guid(caseId));
                }
                if (!string.IsNullOrEmpty(patientguid))
                {
                    childFilter.AddCondition("mzk_customer", ConditionOperator.Equal, new Guid(patientguid));                    
                }
                else
                {
                    if (!string.IsNullOrEmpty(patientEncounter))
                    {
                        childFilter.AddCondition("mzk_patientencounterid", ConditionOperator.Equal, new Guid(patientEncounter));
                    }
                }

                //Search Filter
                if (!string.IsNullOrEmpty(SearchFilters))
                {
                    if (SearchFilters == Convert.ToString(mzk_referralfilter.Consultation))
                        childFilter.AddCondition("mzk_referraltype", ConditionOperator.Equal, Convert.ToInt32(mzk_patientordermzk_ReferralType.Consultation));
                    if (SearchFilters == "Consultation & Treatment")
                        childFilter.AddCondition("mzk_referraltype", ConditionOperator.Equal, Convert.ToInt32(mzk_patientordermzk_ReferralType.ConsultationTreatment));
                    if (SearchFilters == "Transfer of Care")
                        childFilter.AddCondition("mzk_referraltype", ConditionOperator.Equal, Convert.ToInt32(mzk_patientordermzk_ReferralType.TransferofCare));
                }

                //Search Date
                if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                    childFilter.AddCondition("createdon", ConditionOperator.Between, new Object[] { startDate, endDate.AddHours(12) });

                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_caseid",
                                                                        "mzk_fulfillmentappointment",
                                                                        "mzk_orderingappointment",
                                                                        "mzk_fulfillmentappointment",
                                                                        "mzk_customer",
                                                                        "mzk_appointmentrecommendation",
                                                                        "mzk_associateddiagnosisid",
                                                                        "mzk_patientaware",
                                                                        "mzk_comments",
                                                                        "mzk_patientencounterid",
                                                                        "mzk_orderstatus",
                                                                        "mzk_referraltoid",
                                                                        "mzk_referraltype",
                                                                        "mzk_specialityid",
                                                                        "ownerid",
                                                                        "mzk_patientordernumber",
                                                                        "createdon",
                                                                        "mzk_referralcategory",
                                                                        "mzk_referraltoexternalid",
                                                                        "mzk_hospitalid",
                                                                        "mzk_treatmentlocation", "mzk_orderinglocation");

                LinkEntity EntitySpecialty = new LinkEntity("mzk_patientorder", "characteristic", "mzk_specialityid", "characteristicid", JoinOperator.Inner);
                EntitySpecialty.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                EntitySpecialty.EntityAlias = "mzk_speciality";

                LinkEntity EntityDiagnosis = new LinkEntity("mzk_patientorder", "mzk_concept", "mzk_associateddiagnosisid", "mzk_conceptid", JoinOperator.LeftOuter);
                EntityDiagnosis.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_conceptname", "mzk_icdcodeid");
                EntityDiagnosis.EntityAlias = "mzk_conceptDiagnosis";

                LinkEntity EntityReferringPhysician = new LinkEntity("mzk_patientorder", "mzk_referringphysician", "mzk_referraltoexternalid", "mzk_referringphysicianid", JoinOperator.LeftOuter);
                EntityReferringPhysician.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                EntityReferringPhysician.EntityAlias = "ReferringPhysician";

                LinkEntity EntityInternalPhysician = new LinkEntity("mzk_patientorder", "systemuser", "mzk_referraltoid", "systemuserid", JoinOperator.LeftOuter);
                EntityInternalPhysician.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_axresourcerefrecid");
                EntityInternalPhysician.EntityAlias = "InternalPhysician";

                LinkEntity EntityOrderingPhysician = new LinkEntity("mzk_patientorder", "systemuser", "createdby", "systemuserid", JoinOperator.LeftOuter);
                EntityOrderingPhysician.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_axresourcerefrecid");
                EntityOrderingPhysician.EntityAlias = "OrderingPhysician";

                if (!string.IsNullOrEmpty(searchOrder))
                {
                    FilterExpression filter1 = EntityReferringPhysician.LinkCriteria.AddFilter(LogicalOperator.And);
                    filter1.AddCondition("mzk_name", ConditionOperator.Like, ("%" + searchOrder + "%"));
                    EntityReferringPhysician.LinkCriteria.AddFilter(filter1);
                }

                OrderExpression orderby = new OrderExpression();
                orderby.AttributeName = "createdon";
                orderby.OrderType = OrderType.Descending;
                query.Orders.Add(orderby);

                query.LinkEntities.Add(EntitySpecialty);
                query.LinkEntities.Add(EntityOrderingPhysician);
                query.LinkEntities.Add(EntityDiagnosis);
                query.LinkEntities.Add(EntityReferringPhysician);
                query.LinkEntities.Add(EntityInternalPhysician);

                if (!forFulfillment && pageNumber > 0)
                {
                    query.PageInfo = new Microsoft.Xrm.Sdk.Query.PagingInfo();
                    query.PageInfo.Count = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                    query.PageInfo.PageNumber = pageNumber;
                    query.PageInfo.PagingCookie = null;
                    query.PageInfo.ReturnTotalRecordCount = true;
                }

                #endregion

                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
                string Name = string.Empty;
                foreach (Entity entity in entitycollection.Entities)
                {
                    if (entity.Contains("mzk_referraltoid"))
                        Name = ((EntityReference)entity["mzk_referraltoid"]).Name;
                    else
                        if (entity.Contains("ReferringPhysician.mzk_name"))
                        Name = ((AliasedValue)entity["ReferringPhysician.mzk_name"]).Value.ToString();

                    if (!string.IsNullOrEmpty(searchOrder))
                    {
                        if (Name.ToLower().Contains(searchOrder.ToLower()))
                        {
                            PatientReferralOrder model = new PatientReferralOrder();

                            if (!this.getPatientOrder(model, entity, forFulfillment, orderId, mzk_entitytype.ReferralOrder))
                            {
                                continue;
                            }

                            if (entity.Attributes.Contains("mzk_specialityid"))
                            {
                                model.SpecialtyId = ((EntityReference)entity["mzk_specialityid"]).Id.ToString();
                                model.SpecialtyName = ((EntityReference)entity["mzk_specialityid"]).Name;
                            }

                            if (entity.Attributes.Contains("ownerid"))
                                model.RefPhysician = ((EntityReference)entity["ownerid"]).Name;

                            if (entity.Attributes.Contains("createdon"))
                                model.ReferralDate = Convert.ToDateTime(entity.Attributes["createdon"]);
                            if (entity.Attributes.Contains("mzk_comments"))
                                model.ReferralComment = entity["mzk_comments"].ToString();
                            if (entity.Attributes.Contains("mzk_patientaware") && Convert.ToBoolean(entity["mzk_patientaware"]) == true)
                                model.PatientAware = "1";
                            else
                                model.PatientAware = "0";

                            if (entity.Attributes.Contains("mzk_appointmentrecommendation"))
                            {
                                model.Apptrecommendation = ((OptionSetValue)entity.Attributes["mzk_appointmentrecommendation"]).Value;
                                model.ApptrecommendationName = entity.FormattedValues["mzk_appointmentrecommendation"].ToString();
                            }

                            if (entity.Attributes.Contains("mzk_referraltype"))
                            {
                                model.ReferralType = ((OptionSetValue)entity["mzk_referraltype"]).Value;
                                model.ReferralTypeName = entity.FormattedValues["mzk_referraltype"].ToString();
                            }

                            if (entity.Attributes.Contains("mzk_referralcategory"))
                            {
                                model.Category = ((OptionSetValue)entity["mzk_referralcategory"]).Value;
                                model.CategoryName = entity.FormattedValues["mzk_referralcategory"].ToString();

                                if (((OptionSetValue)entity["mzk_referralcategory"]).Value == Convert.ToInt32(mzk_patientordermzk_ReferralCategory.Internal))
                                {
                                    if (entity.Attributes.Contains("mzk_referraltoid"))
                                        model.Referralto = ((EntityReference)entity["mzk_referraltoid"]).Name;
                                }
                                else
                                {
                                    if (((OptionSetValue)entity["mzk_referralcategory"]).Value == Convert.ToInt32(mzk_patientordermzk_ReferralCategory.External))
                                    {
                                        if (entity.Attributes.Contains("ReferringPhysician.mzk_name"))
                                            model.Referralto = ((AliasedValue)entity["ReferringPhysician.mzk_name"]).Value.ToString();
                                    }
                                }
                            }

                            if (entity.Attributes.Contains("mzk_associateddiagnosisid"))
                                model.Diagnosis = ((EntityReference)entity.Attributes["mzk_associateddiagnosisid"]).Id.ToString();

                            if (entity.Attributes.Contains("mzk_hospitalid"))
                            {
                                model.HospitalId = ((EntityReference)entity.Attributes["mzk_hospitalid"]).Id.ToString();
                                model.HospitalName = ((EntityReference)entity.Attributes["mzk_hospitalid"]).Name.ToString();
                            }

                            if (entity.Attributes.Contains("mzk_conceptDiagnosis.mzk_conceptname"))
                                model.DiagnosisName = ((AliasedValue)entity.Attributes["mzk_conceptDiagnosis.mzk_conceptname"]).Value.ToString();

                            if (entity.Attributes.Contains("mzk_conceptDiagnosis.mzk_icdcodeid"))
                                model.ICDCode = ((EntityReference)((AliasedValue)entity.Attributes["mzk_conceptDiagnosis.mzk_icdcodeid"]).Value).Name;

                            PatientReferral.Add(model);
                        }
                    }
                    else
                    {
                        PatientReferralOrder model = new PatientReferralOrder();

                        if (!this.getPatientOrder(model, entity, forFulfillment, orderId, mzk_entitytype.ReferralOrder))
                        {
                            continue;
                        }

                        if (entity.Attributes.Contains("mzk_specialityid"))
                        {
                            model.SpecialtyId = ((EntityReference)entity["mzk_specialityid"]).Id.ToString();
                            model.SpecialtyName = ((EntityReference)entity["mzk_specialityid"]).Name;
                        }
                        
                        if (entity.Attributes.Contains("ownerid"))
                            model.RefPhysician = ((EntityReference)entity["ownerid"]).Name;

                        if (entity.Attributes.Contains("OrderingPhysician.mzk_axresourcerefrecid"))
                        {
                            model.RefPhysicianResourceId = ((AliasedValue)entity.Attributes["OrderingPhysician.mzk_axresourcerefrecid"]).Value.ToString();
                        }

                        if (entity.Attributes.Contains("createdon"))
                            model.ReferralDate = Convert.ToDateTime(entity.Attributes["createdon"]);
                        if (entity.Attributes.Contains("mzk_comments"))
                            model.ReferralComment = entity["mzk_comments"].ToString();
                        if (entity.Attributes.Contains("mzk_patientaware") && Convert.ToBoolean(entity["mzk_patientaware"]) == true)
                            model.PatientAware = "1";
                        else
                            model.PatientAware = "0";

                        if (entity.Attributes.Contains("mzk_appointmentrecommendation"))
                        {
                            model.Apptrecommendation = ((OptionSetValue)entity.Attributes["mzk_appointmentrecommendation"]).Value;
                            model.ApptrecommendationName = entity.FormattedValues["mzk_appointmentrecommendation"].ToString();
                        }

                        if (entity.Attributes.Contains("mzk_referraltype"))
                        {
                            model.ReferralType = ((OptionSetValue)entity["mzk_referraltype"]).Value;
                            model.ReferralTypeName = entity.FormattedValues["mzk_referraltype"].ToString();
                        }

                        if (entity.Attributes.Contains("mzk_referralcategory"))
                        {
                            model.Category = ((OptionSetValue)entity["mzk_referralcategory"]).Value;
                            model.CategoryName = entity.FormattedValues["mzk_referralcategory"].ToString();

                            if (((OptionSetValue)entity["mzk_referralcategory"]).Value == Convert.ToInt32(mzk_patientordermzk_ReferralCategory.Internal))
                            {
                                if (entity.Attributes.Contains("mzk_referraltoid"))
                                    model.Referralto = ((EntityReference)entity["mzk_referraltoid"]).Name;

                                if (entity.Attributes.Contains("InternalPhysician.mzk_axresourcerefrecid"))
                                    model.ResourceRecId = ((AliasedValue)entity["InternalPhysician.mzk_axresourcerefrecid"]).Value.ToString();
                            }
                            else
                            {
                                if (((OptionSetValue)entity["mzk_referralcategory"]).Value == Convert.ToInt32(mzk_patientordermzk_ReferralCategory.External))
                                {
                                    if (entity.Attributes.Contains("ReferringPhysician.mzk_name"))
                                        model.Referralto = ((AliasedValue)entity["ReferringPhysician.mzk_name"]).Value.ToString();
                                }
                            }
                        }

                        if (entity.Attributes.Contains("mzk_hospitalid"))
                        {
                            model.HospitalId = ((EntityReference)entity.Attributes["mzk_hospitalid"]).Id.ToString();
                            model.HospitalName = ((EntityReference)entity.Attributes["mzk_hospitalid"]).Name.ToString();
                        }

                        if (entity.Attributes.Contains("mzk_associateddiagnosisid"))
                            model.Diagnosis = ((EntityReference)entity.Attributes["mzk_associateddiagnosisid"]).Id.ToString();

                        if (entity.Attributes.Contains("mzk_conceptDiagnosis.mzk_conceptname"))
                            model.DiagnosisName = ((AliasedValue)entity.Attributes["mzk_conceptDiagnosis.mzk_conceptname"]).Value.ToString();


                        if (entity.Attributes.Contains("mzk_conceptDiagnosis.mzk_icdcodeid"))
                            model.ICDCode = ((EntityReference)((AliasedValue)entity.Attributes["mzk_conceptDiagnosis.mzk_icdcodeid"]).Value).Name;


                        PatientReferral.Add(model);

                    }
                }

                if (pageNumber > 0 && entitycollection != null)
                {
                    Pagination.totalCount = entitycollection.TotalRecordCount;
                }

                return PatientReferral;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<List<PatientReferralOrder>> getPatientOrder(List<Entity> patientOrders)
        {
            List<PatientReferralOrder> PatientReferral = new List<PatientReferralOrder>();

            foreach (Entity entity in patientOrders)
            {
                PatientReferralOrder model = new PatientReferralOrder();

                if (entity.Attributes.Contains("mzk_speciality.name"))
                {
                    model.SpecialtyName = ((AliasedValue)entity.Attributes["mzk_speciality.name"]).Value.ToString();
                }

                if (entity.Attributes.Contains("mzk_comments"))
                    model.ReferralComment = entity["mzk_comments"].ToString();

                if (entity.Id != null)
                    model.Id = entity.Id.ToString();

                if (entity.Attributes.Contains("mzk_referralcategory"))
                {
                    model.Category = ((OptionSetValue)entity["mzk_referralcategory"]).Value;
                    model.CategoryName = entity.FormattedValues["mzk_referralcategory"].ToString();

                    if (((OptionSetValue)entity["mzk_referralcategory"]).Value == Convert.ToInt32(mzk_patientordermzk_ReferralCategory.Internal))
                    {
                        if (entity.Attributes.Contains("mzk_referraltoid"))
                            model.Referralto = ((EntityReference)entity["mzk_referraltoid"]).Name;
                    }
                    else
                    {
                        if (((OptionSetValue)entity["mzk_referralcategory"]).Value == Convert.ToInt32(mzk_patientordermzk_ReferralCategory.External))
                        {
                            if (entity.Attributes.Contains("ReferringPhysician.mzk_name"))
                                model.Referralto = ((AliasedValue)entity["ReferringPhysician.mzk_name"]).Value.ToString();
                        }
                    }
                }
                
                PatientReferral.Add(model);
            }

            return PatientReferral;
        }

        public async Task<string> addPatientOrder(PatientReferralOrder _patientReferral)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            mzk_patientorder referralEntity = new mzk_patientorder();
            try
            {
                referralEntity.mzk_appointable = true;

                if (!string.IsNullOrEmpty(_patientReferral.appointmentId))
                {
                    referralEntity.mzk_orderingappointment = new EntityReference("mzk_patientorder", new Guid(_patientReferral.appointmentId));
                    referralEntity.mzk_fulfillmentappointment = new EntityReference("mzk_patientorder", new Guid(_patientReferral.appointmentId));
                }

                if (!string.IsNullOrEmpty(_patientReferral.PatientId))
                {
                    referralEntity.Attributes["mzk_customer"] = new EntityReference("contact", new Guid(_patientReferral.PatientId));
                }

                if (!string.IsNullOrEmpty(_patientReferral.EncounterId))
                {
                    referralEntity.Attributes["mzk_patientencounterid"] = new EntityReference("mzk_patientencounter", new Guid(_patientReferral.EncounterId));
                    PatientEncounter encounter = new PatientEncounter();
                    encounter.EncounterId = _patientReferral.EncounterId;
                    //PatientId = new PatientCase().getCaseDetails(encounter.encounterDetails(encounter).Result.ToList().First<PatientEncounter>().CaseId).Result.PatientId;
                    PatientId = encounter.getEncounterDetails(encounter).Result.ToList().First<PatientEncounter>().PatientId;
                    referralEntity.Attributes["mzk_customer"] = new EntityReference("contact", new Guid(PatientId));
                }

                if (!string.IsNullOrEmpty(_patientReferral.Diagnosis))
                    referralEntity.Attributes["mzk_associateddiagnosisid"] = new EntityReference("mzk_concept", new Guid(_patientReferral.Diagnosis));

                if (_patientReferral.clinicRecId > 0)
                    referralEntity.Attributes["mzk_axclinicrefrecid"] = Convert.ToDecimal(_patientReferral.clinicRecId);

                if (!string.IsNullOrEmpty(_patientReferral.orderingLocationId))
                    referralEntity.Attributes["mzk_orderinglocation"] = new EntityReference("mzk_organizationalunit", new Guid(_patientReferral.orderingLocationId));

                if (!string.IsNullOrEmpty(_patientReferral.ReferralComment))
                    referralEntity.Attributes["mzk_comments"] = _patientReferral.ReferralComment;

                if (_patientReferral.PatientAware.ToString() != "1")
                    referralEntity.Attributes["mzk_patientaware"] = true;
                else
                    referralEntity.Attributes["mzk_patientaware"] = false;

                if (_patientReferral.Apptrecommendation != 0)
                    referralEntity.Attributes["mzk_appointmentrecommendation"] = new OptionSetValue(_patientReferral.Apptrecommendation);

                //for ReferralType Consultation, Consultation & Treatment and Transfer of Care
                if (_patientReferral.ReferralType != 0)
                    referralEntity.Attributes["mzk_referraltype"] = new OptionSetValue(_patientReferral.ReferralType);


                //for Category Internal /External
                if (_patientReferral.Category != 0 && _patientReferral.Category == Convert.ToInt32(mzk_patientordermzk_ReferralCategory.Internal))
                {
                    referralEntity.Attributes["mzk_referralcategory"] = new OptionSetValue(Convert.ToInt32(_patientReferral.Category));

                    if (!string.IsNullOrEmpty(_patientReferral.ReferralId))
                        referralEntity.Attributes["mzk_referraltoid"] = new EntityReference("systemuser", new Guid(_patientReferral.ReferralId));
                }
                else
                {
                    if (_patientReferral.Category != 0 && _patientReferral.Category == Convert.ToInt32(mzk_patientordermzk_ReferralCategory.External))
                    {
                        referralEntity.Attributes["mzk_referralcategory"] = new OptionSetValue(Convert.ToInt32(_patientReferral.Category));

                        if (!string.IsNullOrEmpty(_patientReferral.ReferralId))
                            referralEntity.Attributes["mzk_referraltoexternalid"] = new EntityReference("mzk_referringphysician", new Guid(_patientReferral.ReferralId));
                    }
                }

                if (!string.IsNullOrEmpty(_patientReferral.SpecialtyId))
                {
                    referralEntity.Attributes["mzk_specialityid"] = new EntityReference("characteristic", new Guid(_patientReferral.SpecialtyId));

                    var specialityNameList = new List<string> {_patientReferral.SpecialtyId};

                    Speciality speciality = new Speciality();

                    referralEntity.Attributes["mzk_specialtyname"] = speciality.getSpecialityList(specialityNameList).First<Speciality>().Description;

                }

                if (!string.IsNullOrEmpty(_patientReferral.HospitalId) && _patientReferral.HospitalId != "0")
                    referralEntity.Attributes["mzk_hospitalid"] = new EntityReference("mzk_hospital", new Guid(_patientReferral.HospitalId));

                referralEntity.Attributes["mzk_type"] = new OptionSetValue(Convert.ToInt32(mzk_patientordermzk_Type.Referral));
                referralEntity.Attributes["mzk_orderstatus"] = new OptionSetValue((int)mzk_orderstatus.Ordered);

                referralEntity.Attributes["mzk_orderdate"] = DateTime.Now.Date;
                referralEntity.Attributes["mzk_fulfillmentdate"] = referralEntity.Attributes["mzk_orderdate"];


                Id = Convert.ToString(entityRepository.CreateEntity(referralEntity));

                if (!string.IsNullOrEmpty(_patientReferral.EncounterId) && !string.IsNullOrEmpty(_patientReferral.SpecialtyId))
                {
                    mzk_casetype caseType = PatientCase.getCaseType(_patientReferral.EncounterId);

                    if (caseType == mzk_casetype.Emergency && _patientReferral.Category == Convert.ToInt32(mzk_patientordermzk_ReferralCategory.Internal))
                    {
                        List<string> specialtyIdList = new List<string>();
                        specialtyIdList.Add(_patientReferral.SpecialtyId);

                        Speciality sp = new Speciality().getSpecialityList(specialtyIdList).FirstOrDefault();

                        if (AppSettings.GetByKey("OperationsIntegration").ToLower() == true.ToString().ToLower())
                        {
                            if (sp != null && sp.SpecialityRefRecId > 0)
                            {
                                await this.createCaseTrans(_patientReferral.EncounterId, Id, "", mzk_orderstatus.Ordered, 1, "", HMUrgency.None, "", "", "", sp.SpecialityRefRecId);
                            }
                        }
                    }
                }

                return Id.ToString();
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    entityRepository.DeleteEntity(mzk_patientorder.EntityLogicalName, new Guid(Id));
                }

                throw ex;
            }
        }
        public async Task<bool> updatePatientOrder(PatientReferralOrder patientReferral)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                Entity referralEntity = entityRepository.GetEntity(mzk_patientorder.EntityLogicalName, new Guid(patientReferral.Id) { },
                    new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_customer",
                                                            "mzk_appointmentrecommendation",
                                                            "mzk_associateddiagnosisid",
                                                            "mzk_patientaware",
                                                            "mzk_comments",
                                                            "mzk_orderstatus",
                                                            "mzk_referraltoid",
                                                            "mzk_referraltype",
                                                            "mzk_specialityid",
                                                            "ownerid",
                                                            "mzk_patientordernumber",
                                                            "createdon",
                                                            "mzk_referralcategory",
                                                            "mzk_referraltoexternalid"));

                if (!string.IsNullOrEmpty(patientReferral.PatientId))
                    referralEntity.Attributes["mzk_customer"] = new EntityReference("contact", new Guid(patientReferral.PatientId));

                if (!string.IsNullOrEmpty(patientReferral.Diagnosis))
                    referralEntity.Attributes["mzk_associateddiagnosisid"] = new EntityReference("mzk_concept", new Guid(patientReferral.Diagnosis));

                if (!string.IsNullOrEmpty(patientReferral.ReferralComment))
                    referralEntity.Attributes["mzk_comments"] = patientReferral.ReferralComment;

                if (patientReferral.PatientAware.ToString() != "1")
                    referralEntity.Attributes["mzk_patientaware"] = true;
                else
                    referralEntity.Attributes["mzk_patientaware"] = false;

                if (patientReferral.Apptrecommendation != 0)
                    referralEntity.Attributes["mzk_appointmentrecommendation"] = new OptionSetValue(patientReferral.Apptrecommendation);

                //for ReferralType Consultation, Consultation & Treatment and Transfer of Care
                if (patientReferral.ReferralType != 0)
                    referralEntity.Attributes["mzk_referraltype"] = new OptionSetValue(patientReferral.ReferralType);

                //for Category Internal /External
                if (patientReferral.Category != 0 && patientReferral.Category == Convert.ToInt32(mzk_patientordermzk_ReferralCategory.Internal))
                {
                    referralEntity.Attributes["mzk_referralcategory"] = new OptionSetValue(Convert.ToInt32(patientReferral.Category));

                    if (!string.IsNullOrEmpty(patientReferral.ReferralId))
                        referralEntity.Attributes["mzk_referraltoid"] = new EntityReference("systemuser", new Guid(patientReferral.ReferralId));
                }
                else
                {
                    if (patientReferral.Category != 0 && patientReferral.Category == Convert.ToInt32(mzk_patientordermzk_ReferralCategory.External))
                    {
                        referralEntity.Attributes["mzk_referralcategory"] = new OptionSetValue(Convert.ToInt32(patientReferral.Category));

                        if (!string.IsNullOrEmpty(patientReferral.ReferralId))
                            referralEntity.Attributes["mzk_referraltoexternalid"] = new EntityReference("mzk_referringphysician", new Guid(patientReferral.ReferralId));
                    }
                }
                if (!string.IsNullOrEmpty(patientReferral.SpecialtyId))
                {
                    referralEntity.Attributes["mzk_specialityid"] = new EntityReference("characteristic", new Guid(patientReferral.SpecialtyId));

                    var specialityNameList = new List<string> {patientReferral.SpecialtyId};

                    Speciality speciality = new Speciality();

                    referralEntity.Attributes["mzk_specialtyname"] = speciality.getSpecialityList(specialityNameList).First<Speciality>().Description;

                }
                
                if (!string.IsNullOrEmpty(patientReferral.HospitalId))
                    referralEntity.Attributes["mzk_hospitalid"] = new EntityReference("mzk_hospital", new Guid(patientReferral.HospitalId));

                entityRepository.UpdateEntity(referralEntity);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<List<PatientReferralOrder>> getReferral(PatientReferralOrder referral)
        {
            List<PatientReferralOrder> PatientReferral = new List<PatientReferralOrder>();
            List<Speciality> speciality = new Speciality().getSpecialityList(null, referral.CategoryName == mzk_patientordermzk_ReferralCategory.External.ToString() ? mzk_locationtype.External : mzk_locationtype.Internal).ToList<Speciality>();

            foreach (Speciality ss in speciality)
            {

                PatientReferralOrder model = new PatientReferralOrder();
                if (referral.CategoryName == mzk_patientordermzk_ReferralCategory.External.ToString())
                {
                    model.SpecialtyName = ss.Description;
                    model.SpecialtyId = ss.SpecialityId;
                    model.referringPhysicianList = new ReferringPhysician().getReferringPhysician(ss.SpecialityId);
                    model.Category = (int)mzk_patientordermzk_ReferralCategory.External;
                }
                else
                {
                    if (referral.CategoryName == mzk_patientordermzk_ReferralCategory.Internal.ToString())
                    {
                        model.SpecialtyName = ss.Description;
                        model.SpecialtyId = ss.SpecialityId;
                        
                        if (!string.IsNullOrEmpty(referral.UserId))
                        {
                            model.usersList = new User().getUsers(ss.SpecialityRefRecId, true, referral.UserId, ss.SpecialityId);
                        }
                        else
                        {
                            model.usersList = new User().getUsers(ss.SpecialityRefRecId, true, string.Empty, ss.SpecialityId);
                        }                        
                        
                        model.Category = (int)mzk_patientordermzk_ReferralCategory.Internal;
                    }
                }
                PatientReferral.Add(model);
            }

            //QueryExpression query;
            //#region Search User Query
            //if (referral.CategoryName == mzk_patientordermzk_ReferralCategory.Internal.ToString())
            //{
            //    query = new QueryExpression("systemuser");
            //    FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);
            //    childFilter.AddCondition("fullname", ConditionOperator.Like, "%" + referral.Referralto + "%");
            //    query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("fullname", "address1_composite", "mzk_type", "mzk_axresourcerefrecid");
            //}          
            //else
            //{
            //    query = new QueryExpression("mzk_referringphysician");
            //    FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);
            //    childFilter.AddCondition("mzk_name", ConditionOperator.Like, "%" + referral.Referralto + "%");
            //    query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_name", "mzk_contactid", "mzk_specialityid", "mzk_type");

            //    LinkEntity linkContact = new LinkEntity("mzk_referringphysician", "contact", "mzk_contactid", "contactid", JoinOperator.LeftOuter);
            //    linkContact.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("address1_composite");

            //    LinkEntity linkSpeciality = new LinkEntity("mzk_referringphysician", "mzk_speciality", "mzk_specialityid", "mzk_specialityid", JoinOperator.LeftOuter);
            //    linkSpeciality.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_description");
            //    query.LinkEntities.Add(linkContact);
            //    query.LinkEntities.Add(linkSpeciality);
            //}
            //#endregion
            //SoapEntityRepository entityRepository = SoapEntityRepository.GetService(); 
            //EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            //User user = new User();
            //foreach (Entity entity in entitycollection.Entities)
            //{
            //    PatientReferral model = new PatientReferral();

            //    if (referral.CategoryName == mzk_patientordermzk_ReferralCategory.Internal.ToString())
            //    {
            //        if (entity.Attributes.Contains("fullname"))
            //            model.Referralto = entity.Attributes["fullname"].ToString();

            //        if (entity.Attributes.Contains("address1_composite"))
            //            model.Address = entity.Attributes["address1_composite"].ToString();

            //        if (entity.Attributes.Contains("mzk_axresourcerefrecid"))
            //        {
            //            if (user.getSpecialities(Convert.ToInt64(entity.Attributes["mzk_axresourcerefrecid"])).Count > 0)
            //            {
            //                model.SpecialtyId = new User().getSpecialities(Convert.ToInt64(entity.Attributes["mzk_axresourcerefrecid"])).First<Speciality>().SpecialityId;
            //                model.SpecialtyName = new User().getSpecialities(Convert.ToInt64(entity.Attributes["mzk_axresourcerefrecid"])).First<Speciality>().Description;
            //            }
            //        }
            //        model.ReferralId = entity.Id.ToString();
            //    }
            //    else
            //    {
            //        if (referral.CategoryName == mzk_patientordermzk_ReferralCategory.External.ToString())
            //        {
            //            if (entity.Attributes.Contains("mzk_name"))
            //                model.Referralto = entity.Attributes["mzk_name"].ToString();

            //            if (entity.Attributes.Contains("contact1.address1_composite"))
            //                model.Address = (entity.Attributes["contact1.address1_composite"] as AliasedValue).Value.ToString();

            //            if (entity.Attributes.Contains("mzk_speciality2.mzk_description"))
            //            {
            //                model.SpecialtyName = (entity.Attributes["mzk_speciality2.mzk_description"] as AliasedValue).Value.ToString();
            //            }
            //            if (entity.Attributes.Contains("mzk_specialityid"))
            //            {
            //                model.SpecialtyId = ((EntityReference)entity.Attributes["mzk_specialityid"]).Id.ToString();
            //            }
            //            model.ReferralId = entity.Id.ToString();
            //        }
            //    }

            //    PatientReferral.Add(model);
            //}
            return PatientReferral;
        }
        public PatientReferralOrder referralExist(string caseId, string referToId)
        {
            QueryExpression query = new QueryExpression(mzk_patientorder.EntityLogicalName);

            query.Criteria.AddCondition("mzk_type", ConditionOperator.Equal, (int)mzk_patientordermzk_Type.Referral);
            query.Criteria.AddCondition("mzk_orderstatus", ConditionOperator.Equal, (int)mzk_orderstatus.Ordered);
            query.Criteria.AddCondition("mzk_referralcategory", ConditionOperator.Equal, (int)mzk_patientordermzk_ReferralCategory.Internal);
            query.Criteria.AddCondition("mzk_referraltoid", ConditionOperator.Equal, new Guid(referToId));
            query.Criteria.AddCondition("mzk_caseid", ConditionOperator.Equal, new Guid(caseId));
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_specialityid", "mzk_axcasetransrefrecid");

            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            if (entitycollection != null && entitycollection.Entities != null && entitycollection.Entities.Count > 0)
            {
                PatientReferralOrder referral = new PatientReferralOrder();

                if (entitycollection.Entities[0].Attributes.Contains("mzk_specialityid"))
                {
                    referral.SpecialtyId = entitycollection.Entities[0].GetAttributeValue<EntityReference>("mzk_specialityid").Id.ToString();
                }

                if (entitycollection.Entities[0].Attributes.Contains("mzk_axcasetransrefrecid"))
                {
                    referral.CaseTransRecId = entitycollection.Entities[0].Attributes["mzk_axcasetransrefrecid"].ToString();
                }

                return referral;
            }
            else
            {
                return null;
            }
        }
    }
}
