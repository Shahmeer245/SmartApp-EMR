using Helper;
using MazikCareService.AXRepository;
using MazikCareService.AXRepository.AXServices;
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
   public class Products
    {
        public string Name { get; set; }
        public string ProductNumber { get; set; }
        public string ProductId { get; set; }
        public bool Available { get; set; }
        public string FrequencyId { get; set; }
        public string FrequencyName { get; set; }
        public string DiagnosisId { get; set; }
        public string DiagnosisName { get; set; }
        public string RouteId { get; set; }
        public string RouteName { get; set; }
       
        public string ItemId { get; set; }


        public string UrgencyId { get; set; }
        public string UrgencyName { get; set; }

        public string clinicId { get; set; }


        public bool availableForPharmacy { get; set; }
        public bool availableForClinic { get; set; }

        public string UnitId { get; set; }
        public string UnitName { get; set; }
        public int Duration { get; set; }
        public string Instruction { get; set; }
        public string FavouriteId { get; set; }
        public string UserId { get; set; }

        public string patientId { get; set; }
        public string Type { get; set; }
        public string Dosage { get; set; }

        public string filter { get; set; }
        public int currentpage { get; set; }

        public bool antiBioticRequired { get; set; }
        public bool commentsRequired { get; set; }
        public bool contrastOrder { get; set; }
        public bool sedationOrder { get; set; }

        public bool controlledDrugs { get; set; }
        
        public bool isAdded { get; set; }
        public string EncounterId { get; set; }
        public bool IsSpecimenSource { get; set; }
        public string description { get; set; }
        public string productImage { get; set; }
        public List<Products> getProduct(Products product, int pageNumber = 0)
        {
            try
            {                
                List<Products> Products = new List<Products>();
                #region Product
                QueryExpression query = new QueryExpression("product");

                query.Criteria.AddCondition("productstructure", ConditionOperator.NotEqual, (int)ProductProductStructure.ProductFamily);
                query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)ProductState.Active);

                ConditionExpression condition1 = new ConditionExpression();
                condition1.AttributeName = "name";
                condition1.Operator = ConditionOperator.Like;


                ConditionExpression condition2 = new ConditionExpression();
                condition2.AttributeName = "mzk_producttype";
                condition2.Operator = ConditionOperator.Equal;
                if (!string.IsNullOrEmpty(product.Type))
                    condition2.Values.Add(product.Type);

                FilterExpression filter1 = new FilterExpression(LogicalOperator.And);
                FilterExpression filter2 = new FilterExpression();

                if (!string.IsNullOrEmpty(product.Name.ToLower()))
                {
                    
                    if (product.Type == "4")
                    {
                        if (product.Name.Contains(' '))
                        {
                            try
                            {
                                string[] words = product.Name.Split(new Char[] { ' ' });

                                if (words.Length > 1 && words.Length < 3)
                                {
                                    //   filter1 = entityTypeDetails.LinkCriteria.AddFilter(LogicalOperator.Or);

                                    condition1.Values.Add("%" + words[0] + "%");
                                    filter1.Conditions.Add(new ConditionExpression("name", ConditionOperator.Like, "%" + words[1] + "%"));
                                    filter1.Conditions.Add(condition1);
                                }
                                else if (words.Length > 2)
                                {
                                    condition1.Values.Add("%" + words[0] + "%");

                                    filter1.Conditions.Add(condition1);
                                    filter1.Conditions.Add(new ConditionExpression("name", ConditionOperator.Like, "%" + words[1] + "%"));
                                    filter1.Conditions.Add(new ConditionExpression("name", ConditionOperator.Like, "%" + words[2] + "%"));
                                }
                            }
                            catch (Exception ex)
                            {
                                condition1.Values.Add("%" + product.Name + "%");
                                filter1.Conditions.Add(condition1);

                            }
                           
                        }
                        else
                        {
                            condition1.Values.Add("%" + product.Name + "%");
                            filter1.Conditions.Add(condition1);
                        }
                    }
                    else if (product.Type == ((int)Productmzk_ProductType.Lab).ToString())
                    {
                        filter1 = new FilterExpression(LogicalOperator.Or);

                        filter1.AddCondition("name", ConditionOperator.Like, ("%" + product.Name.ToLower() + "%"));
                        filter1.AddCondition("productnumber", ConditionOperator.Like, ("%" + product.Name.ToLower() + "%"));                        
                    }
                    else
                    {
                        condition1.Values.Add("%" + product.Name + "%");
                        filter1.Conditions.Add(condition1);
                    }
                }
                filter2.Conditions.Add(condition2);

                query.Criteria.AddFilter(filter1);
                query.Criteria.AddFilter(filter2);
                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("name", "mzk_dosageid", "productnumber", "productid", "mzk_available", "mzk_diagnosisid", "mzk_frequencyid", "mzk_unitid","mzk_routeid","mzk_duration","mzk_instruction", "parentproductid", "mzk_contrast", "mzk_specimensource", "mzk_axitemid");

                LinkEntity EntityDiagnosis = new LinkEntity("product", "mzk_concept", "mzk_diagnosisid", "mzk_conceptid", JoinOperator.LeftOuter);
                EntityDiagnosis.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_conceptname");

                LinkEntity EntityFrequency = new LinkEntity("product", "mzk_ordersetup", "mzk_frequencyid", "mzk_ordersetupid", JoinOperator.LeftOuter);
                EntityFrequency.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_description");

                LinkEntity EntityRoute = new LinkEntity("product", "mzk_ordersetup", "mzk_routeid", "mzk_ordersetupid", JoinOperator.LeftOuter);
                EntityRoute.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_description");

                LinkEntity EntityUnit = new LinkEntity("product", "mzk_unit", "mzk_unitid", "mzk_unitid", JoinOperator.LeftOuter);
                EntityUnit.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_description");
                
                LinkEntity EntityDosage = new LinkEntity("product", "mzk_dosageform", "mzk_dosageid", "mzk_dosageformid", JoinOperator.LeftOuter);
                EntityDosage.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_dosageformid");
                EntityDosage.EntityAlias = "Dosage";

                LinkEntity EntityUserFavourite;
                if (!string.IsNullOrEmpty(product.filter) && product.filter == "favourite")
                    EntityUserFavourite = new LinkEntity("product", "mzk_userfavourite", "productid", "mzk_productid", JoinOperator.Inner);

                else
                    EntityUserFavourite = new LinkEntity("product", "mzk_userfavourite", "productid", "mzk_productid", JoinOperator.LeftOuter);
                EntityUserFavourite.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

                EntityUserFavourite.EntityAlias = "ProductFavourite";



                LinkEntity EntityFamily= new LinkEntity("product", "product", "parentproductid", "productid", JoinOperator.LeftOuter);
            EntityFamily.EntityAlias = "ProductFamily";
            EntityFamily.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_antibioticmandatory", "mzk_commentsmandatory", "mzk_controlleddrug", "mzk_sedation", "mzk_agefromunit", "mzk_agefromvalue", "mzk_agetounit", "mzk_agetovalue");


                query.LinkEntities.Add(EntityDiagnosis);
                query.LinkEntities.Add(EntityFrequency);
                query.LinkEntities.Add(EntityRoute);
                query.LinkEntities.Add(EntityUnit);
               // query.LinkEntities.Add(EntityImportance);
                query.LinkEntities.Add(EntityFamily);
                query.LinkEntities.Add(EntityDosage);
                if (!string.IsNullOrEmpty(product.UserId))
                {
                    EntityUserFavourite.LinkCriteria.AddCondition("mzk_userid", ConditionOperator.Equal, product.UserId);
                    query.LinkEntities.Add(EntityUserFavourite);
                }

                if (pageNumber > 0)
                {
                    query.PageInfo = new PagingInfo();
                    query.PageInfo.Count = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                    query.PageInfo.PageNumber = product.currentpage;
                    query.PageInfo.PagingCookie = null;
                    query.PageInfo.ReturnTotalRecordCount = true;
                }

                #endregion
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

                CaseParameter caseParm = null;
                List<string> prodExistList = new List<string>();

                if (!string.IsNullOrEmpty(product.EncounterId))
                {
                    caseParm = CaseParameter.getDefaultUrgency(PatientCase.getCaseType(product.EncounterId));

                    query = new QueryExpression(mzk_patientorder.EntityLogicalName);

                    query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                    query.Criteria.AddCondition("mzk_patientencounterid", ConditionOperator.Equal, product.EncounterId);                    
                    query.Criteria.AddCondition("mzk_orderstatus", ConditionOperator.Equal, (int)mzk_orderstatus.Ordered);
                    query.Criteria.AddCondition("mzk_productid", ConditionOperator.NotNull);

                    EntityCollection entitycollectionOrders = entityRepository.GetEntityCollection(query);

                    foreach (Entity entity in entitycollectionOrders.Entities)
                    {
                        mzk_patientorder order = (mzk_patientorder)entity;

                        prodExistList.Add(order.mzk_ProductId.Id.ToString());
                    }                        
                }

                foreach (Entity entity in entitycollection.Entities)
                {
                    Products model = new Products();
                    if (entity.Attributes.Contains("name"))
                        model.Name = entity.Attributes["name"].ToString();
                    if (entity.Attributes.Contains("productnumber"))
                        model.ProductNumber = entity.Attributes["productnumber"].ToString();
                    if (entity.Attributes.Contains("productid"))
                        model.ProductId = entity.Id.ToString();

                    if (entity.Attributes.Contains("mzk_available"))
                        model.Available = Convert.ToBoolean(entity.Attributes["mzk_available"].ToString());

                    if (entity.Attributes.Contains("mzk_contrast"))
                        model.contrastOrder = Convert.ToBoolean(entity.Attributes["mzk_contrast"].ToString());

                    //Diagnosis
                    if (entity.Attributes.Contains("mzk_diagnosisid"))
                        model.DiagnosisId = ((EntityReference)entity.Attributes["mzk_diagnosisid"]).Id.ToString();
                    if (entity.Attributes.Contains("mzk_concept1.mzk_conceptname"))
                        model.DiagnosisName = (entity.Attributes["mzk_concept1.mzk_conceptname"] as AliasedValue).Value.ToString();

                    //Frequency
                    if (entity.Attributes.Contains("mzk_frequencyid"))
                        model.FrequencyId = ((EntityReference)entity.Attributes["mzk_frequencyid"]).Id.ToString();
                    if (entity.Attributes.Contains("mzk_ordersetup2.mzk_description"))
                        model.FrequencyName = (entity.Attributes["mzk_ordersetup2.mzk_description"] as AliasedValue).Value.ToString();

                    //Route
                    if (entity.Attributes.Contains("mzk_routeid"))
                        model.RouteId = ((EntityReference)entity.Attributes["mzk_routeid"]).Id.ToString();
                    if (entity.Attributes.Contains("mzk_ordersetup3.mzk_description"))
                        model.RouteName = (entity.Attributes["mzk_ordersetup3.mzk_description"] as AliasedValue).Value.ToString();

                    //Unit
                    if (entity.Attributes.Contains("mzk_unitid"))
                        model.UnitId = ((EntityReference)entity.Attributes["mzk_unitid"]).Id.ToString();
                    if (entity.Attributes.Contains("mzk_unit4.mzk_description"))
                        model.UnitName = (entity.Attributes["mzk_unit4.mzk_description"] as AliasedValue).Value.ToString();

                    if (!string.IsNullOrEmpty(product.EncounterId) && caseParm != null)
                    {
                        model.UrgencyId = caseParm.urgencyId;
                        model.UrgencyName = caseParm.urgencyName;
                    }

                    //Instruction
                    if (entity.Attributes.Contains("mzk_instruction"))
                        model.Instruction = entity.Attributes["mzk_instruction"].ToString();

                    if (entity.Attributes.Contains("mzk_specimensource"))
                        model.IsSpecimenSource = (bool)entity.Attributes["mzk_specimensource"];

                    if (model.IsSpecimenSource == false)
                    {
                        if (!string.IsNullOrEmpty(product.EncounterId))
                        {
                            model.isAdded = !prodExistList.Exists(item => item == entity.Id.ToString());
                            //model.isAdded = new PatientEncounter().DuplicateDetection(product.EncounterId, entity.Id.ToString());
                        }
                    }
                    else
                    {
                        model.isAdded = true;
                    }

                    if (entity.Attributes.Contains("ProductFavourite.mzk_userfavouriteid"))
                        model.FavouriteId = (entity.Attributes["ProductFavourite.mzk_userfavouriteid"] as AliasedValue).Value.ToString();

                    //Parameters
                    if (entity.Attributes.Contains("ProductFamily.mzk_antibioticmandatory"))
                        model.antiBioticRequired = (bool)((AliasedValue)entity.Attributes["ProductFamily.mzk_antibioticmandatory"]).Value;

                    if (entity.Attributes.Contains("ProductFamily.mzk_controlleddrug"))
                        model.controlledDrugs = (bool)((AliasedValue)entity.Attributes["ProductFamily.mzk_controlleddrug"]).Value;

                    if (entity.Attributes.Contains("ProductFamily.mzk_commentsmandatory"))
                        model.commentsRequired = (bool)((AliasedValue)entity.Attributes["ProductFamily.mzk_commentsmandatory"]).Value;

                    if (entity.Attributes.Contains("ProductFamily.mzk_sedation") && !string.IsNullOrEmpty(product.patientId))
                    {
                        model.sedationOrder = (bool)((AliasedValue)entity.Attributes["ProductFamily.mzk_sedation"]).Value;

                        if (model.sedationOrder)
                        {
                            AgeHelper ageHelper = new AgeHelper(DateTime.Now);
                            DateTime patientBirthDate;
                            Patient patient = new Patient();
                            Helper.Enum.DayWeekMthYr ageFromUnit = Helper.Enum.DayWeekMthYr.Days, ageToUnit = Helper.Enum.DayWeekMthYr.Days;
                            int ageFromValue = 0, ageToValue = 0;                            

                            patientBirthDate = patient.getPatientDetails(product.patientId).Result.dateOfBirth;

                            if (entity.Attributes.Contains("ProductFamily.mzk_agefromunit") && (entity.Attributes["ProductFamily.mzk_agefromunit"] as AliasedValue) != null)
                            {
                                ageFromUnit = (Helper.Enum.DayWeekMthYr)((entity.Attributes["ProductFamily.mzk_agefromunit"] as AliasedValue).Value as OptionSetValue).Value;
                            }

                            if (entity.Attributes.Contains("ProductFamily.mzk_agetounit") && (entity.Attributes["ProductFamily.mzk_agetounit"] as AliasedValue) != null)
                            {
                                ageToUnit = (Helper.Enum.DayWeekMthYr)((entity.Attributes["ProductFamily.mzk_agetounit"] as AliasedValue).Value as OptionSetValue).Value;
                            }

                            if (entity.Attributes.Contains("ProductFamily.mzk_agefromvalue") && (entity.Attributes["ProductFamily.mzk_agefromvalue"] as AliasedValue) != null)
                            {
                                ageFromValue = (int)((entity.Attributes["ProductFamily.mzk_agefromvalue"] as AliasedValue).Value);
                            }

                            if (entity.Attributes.Contains("ProductFamily.mzk_agetovalue") && (entity.Attributes["ProductFamily.mzk_agetovalue"] as AliasedValue) != null)
                            {
                                ageToValue = (int)((entity.Attributes["ProductFamily.mzk_agetovalue"] as AliasedValue).Value);
                            }

                            model.sedationOrder = ageHelper.isAgeMatched(patientBirthDate, ageFromUnit, ageFromValue, ageToUnit, ageToValue);
                        }
                    }

                    if (entity.Attributes.Contains("mzk_dosageid"))
                        model.Dosage = ((EntityReference)entity.Attributes["mzk_dosageid"]).Id.ToString();

                    if (AppSettings.GetByKey("OperationsIntegration").ToLower() == true.ToString().ToLower())
                    {
                        if (!string.IsNullOrEmpty(product.clinicId) && entity.Attributes.Contains("mzk_axitemid") && !string.IsNullOrEmpty(entity.Attributes["mzk_axitemid"].ToString()))
                        {
                            CommonRepository comRepo = new CommonRepository();

                            Clinic clinic = new Clinic().getClinicDetails(product.clinicId);

                            Dictionary<int, int> retStock = comRepo.checkItemInStock(entity.Attributes["mzk_axitemid"].ToString(), clinic.mzk_axclinicrefrecid);

                            if (retStock != null && retStock.Count > 0)
                            {
                                int availableValue = 0;

                                if (retStock.TryGetValue(0, out availableValue))
                                {
                                    model.availableForClinic = availableValue == 0 ? false : true;
                                }

                                if (retStock.TryGetValue(1, out availableValue))
                                {
                                    model.availableForPharmacy = availableValue == 0 ? false : true;
                                }
                            }
                        }
                    }

                    Products.Add(model);
                }

                if (pageNumber > 0 && entitycollection != null)
                {
                    Pagination.totalCount = entitycollection.TotalRecordCount;
                }

                return Products;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string getItemId(string productId)
        {
            string itemId = string.Empty;
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            xrm.Product products = (xrm.Product) entityRepository.GetEntity(xrm.Product.EntityLogicalName, new Guid(productId), new ColumnSet("mzk_axitemid"));

            if(products!= null)
            {
                itemId = products.mzk_AXItemId;
            }

            return itemId;
        }
        public static Guid getProductId(string itemId)
        {
            Guid productId = Guid.Empty;

            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            QueryExpression query = new QueryExpression("product");
            
            query.Criteria.AddCondition("mzk_axitemid", ConditionOperator.Equal, itemId);
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("name", "productnumber", "productid");
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            if (entitycollection != null && entitycollection.Entities != null && entitycollection.Entities.Count > 0)
            {
                Product products = (Product)entitycollection.Entities[0];

                productId = products.ProductId.Value;
            }

            return productId;
        }

        public static Dictionary<string, string> getProductId(List<string> itemId)
        {
            Dictionary<string, string> productList = new Dictionary<string, string>();

            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            QueryExpression query = new QueryExpression("product");
            
            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.Or);

            foreach (string item in itemId)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    childFilter.AddCondition("mzk_axitemid", ConditionOperator.Equal, item);
                }
            }

            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("name", "productnumber", "productid", "mzk_axitemid");
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            foreach (Entity entity in entitycollection.Entities)
            {
                Product products = (Product)entity;

                productList.Add(products.mzk_AXItemId, products.ProductId.Value.ToString());
            }             

            return productList;
        }

        public static Dictionary<string, Products> getProduct(List<string> itemId)
        {
            Dictionary<string, Products> productList = new Dictionary<string, Products>();

            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            QueryExpression query = new QueryExpression("product");

            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.Or);

            foreach (string item in itemId)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    childFilter.AddCondition("mzk_axitemid", ConditionOperator.Equal, item);
                }
            }

            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("name", "productnumber", "productid", "mzk_axitemid", "mzk_producttype");
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            foreach (Entity entity in entitycollection.Entities)
            {
                Product products = (Product)entity;
                Products prod = new Products();

                prod.ProductId = products.ProductId.Value.ToString();

                if (products.mzk_ProductType != null)
                {
                    prod.Type = products.mzk_ProductType.Value.ToString();
                }

                productList.Add(products.mzk_AXItemId, prod);
            }

            return productList;
        }

        public Products getProductDetails(string productId)
        {
            string itemId = string.Empty;
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            xrm.Product products = (xrm.Product)entityRepository.GetEntity(xrm.Product.EntityLogicalName, new Guid(productId), new ColumnSet(true));
            Products model = new Products();

            if (products != null)
            {                
                if (products.Attributes.Contains("name"))
                    model.Name = products.Name;

                if (products.Attributes.Contains("productnumber"))
                    model.ProductNumber = products.ProductNumber;

                if (products.ProductId.HasValue)
                    model.ProductId = products.ProductId.Value.ToString();

                if (products.mzk_ProductType != null)
                    model.Type = products.mzk_ProductType.Value.ToString();

                if (products.mzk_Contrast.HasValue)
                    model.contrastOrder = products.mzk_Contrast.Value;

                if (products.Attributes.Contains("mzk_specimensource"))
                    model.IsSpecimenSource = (bool)products.Attributes["mzk_specimensource"];
            }

            return model;
        }

        public List<Products> getProduct(string Type)
        {
            List<Products> Products = new List<Products>();
            #region Product
            QueryExpression query = new QueryExpression("product");
            
            ConditionExpression conditionType = new ConditionExpression();
            conditionType.AttributeName = "mzk_producttype";
            conditionType.Operator = ConditionOperator.Equal;
            conditionType.Values.Add(Type);

            FilterExpression filterType = new FilterExpression();
            filterType.Conditions.Add(conditionType);
            
            query.Criteria.AddFilter(filterType);
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("name", "productnumber", "productid");
            #endregion
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            foreach (Entity entity in entitycollection.Entities)
            {
                Products model = new Products();
                if (entity.Attributes.Contains("name"))
                    model.Name = entity.Attributes["name"].ToString();
                if (entity.Attributes.Contains("productnumber"))
                    model.ProductNumber = entity.Attributes["productnumber"].ToString();
                if (entity.Attributes.Contains("productid"))
                    model.ProductId = entity.Id.ToString();
                Products.Add(model);
            }
            return Products;
        }

        public async Task<string> AddUserFavourite(Products product)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            xrm.mzk_userfavourite userFavouriteEntity = new xrm.mzk_userfavourite();
          
            QueryExpression query = new QueryExpression(mzk_userfavourite.EntityLogicalName);

            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
           

            if (!string.IsNullOrEmpty(product.UserId))
            {
                query.Criteria.AddCondition("mzk_userid", ConditionOperator.Equal, product.UserId);
                query.Criteria.AddCondition("mzk_productid", ConditionOperator.Equal, product.ProductId);
            }

            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            if (entitycollection.Entities.Count > 1)
            {
                 throw new ValidationException("Favourite Already exist");
                //FavouriteId = entitycollection.Entities[0].Attributes["mzk_userfavouriteid"].ToString();
                // return FavouriteId.ToString();
            }

            if (string.IsNullOrEmpty(FavouriteId))
            {

                if (!string.IsNullOrEmpty(product.ProductId))
                    userFavouriteEntity.mzk_ProductId = new Microsoft.Xrm.Sdk.EntityReference("product", new Guid(product.ProductId));
                if (!string.IsNullOrEmpty(product.UserId))
                    userFavouriteEntity.mzk_UserId = new Microsoft.Xrm.Sdk.EntityReference("systemuser", new Guid(product.UserId));
                if (!string.IsNullOrEmpty(product.Type))
                    userFavouriteEntity.mzk_ProductType = new Microsoft.Xrm.Sdk.OptionSetValue(Convert.ToInt32(product.Type));

                FavouriteId = Convert.ToString(entityRepository.CreateEntity(userFavouriteEntity));
            }

               return FavouriteId.ToString();
        }

        public async Task<bool> removeUserFavourite(Products product)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            xrm.mzk_userfavourite userFavouriteEntity = new xrm.mzk_userfavourite();

            try
            {
                QueryExpression query = new QueryExpression(mzk_userfavourite.EntityLogicalName);
                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

                if (!string.IsNullOrEmpty(product.FavouriteId))
                {
                    query.Criteria.AddCondition("mzk_userfavouriteid", ConditionOperator.Equal, product.FavouriteId);
                }

                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

                if (entitycollection.Entities.Count < 1)
                {
                    return false;
                }

                    if (!string.IsNullOrEmpty(product.FavouriteId) && (!product.FavouriteId.Equals("null")))
                    entityRepository.DeleteEntity(xrm.mzk_userfavourite.EntityLogicalName, new Guid(product.FavouriteId));
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Products>> getContractProducts(string workOrderId)
        {
            try
            {
                if (!string.IsNullOrEmpty(workOrderId))
                {
                    List<Products> products = new List<Products>();
                    QueryExpression query = new QueryExpression(xrm.msdyn_workorder.EntityLogicalName);
                    query.Criteria.AddCondition("msdyn_workorderid", ConditionOperator.Equal, new Guid(workOrderId));
                    
                    LinkEntity WorkOrderCase = new LinkEntity(xrm.msdyn_workorder.EntityLogicalName, xrm.Incident.EntityLogicalName, "msdyn_servicerequest", "incidentid", JoinOperator.Inner) { };
                    LinkEntity CaseContract = new LinkEntity(xrm.Incident.EntityLogicalName, "mzk_contractmanagement", "mzk_contract", "mzk_contractmanagementid", JoinOperator.Inner) { };
                    LinkEntity ContractContractLine = new LinkEntity("mzk_contractmanagement", "mzk_contractline", "mzk_contractmanagementid", "mzk_contractmanagement", JoinOperator.Inner) { };
                    LinkEntity ContractLineProduct = new LinkEntity("mzk_contractline", xrm.Product.EntityLogicalName, "mzk_product", "productid", JoinOperator.Inner)
                    {
                        LinkCriteria =
                        {

                            Conditions =
                            {
                                new ConditionExpression("mzk_producttype",ConditionOperator.Equal,8)// 8 = Retail/Ancillery Items
                            }
                        },
                        Columns = new ColumnSet("productid","name", "productnumber", "description", "defaultuomid", "mzk_unitid", "mzk_producttype", "entityimage"),
                        EntityAlias="Product",
                        
                    };
                    ContractContractLine.LinkEntities.Add(ContractLineProduct);
                    CaseContract.LinkEntities.Add(ContractContractLine);
                    WorkOrderCase.LinkEntities.Add(CaseContract);
                    query.LinkEntities.Add(WorkOrderCase);
                    

                    SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                    EntityCollection entitycollection = getReferralProducts(workOrderId);///entityRepository.GetEntityCollection(query);

                    if (entitycollection.Entities.Count == 0)
                    {
                        entitycollection = entityRepository.GetEntityCollection(query);
                    }

                    foreach (Entity entity in entitycollection.Entities)
                    {
                        Products product = new Products();
                        if (entity.Attributes.Contains("Product.productid"))
                        {
                            product.ProductId = entity.GetAttributeValue<AliasedValue>("Product.productid").Value.ToString();
                        }
                        if (entity.Attributes.Contains("Product.name"))
                        {
                            product.Name = entity.GetAttributeValue<AliasedValue>("Product.name").Value.ToString();
                        }
                        if (entity.Attributes.Contains("Product.productnumber"))
                        {
                            product.ProductNumber = entity.GetAttributeValue<AliasedValue>("Product.productnumber").Value.ToString();
                        }
                        if (entity.Attributes.Contains("Product.description"))
                        {
                            product.description = entity.GetAttributeValue<AliasedValue>("Product.description").Value.ToString();
                        }
                        if (entity.Attributes.Contains("Product.defaultuomid"))
                        {
                            product.UnitId = (entity.GetAttributeValue<AliasedValue>("Product.defaultuomid").Value as EntityReference).Id.ToString();
                            product.UnitName = (entity.GetAttributeValue<AliasedValue>("Product.defaultuomid").Value as EntityReference).Name.ToString();
                        }
                        if (entity.Attributes.Contains("Product.mzk_producttype"))
                        {
                            product.Type = entity.FormattedValues["Product.mzk_producttype"].ToString();
                        }
                        if (entity.Attributes.Contains("Product.entityimage"))
                        {
                            byte[] imageInBytes = (entity.Attributes["Product.entityimage"] as AliasedValue).Value as byte[];
                            product.productImage = Convert.ToBase64String(imageInBytes);
                        }
                        products.Add(product);
                    }

                    return products;
                }
                else
                {
                    throw new ValidationException("WorkOrder Id not found");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EntityCollection getReferralProducts(string workOrderId)
        {
            try
            {
                QueryExpression query = new QueryExpression(xrm.msdyn_workorder.EntityLogicalName);
                query.Criteria.AddCondition("msdyn_workorderid", ConditionOperator.Equal, new Guid(workOrderId));

                LinkEntity WorkOrderCase = new LinkEntity(xrm.msdyn_workorder.EntityLogicalName, xrm.Incident.EntityLogicalName, "msdyn_servicerequest", "incidentid", JoinOperator.Inner) { };
                LinkEntity CaseReferral = new LinkEntity(xrm.Incident.EntityLogicalName, xrm.Opportunity.EntityLogicalName, "mzk_referral", "opportunityid", JoinOperator.Inner) { };
                LinkEntity ReferralPatientSpecificItem = new LinkEntity(xrm.Opportunity.EntityLogicalName, "mzk_patientspecificitem", "opportunityid", "mzk_referral", JoinOperator.Inner) { };
                LinkEntity PatientSpecificItemProduct = new LinkEntity("mzk_patientspecificitem", "product", "mzk_product", "productid", JoinOperator.Inner)
                {
                    LinkCriteria =
                        {
                            Conditions =
                            {
                                new ConditionExpression("mzk_producttype",ConditionOperator.Equal,8)// 8 = Retail/Ancillery Items
                            }
                        },
                    Columns = new ColumnSet("productid", "name", "productnumber", "description", "defaultuomid", "mzk_unitid", "mzk_producttype", "entityimage"),
                    EntityAlias = "Product",
                };
                ReferralPatientSpecificItem.LinkEntities.Add(PatientSpecificItemProduct);
                CaseReferral.LinkEntities.Add(ReferralPatientSpecificItem);
                WorkOrderCase.LinkEntities.Add(CaseReferral);
                query.LinkEntities.Add(WorkOrderCase);

                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
                return entitycollection;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}

