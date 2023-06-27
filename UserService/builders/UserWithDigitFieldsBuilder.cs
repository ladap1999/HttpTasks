using UserService.Models.Request;

namespace UserService.Builders;

public class UserWithDigitFieldsBuilder
{
    private RegisterUserWithDigitFields _userWithDigitRequestModel { get; }

    public UserWithDigitFieldsBuilder(RegisterUserWithDigitFields user)
    {
        _userWithDigitRequestModel = user;
    }

    public UserWithDigitFieldsBuilder firstName(int name)
    {
        _userWithDigitRequestModel.FirstName = name;
        return this;
    }

    public UserWithDigitFieldsBuilder lastName(int lastName)
    {
        _userWithDigitRequestModel.LastName = lastName;
        return this;
    }

    public RegisterUserWithDigitFields build()
    {
        return _userWithDigitRequestModel;
    }
}