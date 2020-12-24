namespace AirandWebAPI.Models{

    public class ErrorResponse : RequestResponseBase
    {
        public string error {get;set;}

         public ErrorResponse(bool status, string responseMessage, string validationErrors){

            this.status = status;
            this.message = responseMessage;
            this.error = validationErrors;
        }
    }
}