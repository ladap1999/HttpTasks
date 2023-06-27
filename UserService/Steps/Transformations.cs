using TechTalk.SpecFlow;
using static UserService.Utils.TestData;

namespace UserService.Steps
{
    [Binding]
    public sealed class Transformations
    {
        [StepArgumentTransformation("null")]
        public string TransformEmptyString()
        {
            return null;
        }

        [StepArgumentTransformation("FIRST_NAME_LONG")]
        public string TransformLongFirstNameString()
        {
            return longStringForFirstName;
        }

        [StepArgumentTransformation("LAST_NAME_LONG")]
        public string TransformLongLastNameString()
        {
            return longStringForSecondName;
        }
    }
}