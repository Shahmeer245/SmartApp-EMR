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
using System.Globalization;


namespace MazikCareService.Core.Models
{
    public class PatientVisitProducts
    {
        public string product { get; set; }
        public int visitNumber { get; set; }
        public string contract { get; set; }
        public DateTime visitDate { get; set; }
        public string workOrderProductId { get; set; }
        public string workOrderId { get; set; }
        public int stockLevel { get; set; }
        public string unit { get; set; }
        public double quantity { get; set; }

        public string priceList { get; set; }
        public int lineStatus { get; set; }

        public async Task<List<PatientVisitProducts>> getActiveVisitProducts(string patientId)
        {
            if (!string.IsNullOrEmpty(patientId))
            {
                List<PatientVisitProducts> visits = new List<PatientVisitProducts>();
                QueryExpression query1 = new QueryExpression(xrm.Incident.EntityLogicalName);
                query1.Criteria.AddCondition("customerid", ConditionOperator.Equal, new Guid(patientId));
                query1.ColumnSet = new ColumnSet("mzk_contract");

                LinkEntity workOrder = new LinkEntity(xrm.Incident.EntityLogicalName, xrm.msdyn_workorder.EntityLogicalName, "incidentid", "msdyn_servicerequest", JoinOperator.Inner)
                {
                    Columns = new ColumnSet("mzk_scheduledstartdatetime", "mzk_visitnumber", "msdyn_workorderid"),
                    LinkCriteria =
                {
                    Conditions =
                    {
                        new ConditionExpression("mzk_visitstatus",ConditionOperator.Equal, 275380001),//Confirmed (Previously it was Scheduled 13)
                        new ConditionExpression("mzk_visittype",ConditionOperator.Equal, 275380001)//Delivery Visit
                    },FilterOperator = LogicalOperator.And
                },
                    EntityAlias = "workOrder"
                };
                query1.LinkEntities.Add(workOrder);

                SoapEntityRepository repo = SoapEntityRepository.GetService();
                EntityCollection entityCollection = repo.GetEntityCollection(query1);
                var groupedWorkOrders = entityCollection.Entities.GroupBy(item => (item.GetAttributeValue<Guid>("incidentid")));
                foreach (var groupedVisits in groupedWorkOrders)
                {

                    foreach (Entity entity in groupedVisits)
                    {
                        if (entity.Attributes.Contains("workOrder.msdyn_workorderid"))
                        {
                            QueryExpression query2 = new QueryExpression(xrm.msdyn_workorderproduct.EntityLogicalName);
                            query2.Criteria.AddCondition("msdyn_workorder", ConditionOperator.Equal, new Guid(entity.GetAttributeValue<AliasedValue>("workOrder.msdyn_workorderid").Value.ToString()));
                            query2.ColumnSet = new ColumnSet("msdyn_product", "msdyn_workorderproductid", "mzk_currentstocklevel");

                            EntityCollection productCollection = repo.GetEntityCollection(query2);
                            if (productCollection.Entities.Count > 0)
                            {
                                foreach (Entity product in productCollection.Entities)
                                {
                                    PatientVisitProducts visit = new PatientVisitProducts();
                                    visit.workOrderId = entity.GetAttributeValue<AliasedValue>("workOrder.msdyn_workorderid").Value.ToString();
                                    if (product.Attributes.Contains("mzk_currentstocklevel"))
                                    {
                                        visit.stockLevel = Int32.Parse(product["mzk_currentstocklevel"].ToString());
                                    }

                                    if (product.Attributes.Contains("msdyn_workorderproductid"))
                                    {
                                        visit.workOrderProductId = product["msdyn_workorderproductid"].ToString();
                                    }
                                    if (product.Attributes.Contains("msdyn_product"))
                                    {
                                        visit.product = (product["msdyn_product"] as EntityReference).Name;
                                        //visit.workOrderProductId = (product["msdyn_product"] as EntityReference).Id.ToString();
                                    }

                                    if (entity.Attributes.Contains("workOrder.mzk_scheduledstartdatetime"))
                                    {
                                        visit.visitDate = Convert.ToDateTime(entity.GetAttributeValue<AliasedValue>("workOrder.mzk_scheduledstartdatetime").Value);
                                    }
                                    if (entity.Attributes.Contains("workOrder.mzk_visitnumber"))
                                    {
                                        visit.visitNumber = Int32.Parse(entity.GetAttributeValue<AliasedValue>("workOrder.mzk_visitnumber").Value.ToString());
                                    }
                                    if (entity.Attributes.Contains("mzk_contract"))
                                    {
                                        visit.contract = (entity["mzk_contract"] as EntityReference).Name;
                                    }
                                    visits.Add(visit);
                                }
                            }
                            else
                            {
                                PatientVisitProducts visit = new PatientVisitProducts();
                                if (entity.Attributes.Contains("workOrder.mzk_scheduledstartdatetime"))
                                {
                                    visit.visitDate = Convert.ToDateTime(entity.GetAttributeValue<AliasedValue>("workOrder.mzk_scheduledstartdatetime").Value);
                                }
                                if (entity.Attributes.Contains("workOrder.mzk_visitnumber"))
                                {
                                    visit.visitNumber = Int32.Parse(entity.GetAttributeValue<AliasedValue>("workOrder.mzk_visitnumber").Value.ToString());
                                }
                                if (entity.Attributes.Contains("mzk_contract"))
                                {
                                    visit.contract = (entity["mzk_contract"] as EntityReference).Name;
                                }
                                visits.Add(visit);
                            }
                            break;
                        }
                    }
                    //break;
                }



                return visits;
            }
            else
            {
                throw new ValidationException("Patient Id missing");
            }
        }

