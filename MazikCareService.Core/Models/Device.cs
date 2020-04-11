using Helper;
using MazikCareService.AXRepository;
using MazikCareService.AXRepository.AXServices;
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
    public class Device
    {
        public string deviceId { get; set; }

        public string deviceName { get; set; }
        

        public async Task<List<Device>> getDeviceTypes(string practiceId)
        {
            List<Device> Devices = new List<Device>();
            QueryExpression query = new QueryExpression(msdyn_productinventory.EntityLogicalName);
            query.Criteria.AddCondition("mzk_clinic", ConditionOperator.Equal, practiceId);
            query.ColumnSet = new ColumnSet("msdyn_product");
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
            foreach (Entity entity in entitycollection.Entities)
            {
                EntityReference lookup = (EntityReference)entity.Attributes["msdyn_product"];
                String name = lookup.Name;
                Guid guid = lookup.Id;

                Device device = new Device();
                if(entity.Attributes.Contains("msdyn_product"))
                {
                    device.deviceId = guid.ToString();
                    device.deviceName = name;
                }
                Devices.Add(device);
            }
               
            return Devices;
        }




        public async Task<List<DeviceServiceType>> getDeviceServiceTypes()
        {
            List<DeviceServiceType> serviceTypes = new List<DeviceServiceType>();
            QueryExpression query = new QueryExpression("mzk_servicetype");
            query.ColumnSet = new ColumnSet("mzk_servicetypeid", "mzk_name");
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
            foreach (Entity entity in entitycollection.Entities)
            {
                DeviceServiceType serviceType = new DeviceServiceType();
                if (entity.Attributes.Contains("mzk_servicetypeid"))
                {
                    serviceType.serviceTypeId = entity["mzk_servicetypeid"].ToString();
                }
                if (entity.Attributes.Contains("mzk_name"))
                {
                    serviceType.serviceTypeName = entity["mzk_name"].ToString();
                }
                serviceTypes.Add(serviceType);
            }
                
            return serviceTypes;
        }

        public async Task<List<DeviceSerialNumbers>> getDeviceSerialNumbers(string id)
        {
            //int x = await getDeviceSerialNumbers(id);
            List<DeviceSerialNumbers> MyDevice = new List<DeviceSerialNumbers>();
            DeviceSerialNumbers device = new DeviceSerialNumbers();
            MyDevice.Add(device);
            return MyDevice;
        }

        public async Task<List<DeviceDeploymentDurations>> getDeviceDeployDurations(string id)
        {
            List<DeviceDeploymentDurations> MyDevice = new List<DeviceDeploymentDurations>();
            DeviceDeploymentDurations device = new DeviceDeploymentDurations();
            MyDevice.Add(device);
            return MyDevice;
        }

        public async Task<List<AlternateServices>> getAlternateServices(string id)
        {
            List<AlternateServices> MyDevice = new List<AlternateServices>();
            AlternateServices service = new AlternateServices();
            MyDevice.Add(service);
            return MyDevice;
        }
    }
    public class DeviceServiceType
    {
        public string serviceTypeName { get; set; }
        public string serviceTypeId { get; set; }
    }

    public class DeviceSerialNumbers
    {
        public string serialnumber { get; set; }
    }

    public class DeviceDeploymentDurations
    {
        public string duration { get; set; }
    }

    public class AlternateServices
    {
        public string serviceTypeName { get; set; }
        public string serviceTypeId { get; set; }
    }
}
