using MazikCareService.CRMRepository;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MazikCareWebApi.ApiHelpers
{
    public class ExceptionLogger
    {
        public void Log(Exception ex, string requestBody = "")
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
   
            //Get Values
            string operation = trace.GetFrame(0).GetMethod().ReflectedType.FullName;
            int index = operation.LastIndexOf(">");
            index = index + 1;
            if(index != 1)
            {
                operation = operation.Substring(0, index);
            }
            
            string exMessage = ex.Message;
            string stackTrace = ex.StackTrace;
            int lineNo = trace.GetFrame(0).GetFileLineNumber();
            int colNo = trace.GetFrame(0).GetFileColumnNumber();
            
            // Log Values
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            Entity mzk_exceptionlog = new Entity("mzk_exceptionlog");
            mzk_exceptionlog.Attributes["mzk_exceptionarea"] = new OptionSetValue((int)mzk_exceptionarea.Service);
            mzk_exceptionlog.Attributes["mzk_operation"] = operation;
            mzk_exceptionlog.Attributes["mzk_exceptionmessage"] = exMessage;
            mzk_exceptionlog.Attributes["mzk_stacktrace"] = stackTrace;
            mzk_exceptionlog.Attributes["mzk_linenumber"] = lineNo;
            mzk_exceptionlog.Attributes["mzk_columnnumber"] = colNo;
            mzk_exceptionlog.Attributes["mzk_inputparameters"] = requestBody;
            entityRepository.CreateEntity(mzk_exceptionlog);

        }
    }
}