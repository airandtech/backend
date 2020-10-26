namespace PazarWebApi.Core.Domain
{
    public static class OrderStatus
    {
        public const string Pending = "01";
        public const string Completed = "00";
        public const string InProgress = "02";

        public static string GetStatusFromCode(string code){
            switch (code.ToUpper())
            {
                case "PENDING":
                return "01";
                case "COMPLETED":
                return "00";
                case "INPROGRESS":
                return "02";
                default:
                return "99";
            }
        }
    }
}