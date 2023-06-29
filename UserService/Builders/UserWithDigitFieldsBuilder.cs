using UserService.Models.Request;

namespace UserService.Builders;

public class UserWithDigitFieldsBuilder
{
    private RegisterUserWithDigitFields _userWithDigitRequestModel { get; }

    public UserWithDigitFieldsBuilder(RegisterUserWithDigitFields user)
    {
        _userWithDigitRequestModel = user;
    }

    public UserWithDigitFieldsBuilder FirstName(int name)
    {
        _userWithDigitRequestModel.FirstName = name;
        return this;
    }

    public UserWithDigitFieldsBuilder LastName(int lastName)
    {
        _userWithDigitRequestModel.LastName = lastName;
        return this;
    }

    public RegisterUserWithDigitFields Build()
    {
        return _userWithDigitRequestModel;
    }
}