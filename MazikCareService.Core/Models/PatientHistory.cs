using MazikCareService.CRMRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models
{
    public class PatientHistory
    {
        public string narrativeText { get; set; }
        public DateTime createdDateTime { get; set; }
        public string userName { get; set; }
        public int historyType { get; set; }

        public async Task<List<PatientHistory>> getPatientHistory(string patientguid, int type)
        {
            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();
                ClinicalTemplate template = new ClinicalTemplate();
                List<PatientHistory> modelList = new List<PatientHistory>();

                List<ClinicalTemplateNarration> patientNarration = template.getPatientsClinicalTempalteNarration(patientguid, "", "", true, type);
                PatientHistory model;

                foreach (ClinicalTemplateNarration narration in patientNarration)
                {
                    model = new PatientHistory();
                    string narrativeText = string.Empty;

                    model.createdDateTime = narration.createdDateTime;
                    model.userName = narration.userName;
                    model.historyType = narration.historyType;

                    narrativeText = narration.narrativeText;

                    if(!string.IsNullOrEmpty(narration.ans))
                    {
                        narrativeText = narrativeText.Replace("$1", narration.ans);
                    }
                    else if (narration.choicesName != null && narration.choicesName.Count() > 0)
                    {
                        string choices = string.Empty;

                        foreach (string item in narration.choicesName)
                        {
                            if(string.IsNullOrEmpty(choices))
                            {
                                choices = item;
                            }
                            else
                            {
                                choices += ", " + item;
                            }
                        }

                        narrativeText = narrativeText.Replace("$1", choices);
                    }
                    else
                    {
                        narrativeText = narrativeText.Replace("$1", string.Empty);
                    }

                    narrativeText = narrativeText.Replace("$2", narration.comments);

                    model.narrativeText = narrativeText;

                    modelList.Add(model);
                }

                return modelList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
