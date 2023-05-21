using WalletService.models.request;

namespace WalletService.builders;

public class ChargeBuilder
{
    private Charge _chargeRequestModel { get; }

    public ChargeBuilder(Charge charge)
    {
        _chargeRequestModel = charge;
    }

    public ChargeBuilder userId(int id)
    {
        _chargeRequestModel.userId = id;
        return this;
    }

    public ChargeBuilder amount(double amount)
    {
        _chargeRequestModel.amount = amount;
        return this;
    }

    public Charge build()
    {
        return _chargeRequestModel;
    }
}