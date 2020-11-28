using System;
using System.Linq;
using AirandWebAPI.Services.Contract;
using AirandWebAPI.Services;
using AirandWebAPI.Models.Auth;
using AirandWebAPI;
using AirandWebAPI.Models.Dispatch;
using AirandWebAPI.Models.Company;

namespace TheHangout.Services
{

    public class AddDispatchManagerValidation : IValidation<AddDispatchManagerVM>
    {

        public ValidationInfo Validate(AddDispatchManagerVM model)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            if (model.managerDetails == null)
            {
                validationInfo.addInvalidationNarration("Dispatch manager node is empty");
            }
            else
            {
                foreach (var item in model.managerDetails)
                {
                    if (string.IsNullOrWhiteSpace(item.Name)
                        || string.IsNullOrWhiteSpace(item.Email)
                        || string.IsNullOrWhiteSpace(item.Phone)
                    )
                    {
                        validationInfo.addInvalidationNarration("Missing fields in manager details");
                        break;
                    }
                }
            }

            return validationInfo;
        }
    }

}