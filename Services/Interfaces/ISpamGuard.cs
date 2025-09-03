namespace AnimalesPerdidos.Services.Interfaces
{
    public interface ISpamGuard
    {
        Task<bool> ValidateCaptchaAsync(string token, string ip, CancellationToken ct);
    }
}
