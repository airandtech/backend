using System.Linq;
using AirandWebAPI.Services.Contract;
using AirandWebAPI.Services;
using AirandWebAPI.Models.Auth;

namespace TheHangout.Services
{

    public class UserRegistrationValidation : IValidation<RegisterModel>
    {
        public ValidationInfo Validate(RegisterModel model)
        {

            ValidationInfo validationInfo = new ValidationInfo();
            if (string.IsNullOrWhiteSpace(model.Username))
                validationInfo.addInvalidationNarration("Username is required");
            if (string.IsNullOrWhiteSpace(model.Password))
                validationInfo.addInvalidationNarration("Password is required");

            return validationInfo;
        }
    }

}