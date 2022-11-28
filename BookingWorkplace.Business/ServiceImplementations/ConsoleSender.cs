using BookingWorkplace.Core.Abstractions;

namespace BookingWorkplace.Business.ServiceImplementations;

/// <summary>
///     The class is a stub.
/// </summary>
public class ConsoleSender : ISender
{
    /// <summary>
    ///     Outputs a message to the console.
    /// </summary>
    /// <remarks>The method returns a job. This is done to save the signature.</remarks>
    /// <param name="message">a message as a string</param>
    /// <returns>The Task</returns>
    public Task SendMessageAsync(string message)
    {
        Console.WriteLine(message);
        return Task.CompletedTask;
    }
}