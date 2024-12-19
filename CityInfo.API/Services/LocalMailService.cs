namespace CityInfo.API.Services;

public class LocalMailService : IMailService
{
    private string _from = string.Empty;
    private string _to = string.Empty;

    public LocalMailService(IConfiguration configuration)
    {
        _from = configuration["MailService:from"];
        _to = configuration["MailService:to"];
    }
    public void Send(string subject, string message)
    {
        Console.WriteLine($"Email sent from {_from} to {_to} with {nameof(LocalMailService)} \nSubject:{subject}\n msg: {message}");
    }
}
