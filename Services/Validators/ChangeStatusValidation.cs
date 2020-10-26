using System;
using System.Linq;
using AirandWebAPI.Services.Contract;
using AirandWebAPI.Services;
using AirandWebAPI.Models.Auth;
using AirandWebAPI;
using AirandWebAPI.Models.Direction;
using AirandWebAPI.Models.Dispatch;

namespace TheHangout.Services
{

    public class ChangeStatusValidation : IValidation<ChangeStatusVM>
    {

        public ValidationInfo Validate(ChangeStatusVM model)
        {
            ValidationInfo validationInfo = new ValidationInfo();

            if (string.IsNullOrWhiteSpace(model.orderId))
                validationInfo.addInvalidationNarration("Order Id is required");
            if (string.IsNullOrWhiteSpace(model.status))
                validationInfo.addInvalidationNarration("Status is required");
            else if (!(new string[3]{"Pending", "InProgress", "Completed"}.Contains(model.status)))
                validationInfo.addInvalidationNarration("Invalid status");

            return validationInfo;
        }
    }

}