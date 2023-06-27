using System.Net;
using NUnit.Framework;
using UserService.Builders;
using UserService.Clients;
using UserService.Extensions;
using UserService.Models.Request;
using WalletService.Builders;
using WalletService.Clients;
using WalletService.Models.Request;
using static UserService.Utils.TestData;


namespace WalletService.tests;

[TestFixture]
public class WalletServiceTests
{
    private WalletServiceClient _walletService = new WalletServiceClient();
    private readonly UserServiceClient _userServiceClient = UserServiceClient.Instance;

    RegisterUser user = new UserBuilder(new RegisterUser())
        .firstName("Jeanne")
        .lastName("Dark")
        .build();

    //1
    //2
    [Test]
    public async Task WalletService_GetNotActiveUserBalance_StatusCodeIsInternalErrorWithMessage()
    {
        var id = (await _userServiceClient.RegisterUser(user)).GetId();

        var response = await _walletService.GetBalance(id);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        Assert.That(response.Content.ReadAsStringAsync().Result, Is.EqualTo(notActiveExpectedMessage));
    }

    //3
    [Test]
    public async Task WalletService_GetNotExistingUserBalance_StatusCodeIsInternalErrorWithMessage()
    {
        var id = int.MaxValue;

        var response = await _walletService.GetBalance(id);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        Assert.That(response.Content.ReadAsStringAsync().Result, Is.EqualTo(notActiveExpectedMessage));
    }

    //43
    [Test]
    public async Task WalletService_ChargeBalanceForNotActiveUser_StatusCodeIsInternalServerError()
    {
        var id = (await _userServiceClient.RegisterUser(user)).GetId();
        var charge = new ChargeBuilder(new Charge())
            .userId(id)
            .amount(100.1)
            .build();

        var response = await _walletService.Charge(charge);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        Assert.That(response.Content.ReadAsStringAsync().Result, Is.EqualTo(notActiveExpectedMessage));
    }

    //44
    [Test]
    public async Task WalletService_ChargeBalanceForNotExistingUser_StatusCodeIsInternalErrorWithMessage()
    {
        var id = int.MaxValue;
        var charge = new ChargeBuilder(new Charge())
            .userId(id)
            .amount(12.5)
            .build();

        var response = await _walletService.Charge(charge);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        Assert.That(response.Content.ReadAsStringAsync().Result, Is.EqualTo(notActiveExpectedMessage));
    }

    //45
    //Message doesn't correspond to documentation
    [Test]
    public async Task WalletService_ChargeNegativeValueMoreThenBalance_StatusCodeIsInternalErrorWithMessage()
    {
        var id = (await _userServiceClient.RegisterUser(user)).GetId();
        await _userServiceClient.SetUserStatus(id, true);
        var charge = new ChargeBuilder(new Charge())
            .userId(id)
            .amount(30)
            .build();
        await _walletService.Charge(charge);

        charge = new ChargeBuilder(charge)
            .amount(-30.01)
            .build();
        var response = await _walletService.Charge(charge);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        Assert.That(response.Content.ReadAsStringAsync().Result,
            Is.EqualTo("User has '30.0', you try to charge '-30.01'."));
    }

