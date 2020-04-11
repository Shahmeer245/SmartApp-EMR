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
    public class ProductController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> getProduct(Products inputmodel)
        {
            List<Products> data = new List<Products>();
            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");
            
            try
            {
                ApiResponseModel<List<Products>> model = new ApiResponseModel<List<Products>>() { };
                var client = ServiceFactory.GetService(typeof(Products));
                data =  client.getProduct(inputmodel, inputmodel.currentpage);
                if (data != null)
                {
                    model.data.records = data;
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = Pagination.totalCount.ToString();
                }
                return Response.Success<List<Products>>(model);
            }
             catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        [HttpPost]
        public async Task<HttpResponseMessage> AddUserFavourite(Products inputmodel)
        {
            string data = string.Empty;
            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };
                var client = ServiceFactory.GetService(typeof(Products));
                data = await client.AddUserFavourite(inputmodel);
                model.data.records = data;

                return Response.Success<string>(model);
            }
             catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        [HttpPost]
        public async Task<HttpResponseMessage> removeUserFavourite(Products inputmodel)
        {
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client = ServiceFactory.GetService(typeof(Products));
                data = await client.removeUserFavourite(inputmodel);
                model.data.records = data;
                return Response.Success<bool>(model);
            }
             catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
    }
}
