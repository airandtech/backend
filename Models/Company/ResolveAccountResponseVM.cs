using System.Collections.Generic;
namespace AirandWebAPI.Models.Company
{

    // public class ResolveAccountResponseVM : ResolveAccountVM
    // {
    //     public string AccountName { get; set; }

    // }

    public class ResolveAccountRootResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public ResolveAccountResponseVM data { get; set; }
    }

    public class ResolveAccountResponseVM
    {
        public string account_number { get; set; }
        public string account_name { get; set; }
    }

}