using UserService.models.request;

namespace UserService.builders;

public class UserBuilder
{
    private RegisterUser _userRequestModel { get; }

    public UserBuilder(RegisterUser user)
    {
        _userRequestModel = user;
    }

    public UserBuilder firstName(string name)
    {
        _userRequestModel.firstName = name;
        return this;
    }

    public UserBuilder lastName(string lastName)
    {
        _userRequestModel.lastName = lastName;
        return this;
    }

    public RegisterUser build()
    {
        return _userRequestModel;
    }
}