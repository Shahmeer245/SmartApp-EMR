using Helper;
using MazikCareService.Core.Models;
using MazikCareWebApi.ApiHelpers;
using MazikCareWebApi.Filters;
using MazikCareWebApi.Models;
using MazikLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace MazikCareWebApi.Controllers
{
    [MzkAuthorize]
    public class WorklistController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> getWorklistTypes(ApiModel inputmodel)
        {
            List<Worklist> dataByUser = new List<Worklist>(); // Temp. work for now , to be removed later.
            List<Worklist> dataByRole = new List<Worklist>();

            List<Worklist> uniqueWorkerList = new List<Worklist>();

            try
            {
                ApiResponseModel<List<Worklist>> model = new ApiResponseModel<List<Worklist>>() { };

                var client = ServiceFactory.GetService(typeof(Worklist));
                dataByUser = await client.getWorklistTypes(inputmodel.userId);

                dataByRole = await client.getWorklistTypes(inputmodel.userId,true);

                dataByUser.AddRange(dataByRole);

                foreach (var recordFrom in dataByUser)
                {
                    bool insertUniqureRecord = true;
                   
                    foreach (var recordTo in uniqueWorkerList)
                    {
                        
                        if(recordFrom.worklistTypeId == recordTo.worklistTypeId)
                        {
                            insertUniqureRecord = false;
                            break;
                        }
                    }
                    if (insertUniqureRecord == true)
                    {
                        uniqueWorkerList.Add(recordFrom);
                    }
                }

                if (dataByUser != null)
                {                   
                    model.data.records = uniqueWorkerList.ToList<Worklist>();
                }

                return Response.Success<List<Worklist>>(model);
            }
             catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> getUserWorkListData(ApiModel inputmodel)
        {
            List<Activity> data = new List<Activity>();

            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<Activity>> model = new ApiResponseModel<List<Activity>>() { };

                var client = ServiceFactory.GetService(typeof(Worklist));
                data = await client.getUserWorkListData(inputmodel.worklistTypeID, inputmodel.userId, inputmodel.clinicId, inputmodel.date, inputmodel.SearchFilters, inputmodel.searchOrder,inputmodel.timezone, inputmodel.resourceId);


                if (data != null)
                {
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = data.Count().ToString();
                    int pageSize = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                    model.data.records = data.Take<Activity>(Convert.ToInt32(CurrentPage * pageSize)).Skip((CurrentPage - 1) * pageSize).ToList();
                    //model.data.records = data;
                }

                return Response.Success<List<Activity>>(model);
            }
             catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
    }
}
