using MazikCareService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Interfaces
{
    [ServiceContract(Name = "IOptionSet")]
    public interface IOptionSet
    {
        [OperationContract]
        Task<List<OptionSet>> getOptionSetItems(string entityName,string optionSetAttributeName);
    }
}
