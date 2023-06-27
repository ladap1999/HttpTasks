using NUnit.Framework;
using System.Net;
using TechTalk.SpecFlow;
using UserService.Extensions;

namespace UserService.Steps
{
    [Binding]
    public class UserServiceAsserts
    {
        private  readonly ScenarioContext _context;
        public UserServiceAsserts(ScenarioContext context)
        {
            _context = context;
        }

        [Then(@"Status code is '(.*)'")]
        public void ThenStatusCodeIs(HttpStatusCode statusCode)
        {
            var response = (HttpResponseMessage)_context["response"];
            Assert.That(response.StatusCode, Is.EqualTo(statusCode));
        }

        [Then(@"Second ID is bigger then the first")]
        public void ThenSecondIdIsBiggerThenTheFirst()
        {
            var responses = (List<HttpResponseMessage>)_context["responses"];
            Assert.True(responses.Last().GetId() > responses.First().GetId());
        }

        [Then(@"Status of user is not active")]
        public void ThenStatusOfUserIsNotActive()
        {
            var responseUserStatus = (HttpResponseMessage)_context["responseUserStatus"];
            Assert.False(responseUserStatus.GetBoolValue());
        }

        [Then(@"Status of user is active")]
        public void ThenStatusOfUserIsActive()
        {
            var responseUserStatus = (HttpResponseMessage)_context["responseUserStatus"];
            Assert.True(responseUserStatus.GetBoolValue());
        }
    }

}