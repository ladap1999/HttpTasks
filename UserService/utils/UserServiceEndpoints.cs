namespace UserService.Utils;

public class UserServiceEndpoints
{
    public static readonly string baseURL = "https://userservice-uat.azurewebsites.net";
    
    public static readonly string register_user = $"{baseURL}/Register/RegisterNewUser";
    
    public static readonly string delete_user = $"{baseURL}/Register/DeleteUser";
    
    public static readonly string get_user_status = $"{baseURL}/UserManagement/GetUserStatus";
    
    public static readonly string set_user_status = $"{baseURL}/UserManagement/SetUserStatus";
}