namespace AirandWebAPI.Models{

    public class GenericResponse<T> : RequestResponseBase{

        public T data {get;set;}
         public GenericResponse(bool status, string responseMessage, T t){
            
            this.status = status;
            this.message = responseMessage;
            this.data = t;
        }
    }

   
}