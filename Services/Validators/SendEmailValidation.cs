using System.Linq;
using AirandWebAPI.Services.Contract;
using AirandWebAPI.Services;
using AirandWebAPI.Models.Auth;
using AirandWebAPI.Models;

namespace TheHangout.Services
{

    public class SendEmailValidation : IValidation<EmailVM>
    {
        public ValidationInfo Validate(EmailVM model)
        {

            ValidationInfo validationInfo = new ValidationInfo();
            if (string.IsNullOrWhiteSpace(model.name))
                validationInfo.addInvalidationNarration("sender's name is required");
            if (string.IsNullOrWhiteSpace(model.to))
                validationInfo.addInvalidationNarration("recipient address is required");
            if (string.IsNullOrWhiteSpace(model.subject))
                validationInfo.addInvalidationNarration("subject is required");
            if (string.IsNullOrWhiteSpace(model.message))
                validationInfo.addInvalidationNarration("message is required");

            return validationInfo;
        }
    }

}