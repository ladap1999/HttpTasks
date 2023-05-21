using System.Dynamic;
using System.Net;
using NUnit.Framework;
using UserService.builders;
using UserService.Clients;
using UserService.models.request;
using static UserService.Extensions.HttpResponseMessageExtension;
using static UserService.Utils.TestData;

namespace UserService;

[TestFixture]
public class UserServiceTests
{
    private readonly UserServiceClient _userServiceClient = new UserServiceClient();

    RegisterUser user = new UserBuilder(new RegisterUser())
        .firstName("Jeanne")
        .lastName("Dark")
        .build();

    //1
    [Test]
    public async Task UserService_CreateEmptyUser_StatusCodeIsOk()
    {
        var user = new UserBuilder(new RegisterUser())
            .firstName("")
            .lastName("")
            .build();

        var response = await _userServiceClient.RegisterUser(user);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    //2
    //This is failed according to documentation file
    [Test]
    public async Task UserService_CreateUserWithNullFields_StatusCodeIsOk()
    {
        var user = new UserBuilder(new RegisterUser())
            .firstName(null)
            .lastName(null)
            .build();

        var response = await _userServiceClient.RegisterUser(user);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    //3
    [Test]
    public async Task UserService_CreateUserWithDigitsInFields_StatusCodeIsBadRequest()
    {
        var user = new UserWithDigitFieldsBuilder(new RegisterUserWithDigitFields())
            .firstName(12)
            .lastName(22222)
            .build();

        var response = await _userServiceClient.RegisterUser(user);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    //4
    [Test]
    public async Task UserService_CreateUserWithSpecialCharactersInFields_StatusCodeIsOk()
    {
        var user = new UserBuilder(new RegisterUser())
            .firstName("~!@#$%^&*()-_=+[]\\{}|;':\",./<>?")
            .lastName("~!@#$%^&*()-_=+[]\\{}|;':\",./<>?")
            .build();

        var response = await _userServiceClient.RegisterUser(user);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    //5
    [Test]
    public async Task UserService_CreateUserFieldsWithOneSymbol_StatusCodeIsOk()
    {
        var user = new UserBuilder(new RegisterUser())
            .firstName("j")
            .lastName("s")
            .build();

        var response = await _userServiceClient.RegisterUser(user);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    //6
    [Test]
    public async Task UserService_CreateUserWithFieldsLenghtMoreThan100_StatusCodeIsOk()
    {
        var user = new UserBuilder(new RegisterUser())
            .firstName(longStringForFirstName)
            .lastName(longStringForSecondName)
            .build();
        var response = await _userServiceClient.RegisterUser(user);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    //7
    [Test]
    public async Task UserService_CreateUserWithUpperCaseValueInFields_StatusCodeIsOk()
    {
        var user = new UserBuilder(new RegisterUser())
            .firstName("JOHN")
            .lastName("SMITH")
            .build();
        var response = await _userServiceClient.RegisterUser(user);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    //8
    [Test]
    public async Task UserService_CheckAutoincrementedIdOfTwoUsers_SecondIdMoreThenFirst()
    {
        var firstUserResponse = await _userServiceClient.RegisterUser(user);
        var secondUserResponse = await _userServiceClient.RegisterUser(user);

        Assert.That(secondUserResponse.GetId(), Is.EqualTo(firstUserResponse.GetId() + 1));
    }

    //9
    [Test]
    public async Task UserService_CheckIdOfNextUserAfterDeleted_IdIsAutoincremented()
    {
        var firstUserResponse = await _userServiceClient.RegisterUser(user);

        await _userServiceClient.DeleteUser(firstUserResponse.GetId());
        var secondUserResponse = await _userServiceClient.RegisterUser(user);

        Assert.That(secondUserResponse.GetId(), Is.EqualTo(firstUserResponse.GetId() + 1));
    }

    //21
    [Test]
    public async Task UserService_DeleteNotExistingUser_StatusCodeIsInternalServerError()
    {
        var id = int.MaxValue;
        var responseStatusCode = await _userServiceClient.DeleteUser(id);

        Assert.That(responseStatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }

    //22
    [Test]
    public async Task UserService_DeleteUserWithNotActiveStatus_StatusCodeIsOk()
    {
        var response = await _userServiceClient.RegisterUser(user);

        var responseStatusCode = await _userServiceClient.DeleteUser(response.GetId());

        Assert.That(responseStatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    //10
    //This is failed according to documentation file
    [Test]
    public async Task UserService_GetStatusForNonExistentUser_StatusIsFalse()
    {
        var nonExistingId = int.MaxValue;
        var responseStatusCode = (await _userServiceClient.GetUserStatus(nonExistingId)).StatusCode;

        Assert.That(responseStatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    //11
    [Test]
    public async Task UserService_GetActiveStatusOfDefaultUser_StatusIsNonActive()
    {
        var response = await _userServiceClient.RegisterUser(user);

        var responseUserStatus = _userServiceClient.GetUserStatus(response.GetId()).Result;

        Assert.False(responseUserStatus.GetBoolValue());
    }

    //12
    //16
    [Test]
    public async Task UserService_SetAndGetChangedTrueStatus_StatusIsNonActive()
    {
        var response = await _userServiceClient.RegisterUser(user);
        await _userServiceClient.SetUserStatus(response.GetId(), true);

        var responseUserStatusCodeForFalseStatus =
            await _userServiceClient.SetUserStatus(response.GetId(), false);
        var responseUserStatus = await _userServiceClient.GetUserStatus(response.GetId());

        Assert.That(responseUserStatusCodeForFalseStatus, Is.EqualTo(HttpStatusCode.OK));
        Assert.False(responseUserStatus.GetBoolValue());
    }

    //13
    //15
    [Test]
    public async Task UserService_SetAndGetChangedFalseStatus_StatusIsActive()
    {
        var response = await _userServiceClient.RegisterUser(user);

        var responseUserStatusCode = await _userServiceClient.SetUserStatus(response.GetId(), true);
        var responseUserStatus = await _userServiceClient.GetUserStatus(response.GetId());

        Assert.That(responseUserStatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.True(responseUserStatus.GetBoolValue());
    }

    //14
    //This is failed according to documentation file
    [Test]
    public async Task UserService_SetStatusNonExistentUser_StatusCodeIsNotFound()
    {
        var nonExistingId = int.MaxValue;
        var responseStatusCode = await _userServiceClient.SetUserStatus(nonExistingId, true);

        Assert.That(responseStatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    //19
    [Test]
    public async Task UserService_SetTrueStatusForActiveUser_StatusCodeIsOk()
    {
        var response = await _userServiceClient.RegisterUser(user);
        await _userServiceClient.SetUserStatus(response.GetId(), true);

        var responseUserStatusCodeForTrueStatus =
            await _userServiceClient.SetUserStatus(response.GetId(), true);
        var responseUserStatus = await _userServiceClient.GetUserStatus(response.GetId());

        Assert.That(responseUserStatusCodeForTrueStatus, Is.EqualTo(HttpStatusCode.OK));
        Assert.True(responseUserStatus.GetBoolValue());
    }
}