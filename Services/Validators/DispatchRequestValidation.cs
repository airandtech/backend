using System;
using System.Linq;
using AirandWebAPI.Services.Contract;
using AirandWebAPI.Services;
using AirandWebAPI.Models.Auth;
using AirandWebAPI;
using AirandWebAPI.Models.Dispatch;

namespace TheHangout.Services
{

    public class DispatchRequestValidation : IValidation<RideOrderRequest>
    {

        public ValidationInfo Validate(RideOrderRequest model)
        {
            ValidationInfo validationInfo = new ValidationInfo();
            if (model.Delivery == null || model.PickUp == null)
            {
                validationInfo.addInvalidationNarration("Delivery or PickUp node is empty");
            }
            else
            {
                var pickUp = model.PickUp;
                if (string.IsNullOrWhiteSpace(pickUp.Address) || string.IsNullOrWhiteSpace(pickUp.Email) || string.IsNullOrWhiteSpace(pickUp.Name)
                   || string.IsNullOrWhiteSpace(pickUp.Phone) || string.IsNullOrWhiteSpace(pickUp.RegionCode) || string.IsNullOrWhiteSpace(pickUp.AreaCode))
                {
                    validationInfo.addInvalidationNarration("Missing fields in pickup");
                }
                foreach (var item in model.Delivery)
                {
                    if (string.IsNullOrWhiteSpace(item.Address) || string.IsNullOrWhiteSpace(item.Email) || string.IsNullOrWhiteSpace(item.Name)
                   || string.IsNullOrWhiteSpace(item.Phone) || string.IsNullOrWhiteSpace(item.RegionCode) || string.IsNullOrWhiteSpace(item.AreaCode))
                    {
                        validationInfo.addInvalidationNarration("Missing fields in delivery");
                        break;
                    }
                }
            }

            return validationInfo;
        }
    }

}