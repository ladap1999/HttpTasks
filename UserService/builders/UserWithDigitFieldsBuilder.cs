using UserService.models.request;

namespace UserService.builders;

public class UserWithDigitFieldsBuilder
{
    private RegisterUserWithDigitFields _userWithDigitRequestModel { get; }

    public UserWithDigitFieldsBuilder(RegisterUserWithDigitFields user)
    {
        _userWithDigitRequestModel = user;
    }

    public UserWithDigitFieldsBuilder firstName(int name)
    {
        _userWithDigitRequestModel.firstName = name;
        return this;
    }

    public UserWithDigitFieldsBuilder lastName(int lastName)
    {
        _userWithDigitRequestModel.lastName = lastName;
        return this;
    }

    public RegisterUserWithDigitFields build()
    {
        return _userWithDigitRequestModel;
    }
}