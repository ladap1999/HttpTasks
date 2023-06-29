using UserService.Models.Request;

namespace UserService.Builders;

public class UserBuilder
{
    private RegisterUser _userRequestModel { get; }

    public UserBuilder(RegisterUser user)
    {
        _userRequestModel = user;
    }

    public UserBuilder FirstName(string name)
    {
        _userRequestModel.FirstName = name;
        return this;
    }

    public UserBuilder LastName(string lastName)
    {
        _userRequestModel.LastName = lastName;
        return this;
    }

    public RegisterUser Build()
    {
        return _userRequestModel;
    }
}