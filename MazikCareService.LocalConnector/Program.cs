using MazikCareService.Core;
using MazikCareService.Core.Models;
using MazikCareService.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.LocalConnector
{
    class Program
    {
        static void Main(string[] args)
        {
          
            var sh2 = new PatientServiceHost(typeof(CrmService));
            sh2.Open();
            Console.WriteLine("CRM Service Started");

            var sh3 = new PatientServiceHost(typeof(PatientAllergy));
            sh3.Open();

            Console.WriteLine("PatientAllergy Service Started");

            Console.ReadLine();

            sh2.Close();
            sh3.Close();

        }
    }
}
