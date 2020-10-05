using System;
using System.Collections.Generic;
using AirandWebAPI.Models;

namespace AirandWebAPI.Exceptions
{
    public  class ExceptionHandler : RequestResponseBase
    {
        private List<string> InnerExceptionDetails(Exception ex){
            List<string> innerExceptionList = new List<string>();
            while (ex.InnerException != null) 
               innerExceptionList.Add(ex.InnerException.Message);
            return innerExceptionList;
        }

        public string error {get;set;}
        public List<string> innerException {get;set;}

         public ExceptionHandler( bool status, Exception exception, string responseMessage){
            this.innerException = InnerExceptionDetails(exception);
            this.status = status;
            this.message = responseMessage;
            this.error = exception.Message;
            Console.WriteLine($"++++==> Exception Occured: in {this.GetType().Name} Exception: {exception.Message} \n Exception Inner Details: {exception.InnerException} \n Exception Stacktrace: {exception.StackTrace}");
        }
    }
}