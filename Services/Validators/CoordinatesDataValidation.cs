using System;
using System.Linq;
using AirandWebAPI.Services.Contract;
using AirandWebAPI.Services;
using AirandWebAPI.Models.Auth;
using AirandWebAPI;
using AirandWebAPI.Models.Direction;

namespace TheHangout.Services
{

    public class CoordinatesDataValidation : IValidation<Coordinates>
    {

        public ValidationInfo Validate(Coordinates model)
        {
            ValidationInfo validationInfo = new ValidationInfo();

            if (string.IsNullOrWhiteSpace(model.latitude))
                validationInfo.addInvalidationNarration("Latitude is required");
            if (string.IsNullOrWhiteSpace(model.longitude))
                validationInfo.addInvalidationNarration("Longitude is required");

            return validationInfo;
        }
    }

}