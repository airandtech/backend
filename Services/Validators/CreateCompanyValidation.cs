using System;
using System.Linq;
using AirandWebAPI.Services.Contract;
using AirandWebAPI.Services;
using AirandWebAPI.Models.Auth;
using AirandWebAPI;
using AirandWebAPI.Models.Direction;
using AirandWebAPI.Models.Dispatch;
using AirandWebAPI.Models.Company;

namespace TheHangout.Services
{

    public class CreateCompanyValidation : IValidation<CreateCompanyVM>
    {

        public ValidationInfo Validate(CreateCompanyVM model)
        {
            ValidationInfo validationInfo = new ValidationInfo();

            if (string.IsNullOrWhiteSpace(model.AccountName))
                validationInfo.addInvalidationNarration("Account Name is required");
            if (string.IsNullOrWhiteSpace(model.AccountNumber))
                validationInfo.addInvalidationNarration("Account Number required");
            if (string.IsNullOrWhiteSpace(model.BankName))
                validationInfo.addInvalidationNarration("Bank Name required");
            if (string.IsNullOrWhiteSpace(model.CompanyAddress))
                validationInfo.addInvalidationNarration("Company Address required");
            if (string.IsNullOrWhiteSpace(model.CompanyName))
                validationInfo.addInvalidationNarration("Company Name required");
            if (string.IsNullOrWhiteSpace(model.OfficeArea))
                validationInfo.addInvalidationNarration("Office Area required");

            return validationInfo;
        }
    }

}