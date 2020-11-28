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

    public class AddRidersValidation : IValidation<AddRidersVM>
    {

        public ValidationInfo Validate(AddRidersVM model)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            if (model.ridersDetails == null)
            {
                validationInfo.addInvalidationNarration("Rider details node is empty");
            }
            else
            {
                foreach (var item in model.ridersDetails)
                {
                    if (string.IsNullOrWhiteSpace(item.Name)
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