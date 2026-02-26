using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

public class Fast2SMSService
{
    private readonly string apiKey = "IcZlqsmwevpb2NA0WUa1yGXdigH8KYRP6rTxzSh34EVMFDoOkLyaM9LzBfl8qIWPcXHZYn6tQSvAg0ib"; // Replace

    public async Task<string> SendOtpAsync(string mobileNumber, string otp)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("authorization", apiKey);

            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("route", "v3"),
                new KeyValuePair<string, string>("sender_id", "TXTIND"),
                new KeyValuePair<string, string>("message", $"Your OTP is {otp}"),
                new KeyValuePair<string, string>("language", "english"),
                new KeyValuePair<string, string>("numbers", mobileNumber),
            });

            var response = await client.PostAsync("https://www.fast2sms.com/dev/bulkV2", data);
            var result = await response.Content.ReadAsStringAsync();

            return result;
        }
    }
}
