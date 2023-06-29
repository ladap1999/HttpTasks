using WalletService.Models.Request;

namespace WalletService.Builders;

public class ChargeBuilder
{
    private Charge _chargeRequestModel { get; }

    public ChargeBuilder(Charge charge)
    {
        _chargeRequestModel = charge;
    }

    public ChargeBuilder UserId(int id)
    {
        _chargeRequestModel.UserId = id;
        return this;
    }

    public ChargeBuilder Amount(double amount)
    {
        _chargeRequestModel.Amount = amount;
        return this;
    }

    public Charge Build()
    {
        return _chargeRequestModel;
    }
}