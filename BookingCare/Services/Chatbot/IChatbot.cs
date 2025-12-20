namespace BookingCare.Services
{
    public interface IChatbot
    {
        Task<string> AskAsync(string userId, string message);
    }
}