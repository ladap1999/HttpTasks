using UserService.Models.Request;

namespace UserService.Builders;

public class UserBuilder
{
    private RegisterUser _userRequestModel { get; }

    public UserBuilder(RegisterUser user)
    {
        _userRequestModel = user;
    }

    public UserBuilder firstName(string name)
    {
        _userRequestModel.FirstName = name;
        return this;
    }

    public UserBuilder lastName(string lastName)
    {
        _userRequestModel.LastName = lastName;
        return this;
    }

    public RegisterUser build()
    {
        return _userRequestModel;
    }
}