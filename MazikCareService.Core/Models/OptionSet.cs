using MazikCareService.CRMRepository;
using MazikLogger;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MazikCareService.Core.Models
{
    public class OptionSet
    {

        public string OptionSetValue    { get; set; }
        public string OptionSetName     { get; set; }
        public int defaultValue         { get; set; }

        public string OptionSetColor { get; set; }

        public  List<OptionSet> getOptionSetItems(string entityName, string optionSetAttributeName)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                List<OptionSet> OptionSet = new List<OptionSet>();
                OptionMetadata[] optionList = null;

                if (string.IsNullOrEmpty(entityName))
                {
                    optionList = entityRepository.GetOptionSetItems(optionSetAttributeName);
                }
                else
                {
                    optionList = entityRepository.GetOptionSetItems(entityName, optionSetAttributeName);
                }

                if (optionList != null)
                {
                    foreach (OptionMetadata o in optionList)
                    {
                        OptionSet model = new OptionSet();
                        model.OptionSetName = o.Label.LocalizedLabels[0].Label;
                        model.OptionSetValue = o.Value.Value.ToString();

                        if (!string.IsNullOrEmpty(o.Color))
                        {
                            model.OptionSetColor = o.Color.ToString();
                        }
                        //model.defaultValue = optionList.FirstOrDefault().Value.Value;// o.Value.GetValueOrDefault();
                        OptionSet.Add(model);
                    }
                }
                return OptionSet;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            
        }        
    }
}
