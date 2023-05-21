namespace UserService.Extensions;

public static class HttpResponseMessageExtension
{
    public static int GetId(this HttpResponseMessage response)
    {
        return Convert.ToInt32(response.Content.ReadAsStringAsync().Result);
    }
    
    public static double GetDoubleValue(this HttpResponseMessage response)
    {
        return Convert.ToDouble(response.Content.ReadAsStringAsync().Result);
    }
    
    public static string GetStringValue(this HttpResponseMessage response)
    {
        return response.Content.ReadAsStringAsync().Result;
    }

    public static bool GetBoolValue(this HttpResponseMessage response)
    {
        return Convert.ToBoolean(response.Content.ReadAsStringAsync().Result);
    }
}