    //53
    [Test]
    public async Task WalletService_ChargeBalanceMinusInitialValue_StatusCodeIsOk()
    {
        var id = (await _userServiceClient.RegisterUser(user)).GetId();
        await _userServiceClient.SetUserStatus(id, true);
        var charge = new ChargeBuilder(new Charge())
            .userId(id)
            .amount(30)
            .build();
        await _walletService.Charge(charge);

        charge = new ChargeBuilder(charge)
            .amount(-30)
            .build();
        var response = await _walletService.Charge(charge);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    //54
    //Message doesn't correspond to documentation
    //According to documentation, it is not possible to charge negative value
    [Test]
    public async Task WalletService_ChargeBalanceMinusInitialLessThenZero_StatusInternalServerError()
    {
        var id = (await _userServiceClient.RegisterUser(user)).GetId();
        await _userServiceClient.SetUserStatus(id, true);
        var charge = new ChargeBuilder(new Charge())
            .userId(id)
            .amount(-10)
            .build();
        await _walletService.Charge(charge);

        charge = new ChargeBuilder(charge)
            .amount(-10)
            .build();
        var response = await _walletService.Charge(charge);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        Assert.That(response.Content.ReadAsStringAsync().Result,
            Is.EqualTo("User has '0', you try to charge '-10.0'."));
    }

    //55
    //If amount< 0 and there is less money in the account than amount => Code:
    //500; Message  “User has '30', you try to charge '-40'.”
    //Message doesn't correspond to documentation
    //According to documentation, it is not possible to charge negative value
    [Test]
    public async Task WalletService_ChargeMinusValueWithInitialZero_StatusCodeIsInternalServerError()
    {
        var id = (await _userServiceClient.RegisterUser(user)).GetId();
        await _userServiceClient.SetUserStatus(id, true);
        var charge = new ChargeBuilder(new Charge())
            .userId(id)
            .amount(-30)
            .build();
        var response = await _walletService.Charge(charge);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }

    //56
    [Test]
    public async Task WalletService_ChargeSameBalanceAsInitial_StatusCodeIsOkBalancesAreEqual()
    {
        var initialBalance = 12.5;
        var expectedBalance = initialBalance * 2 + 10;
        var id = (await _userServiceClient.RegisterUser(user)).GetId();
        await _userServiceClient.SetUserStatus(id, true);
        var charge = new ChargeBuilder(new Charge())
            .userId(id)
            .amount(initialBalance)
            .build();
        await _walletService.Charge(charge);
        charge = new ChargeBuilder(charge)
            .amount(10)
            .build();
        await _walletService.Charge(charge);

        charge = new ChargeBuilder(charge)
            .amount(initialBalance)
            .build();
        var response = await _walletService.Charge(charge);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That((await _walletService.GetBalance(id)).GetDoubleValue(),
            Is.EqualTo(expectedBalance));
    }

    //15
    [Test]
    public async Task WalletService_GetBalanceForActiveUserWithoutTransaction_StatusCodeIsOk()
    {
        var id = (await _userServiceClient.RegisterUser(user)).GetId();
        await _userServiceClient.SetUserStatus(id, true);

        var response = await _walletService.GetBalance(id);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    //16
    [Test]
    public async Task WalletService_GetBalanceAfterRevert_StatusCodeIsOk()
    {
        var userId = (await _userServiceClient.RegisterUser(user)).GetId();
        await _userServiceClient.SetUserStatus(userId, true);
        var charge = new ChargeBuilder(new Charge())
            .userId(userId)
            .amount(10)
            .build();
        var transactionId = (await _walletService.Charge(charge)).GetStringValue();

        await _walletService.RevertTransaction(transactionId.Trim('"'));
        var response = await _walletService.GetBalance(userId);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response.GetDoubleValue(), Is.EqualTo(0));
    }

    //10
    //12
    //13
    [Test]
    [TestCase(0.01)]
    [TestCase(9999999.99)]
    [TestCase(10000000)]
    public async Task WalletService_GetBalanceAfterTransactionWithValue_StatusCodeIsOk(double amount)
    {
        var userId = (await _userServiceClient.RegisterUser(user)).GetId();
        await _userServiceClient.SetUserStatus(userId, true);
        var charge = new ChargeBuilder(new Charge())
            .userId(userId)
            .amount(amount)
            .build();
        await _walletService.Charge(charge);

        var response = await _walletService.GetBalance(userId);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response.GetDoubleValue(), Is.EqualTo(amount));
    }

