using MazikCareService.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models
{
    public class DropDown
    {
        public string text { get; set; }
        public string value { get; set; }
        public string IsDefault { get; set; }
        public string address { get; set; }
        public string color { get; set; }
    }
}
