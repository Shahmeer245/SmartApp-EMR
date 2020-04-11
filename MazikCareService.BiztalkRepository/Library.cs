using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.BiztalkRepository
{
   public class Library
    {
        public static string SequentialGuid()
        {
          
            return Guid.NewGuid().ToString();
           
        }
    }
}
