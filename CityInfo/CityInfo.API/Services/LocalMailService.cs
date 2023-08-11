namespace CityInfo.API.Services
{
	public class LocalMailService : IMailService
	{
		private readonly string mailTo = string.Empty;
		private readonly string mailFrom = string.Empty;

		public LocalMailService(IConfiguration configuration)
		{
			this.mailTo = configuration["mailSettings:mailToAddress"];
			this.mailFrom = configuration["mailSettings:mailFromAddress"];
		}

		public void Send(string subject, string message)
		{
			// Send mail -> Writing to console window
			Console.WriteLine($"Mail from {this.mailFrom} to {this.mailTo}, " + $"with {nameof(LocalMailService)}.");
			Console.WriteLine($"Subject: {subject}");
			Console.WriteLine($"Message: {message}");
		}
	}
}
