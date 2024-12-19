namespace CityInfo.API.Services
{
    public class CloudMailService : IMailService
    {
        private string _from = string.Empty;
        private string _to = string.Empty;

        public CloudMailService(IConfiguration configuration)
        {
            _from = configuration["MailService:from"];
            _to = configuration["MailService:to"];

        }
        public void Send(string subject, string message)
        {
            Console.WriteLine($"Email sent from {_from} to {_to} with {nameof(CloudMailService)} \nSubject:{subject}\n msg: {message}");
        }
    }
}