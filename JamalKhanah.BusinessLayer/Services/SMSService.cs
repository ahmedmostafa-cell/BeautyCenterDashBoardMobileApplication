using JamalKhanah.Core.Helpers;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace JamalKhanah.BusinessLayer.Services;

public static class SmsService
{
    private const string ApiUrl = "https://www.mora-sa.com/api/v1/sendsms?";
    private const string ApiKey = "843a1728cc7b048fb8914f05ef22164cfa99b6fb";
    private const string Username = "966555139936";
    private const string Sender = "khanatkm";
    private const string Url ="{0}api_key={1}&username={2}&message={3}&sender={4}&numbers={5}&response=Json";

    public static async Task<string> SendMessage( string phoneNumber, string message, string phoneCountryCode = "966+")
    {
        if (phoneNumber.StartsWith("+"))
        {
            phoneNumber = phoneNumber.Remove(0, 1);
        }


        if (string.IsNullOrEmpty(phoneCountryCode) || string.IsNullOrEmpty(phoneNumber) ||
            string.IsNullOrEmpty(message)) return null;
        try
        {
            var endPoint = string.Format(Url,ApiUrl,ApiKey,Username, message,Sender, phoneNumber);

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // var bodyJS = JsonConvert.SerializeObject(new PaymentStatusDto());
            var body = new StringContent("SMS", Encoding.UTF8, "application/json");
            var response = client.PostAsync(endPoint, body).GetAwaiter().GetResult();
            var x = await response.Content.ReadAsStringAsync();

            var smsResponse = JsonConvert.DeserializeObject<SmsResponse>(x);
            if (smsResponse.Status.code==200 && smsResponse.Data.code == 100)
            {
                return "تم ارسال الرسالة بنجاح";
            }
            var statusMassage = smsResponse.Status.message;
            var smsMassage = smsResponse.Data.message;
            return null;
        }
        catch (Exception)
        {
            return null;
            //  return "برجاء المحاوله مره اخرى";
        }
    }

    public static async Task<string> SendMessageToMultipleUsers(string phoneCountryCode, List<string> phoneNumbers, string message)
    {
        for (var i = 0; i < phoneNumbers.Count; ++i)
        {
            if (phoneNumbers[i].StartsWith("+"))
            {
                phoneNumbers[i] = phoneNumbers[i].Remove(0, 1);
            }
           
        }
        if (!string.IsNullOrEmpty(phoneCountryCode) && phoneNumbers.Any() && !string.IsNullOrEmpty(message))
        {
            var mobiles = string.Join(",", phoneNumbers);
            var endPoint = string.Format(Url,ApiUrl,ApiKey,Username, message,Sender, mobiles);

            using var client = new HttpClient();
            try
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                // var bodyJS = JsonConvert.SerializeObject(new PaymentStatusDto());
                var body = new StringContent("aaaa", Encoding.UTF8, "application/json");
                var response = client.PostAsync(endPoint, body).GetAwaiter().GetResult();
                var x = await response.Content.ReadAsStringAsync();
                var smsResponse = JsonConvert.DeserializeObject<SmsResponse>(x);
                if (smsResponse.Status.code==200 && smsResponse.Data.code == 100)
                {
                    return "تم ارسال الرسالة بنجاح";
                }
                else
                {
                    return "عفواً ، برجاء المحاوله لاحقاً ، فى حاله تكرر المشكلة برجاء التواصل مع التقنى.";
                }
            }
            catch
            {
                return "عفواً ، برجاء المحاوله لاحقاً ، فى حاله تكرر المشكلة برجاء التواصل مع التقنى.";
            }
        }
        else
        {
            return "برجاء التأكد من صحة المدخلات";
        }
    }
}