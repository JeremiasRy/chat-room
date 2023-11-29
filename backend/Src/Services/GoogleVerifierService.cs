using Google.Apis.Auth;

namespace backend.Src.Services;

public class GoogleVerifierService : IGoogleVerifierService
{
    private readonly IConfiguration _configuration;
    public GoogleVerifierService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task<GoogleJsonWebSignature.Payload?> VerifyTokenAsync(string token)
    {
        var options = new GoogleJsonWebSignature.ValidationSettings()
        {
            Audience = new string[] { _configuration.GetValue<string>("Google:client_id") }
        };
        return await GoogleJsonWebSignature.ValidateAsync(token, options);
    }
}
