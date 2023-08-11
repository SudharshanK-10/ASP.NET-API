namespace CityInfo.API.Services
{
	public class CloudMailService : IMailService
	{
		private readonly string mailTo = string.Empty;
		private readonly string mailFrom = string.Empty;

		public CloudMailService(IConfiguration configuration)
		{
			this.mailTo = configuration["mailSettings:mailToAddress"];
			this.mailFrom = configuration["mailSettings:mailFromAddress"];
		}

		public void Send(string subject, string message)
		{
			// Send mail -> Writing to console window
			Console.WriteLine($"Mail from {this.mailFrom} to {this.mailTo}, " + $"with {nameof(CloudMailService)}.");
			Console.WriteLine($"Subject: {subject}");
			Console.WriteLine($"Message: {message}");
		}
	}
}
 