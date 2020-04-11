using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace MazikCareService.Controllers
{
    public class ApiExplorerController : Controller
    {
        // GET: api/ApiExplorer
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
