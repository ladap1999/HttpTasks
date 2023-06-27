using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using UserService.Builders;
using UserService.Clients;
using UserService.Extensions;
using UserService.Models.Request;


namespace UserService.Steps;

[Binding]
public class UserSteps
{

    private readonly UserServiceClient _userServiceClient = UserServiceClient.Instance;
    private ScenarioContext _context;
    public UserSteps(ScenarioContext context)
    {
        _context = context;
    }

    [BeforeScenario]
    public void BeforeScenario()
    {
        //TODO: implement logic that has to run before executing each scenario
    }

    [AfterScenario]
    public void AfterScenario()
    {
        //TODO: implement logic that has to run after executing each scenario
    }


    [Given(@"New user with firstName (.*) and lastName (.*) was created")]
    public async Task GivenNewUserWithFirstNameAndLastName(string firstName, string lastName)
    { 
        RegisterUser user = new UserBuilder(new RegisterUser())
        .firstName(firstName)
        .lastName(lastName)
        .build();

        var response = await _userServiceClient.RegisterUser(user);
        _context["response"] = response;
    }

    [Given(@"New user with digit firstName '(.*)' and digit lastName '(.*)' was created")]
    public async Task GivenNewUserWithDigitFirstNameAndDigitLastNameWasCreated(int firstName, int lastName)
    {
        RegisterUserWithDigitFields user = new UserWithDigitFieldsBuilder(new RegisterUserWithDigitFields())
            .firstName(firstName)
            .lastName(lastName)
            .build();

        var response = await _userServiceClient.RegisterUser(user);
        _context["response"] = response;
    }



    [Given(@"Users with firstName and lastName were created")]
    public async Task GivenUsersWithFirstNameAndLastNameWereCreated(Table table)
    {
        var responsesList = new List<HttpResponseMessage>();
        var usersList = table.CreateSet<(string, string)>();
        foreach (var element in usersList)
        {
            var user = new UserBuilder(new RegisterUser())
                .firstName(element.Item1)
                .lastName(element.Item2)
                .build();
           responsesList.Add(await _userServiceClient.RegisterUser(user));
        }
        _context["responses"] = responsesList;
    }


    [When(@"User with non existed Id is deleted")]
    public async Task WhenUserWithNonExistedIdIsDeleted()
    {
        var id = int.MaxValue;
        var response = await _userServiceClient.DeleteUser(id);
        _context["response"] = response;
    }


    [When(@"The first user is deleted and new user with firstName '(.*)' and lastName '(.*)' is created")]
    public async Task WhenTheFirstUserWasDeletedAndNewUserWithFirstNameAndLastNameIsCreated(string firstName, string lastName)
    {
        var responsesList = new List<HttpResponseMessage>();
        var firstResponse = (HttpResponseMessage)_context["response"];
        responsesList.Add(firstResponse);

        await _userServiceClient.DeleteUser(firstResponse.GetId());

        RegisterUser user = new UserBuilder(new RegisterUser())
            .firstName(firstName)
            .lastName(lastName)
            .build();
        
        responsesList.Add(await _userServiceClient.RegisterUser(user));
        _context["responses"] = responsesList;
    }

    [When(@"Created user is deleted")]
    public async Task WhenCreatedUserIsDeleted()
    {
        var responseAfterDelete = await _userServiceClient.DeleteUser(((HttpResponseMessage)_context["response"]).GetId());
        _context["responseAfterDelete"] = responseAfterDelete;
    }

    [When(@"Get status of non existed")]
    public async Task WhenGetStatusOfNonExistedUser()
    {
        var nonExistingId = int.MaxValue;
        _context["response"] = (await _userServiceClient.GetUserStatus(nonExistingId));
    }

    [When(@"Get status of created user")]
    public void WhenGetStatusOfCreatedUser()
    {
        _context["responseUserStatus"] = _userServiceClient.GetUserStatus(((HttpResponseMessage)_context["response"]).GetId()).Result;
    }

    [When(@"Set status '(.*)' to created user")]
    [Given(@"Set status '(.*)' to created user")]
    public async Task WhenSetStatusToCreatedUser(bool isActive)
    {
        var response = await _userServiceClient.SetUserStatus(((HttpResponseMessage)_context["response"]).GetId(), isActive);
        _context["responseStatus"] = response;
    }

    [When(@"Set status for user with invalid Id")]
    public async Task WhenSetStatusForUserWithInvalidId()
    {
        var nonExistingId = int.MaxValue;
        _context["response"] = await _userServiceClient.SetUserStatus(nonExistingId, true);
    }
}
