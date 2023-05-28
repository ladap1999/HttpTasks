namespace WalletService.Utils;

public class WalletServiceEndpoints
{
    public static readonly string baseURL = "https://walletservice-uat.azurewebsites.net";
    public static readonly string get_balance = $"{baseURL}/Balance/GetBalance";
    public static readonly string charge = $"{baseURL}/Balance/Charge";
    public static readonly string revert_transaction = $"{baseURL}/Balance/RevertTransaction";
}