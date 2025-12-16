namespace BookingCare.Services
{
    public interface IChatbotService
    {
        Task<string> AskAsync(string userId, string message);
    }
}
