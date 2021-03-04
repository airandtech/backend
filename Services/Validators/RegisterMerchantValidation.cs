using System.Linq;
using AirandWebAPI.Services.Contract;
using AirandWebAPI.Services;
using AirandWebAPI.Core;
using AirandWebAPI.Models.Merchant;

namespace TheHangout.Services
{

    public class RegisterMerchantValidation : IValidation<RegisterMerchantVM>
    {
        private IUnitOfWork _unitOfWork;
        public RegisterMerchantValidation(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ValidationInfo Validate(RegisterMerchantVM model)
        {
            ValidationInfo validationInfo = new ValidationInfo();

            if (string.IsNullOrWhiteSpace(model.BusinessName))
                validationInfo.addInvalidationNarration("Business name is required");
            if (string.IsNullOrWhiteSpace(model.Address))
                validationInfo.addInvalidationNarration("Primary pickup address is required");
            if (string.IsNullOrWhiteSpace(model.AvgMonthlyDelivery))
                validationInfo.addInvalidationNarration("Average Monthly Delivery is required");
            if (string.IsNullOrWhiteSpace(model.DeliveryFrequency))
                validationInfo.addInvalidationNarration("Delivery Frequency is required");
            if (string.IsNullOrWhiteSpace(model.OwnerName))
                validationInfo.addInvalidationNarration("Name of business owner is required");
            if (string.IsNullOrWhiteSpace(model.OwnerPhone))
                validationInfo.addInvalidationNarration("Phone number for business owner is required");
            if (string.IsNullOrWhiteSpace(model.ProductCategory))
                validationInfo.addInvalidationNarration("Product category is required");

            if (_unitOfWork.Merchants.Find(x => x.BusinessName.ToUpper().Equals(model.BusinessName.ToUpper())).Count() > 0)
                validationInfo.addInvalidationNarration("Business name already exists");

            return validationInfo;
        }
    }

}