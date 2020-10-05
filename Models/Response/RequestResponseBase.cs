using System;

namespace AirandWebAPI.Models{

    public abstract class RequestResponseBase{

        public static string SUCCESSFULL = "Approved or Completed Successfully";
        public static string FAILED = "Failed";
        public static string INSUFFICIENT_FUNDS = "Insufficient Funds";
        public static string TRANSFER_FAILED = "Transfer Failed";
        public static string PAYMENT_FAILED = "Payment Failed";
        public Boolean status { get; set; }
        public string message { get; set; }
    }

}