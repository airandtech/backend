using AirandWebAPI.Services.Contract;

namespace AirandWebAPI.Services{
    public interface IValidation<T> {
        ValidationInfo Validate(T model);
    }
}