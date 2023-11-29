using Google.Apis.Auth;

namespace backend.Src.Services
{
    public interface IGoogleVerifierService
    {
        Task<GoogleJsonWebSignature.Payload?> VerifyTokenAsync(string token);
    }
}