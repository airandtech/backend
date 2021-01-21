using System;
using System.Linq;
using AirandWebAPI.Services.Contract;
using AirandWebAPI.Services;
using AirandWebAPI.Models.Auth;
using AirandWebAPI;
using AirandWebAPI.Models.Direction;
using AirandWebAPI.Models.Dispatch;
using AirandWebAPI.Models.Company;
using AirandWebAPI.Core;

namespace TheHangout.Services
{

    public class CreateCompanyValidation : IValidation<CreateCompanyVM>
    {
        private IUnitOfWork _unitOfWork;
        public CreateCompanyValidation(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

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

            // if (_unitOfWork.Companies.Find(x => x.UserId.Equals(model.UserId)).Count() > 0)
            //     validationInfo.addInvalidationNarration("Company already exists for user");

            return validationInfo;
        }
    }

}