    //11
    //14
    //According to documentation, it is possible to have negative balance.  In this case checked only 
    //function GetBalance with no attention to Charge function
    [Test]
    [TestCase(-0.01)]
    [TestCase(-10000000.01)]
    public async Task WalletService_GetBalanceAfterTransactionWithMinusBorderValue_StatusCodeIsOk(double amount)
    {
        var userId = (await _userServiceClient.RegisterUser(user)).GetId();
        await _userServiceClient.SetUserStatus(userId, true);
        var charge = new ChargeBuilder(new Charge())
            .userId(userId)
            .amount(amount)
            .build();
        await _walletService.Charge(charge);

        var response = await _walletService.GetBalance(userId);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response.GetDoubleValue(), Is.EqualTo(amount));
    }

    //46
    [Test]
    public async Task WalletService_ChargeZeroValue_StatusCodeIsInternalServerErrorWithMessage()
    {
        var id = (await _userServiceClient.RegisterUser(user)).GetId();
        await _userServiceClient.SetUserStatus(id, true);
        var charge = new ChargeBuilder(new Charge())
            .userId(id)
            .amount(0)
            .build();
        var response = await _walletService.Charge(charge);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        Assert.That(response.GetStringValue(), Is.EqualTo("Amount cannot be '0'"));
    }

    //47
    [Test]
    public async Task WalletService_ChargeMoreThenMaxValue_StatusCodeIsInternalServerErrorWithMessage()
    {
        var userId = (await _userServiceClient.RegisterUser(user)).GetId();
        await _userServiceClient.SetUserStatus(userId, true);
        var charge = new ChargeBuilder(new Charge())
            .userId(userId)
            .amount(10000000.01)
            .build();

        var response = await _walletService.Charge(charge);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        Assert.That(response.GetStringValue(),
            Is.EqualTo("After this charge balance could be '10000000.01', maximum user balance is '10000000'"));
    }

    //48
    [Test]
    public async Task WalletService_ChargeWith0_001Value_StatusCodeIsInternalServerErrorWithMessage()
    {
        var userId = (await _userServiceClient.RegisterUser(user)).GetId();
        await _userServiceClient.SetUserStatus(userId, true);
        var charge = new ChargeBuilder(new Charge())
            .userId(userId)
            .amount(0.001)
            .build();

        var response = await _walletService.Charge(charge);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        Assert.That(response.GetStringValue(), Is.EqualTo("Amount value must have precision 2 numbers after dot"));
    }

    //49
    [Test]
    public async Task WalletService_ChargeWith0_01Value_StatusCodeIsOk()
    {
        var userId = (await _userServiceClient.RegisterUser(user)).GetId();
        await _userServiceClient.SetUserStatus(userId, true);
        var charge = new ChargeBuilder(new Charge())
            .userId(userId)
            .amount(0.01)
            .build();

        var response = await _walletService.Charge(charge);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    //50
    //Message doesn't correspond to documentation
    //According to documentation, it is not possible to charge negative value
    [Test]
    public async Task WalletService_ChargeWithMinus0_01Value_StatusCodeIsInternalServerErrorWithMessage()
    {
        var userId = (await _userServiceClient.RegisterUser(user)).GetId();
        await _userServiceClient.SetUserStatus(userId, true);
        var charge = new ChargeBuilder(new Charge())
            .userId(userId)
            .amount(-0.01)
            .build();

        var response = await _walletService.Charge(charge);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        Assert.That(response.GetStringValue(), Is.EqualTo("User has '0', you try to charge '-0.01'."));
    }

    //34
    //36
    //39
    [Test]
    [TestCase(0.01)]
    [TestCase(999999.99)]
    [TestCase(10000000)]
    public async Task WalletService_RevertTransactionWithBorderAmount_StatusCodeIsOk(double amount)
    {
        var userId = (await _userServiceClient.RegisterUser(user)).GetId();
        await _userServiceClient.SetUserStatus(userId, true);
        var charge = new ChargeBuilder(new Charge())
            .userId(userId)
            .amount(amount)
            .build();
        var transactionId = (await _walletService.Charge(charge)).GetStringValue();

        var transactionStatusCode = (await _walletService.RevertTransaction(transactionId.Trim('"'))).StatusCode;
        var response = await _walletService.GetBalance(userId);

        Assert.That(transactionStatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response.GetDoubleValue(), Is.EqualTo(0));
    }

    //35
    //According to documentation it is possible to do revert of existing transaction.
    //But transactionId is invalid string,
    //because it is not possible to charge the amount which make overall balance negative
    //37
    //According to documentation it is possible to do revert of existing transaction.
    //But transactionId is invalid string, because it is not possible to charge the value
    //more then max
    [Test]
    [TestCase(-0.01)]
    [TestCase(10000000.01)]
    public async Task WalletService_RevertTransactionAfterIncorrectCharge_StatusCodeIsOk(double amount)
    {
        var userId = (await _userServiceClient.RegisterUser(user)).GetId();
        await _userServiceClient.SetUserStatus(userId, true);
        var charge = new ChargeBuilder(new Charge())
            .userId(userId)
            .amount(amount)
            .build();
        var transactionId = (await _walletService.Charge(charge)).GetStringValue();

        var transactionResponse = await _walletService.RevertTransaction(transactionId.Trim('"'));

        Assert.That(transactionResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    //38
    [Test]
    public async Task WalletService_RevertRevertedTransaction_StatusCodeIsInternalServerErrorWithMessage()
    {
        var userId = (await _userServiceClient.RegisterUser(user)).GetId();
        await _userServiceClient.SetUserStatus(userId, true);
        var charge = new ChargeBuilder(new Charge())
            .userId(userId)
            .amount(12.5)
            .build();
        var transactionId = (await _walletService.Charge(charge)).GetStringValue();

        await _walletService.RevertTransaction(transactionId.Trim('"'));
        var transactionResponse = await _walletService.RevertTransaction(transactionId.Trim('"'));

        Assert.That(transactionResponse.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        Assert.That(transactionResponse.GetStringValue(),
            Is.EqualTo($"Transaction '{transactionId.Trim('"')}'" +
                       $" cannot be reversed due to 'Reverted' current status"));
    }

    //33
    [Test]
    public async Task WalletService_RevertTransactionWithWrongId_StatusCodeIsNotFoundWithMessage()
    {
        var transactionId = new Guid().ToString();

        var transactionResponse = await _walletService.RevertTransaction(transactionId);

        Assert.That(transactionResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        Assert.That(transactionResponse.GetStringValue(),
            Is.EqualTo("The given key was not present in the dictionary."));
    }
}