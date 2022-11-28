namespace BookingWorkplace.Core.Abstractions;

public interface ISender
{
    Task SendMessageAsync(string message);
}