        public async Task<bool> updateWorkOrderProduct(PatientVisitProducts products)
        {
            if (!string.IsNullOrEmpty(products.workOrderProductId))
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();
                Entity workOrderProduct = new Entity(xrm.msdyn_workorderproduct.EntityLogicalName, new Guid(products.workOrderProductId));
                if (products.stockLevel != -1)
                {
                    workOrderProduct["mzk_currentstocklevel"] = products.stockLevel;
                }
                try
                {
                    repo.UpdateEntity(workOrderProduct);
                    return true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            else
            {
                throw new ValidationException("Work Order Product Id not found");
            }

        }

        public async Task<bool> addWorkOrderProducts(string workOrderId, List<PatientVisitProducts> products)
        {
            try
            {
                int orderCount = 0;
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                Configuration configuration = new Configuration();
                configuration = configuration.getConfiguration();
                if (!string.IsNullOrEmpty(workOrderId))
                {
                    Entity workOrder = entityRepository.GetEntity(xrm.msdyn_workorder.EntityLogicalName, new Guid(workOrderId), new ColumnSet("mzk_scheduledstartdatetime", "mzk_ordercount"));
                    if (configuration.orderingPriorHours != 0)
                    {
                        if (workOrder.Attributes.Contains("mzk_scheduledstartdatetime"))
                        {
                            DateTime setDate = workOrder.GetAttributeValue<DateTime>("mzk_scheduledstartdatetime");
                            TimeSpan difference = setDate - DateTime.UtcNow;
                            int hours = difference.Hours;

                            //if (hours <= 0)
                            //{
                            //    throw new ValidationException("Order is already delivered");
                            //}
                            if (hours < configuration.orderingPriorHours)
                            {
                                QueryExpression query = new QueryExpression(BusinessUnit.EntityLogicalName);
                                query.Criteria.AddCondition("parentbusinessunitid", ConditionOperator.Null);
                                query.ColumnSet = new ColumnSet("mzk_contactcentrenumber");
                                query.TopCount = 1;
                                EntityCollection entityCollection = entityRepository.GetEntityCollection(query);
                                if (entityCollection.Entities.Count > 0)
                                {
                                    if (entityCollection.Entities[0].Attributes.Contains("mzk_contactcentrenumber"))
                                    {
                                        throw new ValidationException("Sorry you may not place an order less than " + configuration.orderingPriorHours + " hours before your scheduled delivery date. Should you need assistance please call " + entityCollection.Entities[0]["mzk_contactcentrenumber"].ToString());
                                    }
                                    else
                                    {
                                        throw new ValidationException("Sorry you may not place an order less than " + configuration.orderingPriorHours + " hours before your scheduled delivery date.");
                                    }
                                }
                                else
                                {
                                    throw new ValidationException("Sorry you may not place an order less than " + configuration.orderingPriorHours + " hours before your scheduled delivery date.");
                                }
                            }
                        }
                    }
                    if (configuration.ordersAllowedPerVisit != 0)
                    {
                        if (workOrder.Attributes.Contains("mzk_ordercount"))
                        {
                            orderCount = workOrder.GetAttributeValue<int>("mzk_ordercount");
                            if (orderCount != 0)
                            {
                                if (orderCount >= configuration.ordersAllowedPerVisit)
                                {
                                    QueryExpression query = new QueryExpression(BusinessUnit.EntityLogicalName);
                                    query.Criteria.AddCondition("parentbusinessunitid", ConditionOperator.Null);
                                    query.ColumnSet = new ColumnSet("mzk_contactcentrenumber");
                                    query.TopCount = 1;
                                    EntityCollection entityCollection = entityRepository.GetEntityCollection(query);
                                    if (entityCollection.Entities.Count > 0)
                                    {
                                        if (entityCollection.Entities[0].Attributes.Contains("mzk_contactcentrenumber"))
                                        {
                                            throw new ValidationException("Sorry, you may not order more than " + configuration.ordersAllowedPerVisit + " times per visit. Should you need assistance please call " + entityCollection.Entities[0]["mzk_contactcentrenumber"].ToString());
                                        }
                                        else
                                        {
                                            throw new ValidationException("Sorry, you may not order more than " + configuration.ordersAllowedPerVisit + " times per visit.");
                                        }
                                    }
                                    else
                                    {
                                        throw new ValidationException("Sorry, you may not order more than " + configuration.ordersAllowedPerVisit + " times per visit.");
                                    }
                                }
                                else
                                {
                                    orderCount++;
                                    workOrder["mzk_ordercount"] = orderCount;
                                    try
                                    {
                                        entityRepository.UpdateEntity(workOrder);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }
                                }
                            }
                            else
                            {
                                orderCount++;
                                workOrder["mzk_ordercount"] = orderCount;
                                try
                                {
                                    entityRepository.UpdateEntity(workOrder);
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                            }

                        }
                        else
                        {
                            orderCount++;
                            workOrder["mzk_ordercount"] = orderCount;
                            try
                            {
                                entityRepository.UpdateEntity(workOrder);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                    }
                    if (products.Count > 0)
                    {
                        foreach (PatientVisitProducts product in products)
                        {
                            Entity workOrderProduct = new Entity(xrm.msdyn_workorderproduct.EntityLogicalName);
                            workOrderProduct["msdyn_workorder"] = new EntityReference(xrm.msdyn_workorder.EntityLogicalName, new Guid(workOrderId));
                            if (!string.IsNullOrEmpty(product.product))
                            {
                                workOrderProduct["msdyn_product"] = new EntityReference("product", new Guid(product.product));
                            }
                            if (!string.IsNullOrEmpty(product.unit))
                            {
                                workOrderProduct["msdyn_unit"] = new EntityReference("uom", new Guid(product.unit));
                            }
                            if (product.quantity != 0)
                            {
                                workOrderProduct["msdyn_quantity"] = product.quantity;
                            }
                            if (!string.IsNullOrEmpty(product.priceList))
                            {
                                workOrderProduct["msdyn_pricelist"] = new EntityReference("pricelevel", new Guid(product.priceList));
                            }
                            if (product.lineStatus != 0 )
                            {
                                workOrderProduct["msdyn_linestatus"] = new OptionSetValue(product.lineStatus);
                            }
                            if(orderCount != 0)
                            {
                                workOrderProduct["mzk_ordernumber"] = orderCount;
                            }
                                entityRepository.CreateEntity(workOrderProduct);
                        }
                        return true;
                    }
                    else
                    {
                        throw new ValidationException("Product List is Empty");
                    }
                }
                else
                {
                    throw new ValidationException("Work Order Id not found");
                }
                //return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<bool> addWorkOrderProductValidation(string workOrderId)
        {
            try
            {
                int orderCount = 0;
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                Configuration configuration = new Configuration();
                configuration = configuration.getConfiguration();
                if (!string.IsNullOrEmpty(workOrderId))
                {
                    Entity workOrder = entityRepository.GetEntity(xrm.msdyn_workorder.EntityLogicalName, new Guid(workOrderId), new ColumnSet("mzk_scheduledstartdatetime", "mzk_ordercount"));
                    if (configuration.orderingPriorHours != 0)
                    {
                        if (workOrder.Attributes.Contains("mzk_scheduledstartdatetime"))
                        {
                            DateTime setDate = workOrder.GetAttributeValue<DateTime>("mzk_scheduledstartdatetime");
                            TimeSpan difference = setDate - DateTime.UtcNow;
                            int hours = difference.Hours;
                            if (hours < configuration.orderingPriorHours)
                            {
                                QueryExpression query = new QueryExpression(BusinessUnit.EntityLogicalName);
                                query.Criteria.AddCondition("parentbusinessunitid", ConditionOperator.Null);
                                query.ColumnSet = new ColumnSet("mzk_contactcentrenumber");
                                query.TopCount = 1;
                                EntityCollection entityCollection = entityRepository.GetEntityCollection(query);
                                if (entityCollection.Entities.Count > 0)
                                {
                                    if (entityCollection.Entities[0].Attributes.Contains("mzk_contactcentrenumber"))
                                    {
                                        throw new ValidationException("Sorry you may not place an order less than " + configuration.orderingPriorHours + " hours before your scheduled delivery date. Should you need assistance please call " + entityCollection.Entities[0]["mzk_contactcentrenumber"].ToString());
                                    }
                                    else
                                    {
                                        throw new ValidationException("Sorry you may not place an order less than " + configuration.orderingPriorHours + " hours before your scheduled delivery date.");
                                    }
                                }
                                else
                                {
                                    throw new ValidationException("Sorry you may not place an order less than " + configuration.orderingPriorHours + " hours before your scheduled delivery date.");
                                }
                            }
                        }
                    }
                    if (configuration.ordersAllowedPerVisit != 0)
                    {
                        if (workOrder.Attributes.Contains("mzk_ordercount"))
                        {
                            orderCount = workOrder.GetAttributeValue<int>("mzk_ordercount");
                            if (orderCount != 0)
                            {
                                if (orderCount >= configuration.ordersAllowedPerVisit)
                                {
                                    QueryExpression query = new QueryExpression(BusinessUnit.EntityLogicalName);
                                    query.Criteria.AddCondition("parentbusinessunitid", ConditionOperator.Null);
                                    query.ColumnSet = new ColumnSet("mzk_contactcentrenumber");
                                    query.TopCount = 1;
                                    EntityCollection entityCollection = entityRepository.GetEntityCollection(query);
                                    if (entityCollection.Entities.Count > 0)
                                    {
                                        if (entityCollection.Entities[0].Attributes.Contains("mzk_contactcentrenumber"))
                                        {
                                            throw new ValidationException("Sorry, you may not order more than " + configuration.ordersAllowedPerVisit + " times per visit. Should you need assistance please call " + entityCollection.Entities[0]["mzk_contactcentrenumber"].ToString());
                                        }
                                        else
                                        {
                                            throw new ValidationException("Sorry, you may not order more than " + configuration.ordersAllowedPerVisit + " times per visit.");
                                        }
                                    }
                                    else
                                    {
                                        throw new ValidationException("Sorry, you may not order more than " + configuration.ordersAllowedPerVisit + " times per visit.");
                                    }
                                }
                                
                            }
                            
                        }
                    }
                    return true;
                }
                else
                {
                    throw new ValidationException("Work Order Id not found");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
