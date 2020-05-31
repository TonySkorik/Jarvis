using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Jarvis.Server.Configuration;
using Jarvis.SstCloud.Core.Model;

namespace Jarvis.Server.Infrastructure
{
	public class EmailSender
	{
		private readonly AppSettings _config;
		private readonly SmtpClient _smtp;

		public EmailSender(AppSettings config)
		{
			_config = config;
			_smtp = new SmtpClient(config.EmailSender.Host, config.EmailSender.Port)
			{
				Credentials = new NetworkCredential(config.EmailSender.Login, config.EmailSender.Password)
			};
		}

		public async Task SendStatisticsAsync(WaterCounterInfo hotWaterInfo, WaterCounterInfo coldWaterInfo)
		{
			var template = await File.ReadAllTextAsync(_config.EmailSender.TemplatePath);
			var letterBody = template
				.Replace("%hot_water_counter_value%", (hotWaterInfo.Value / 1000D).ToString(CultureInfo.CurrentCulture))
				.Replace("%cold_water_counter_value%", (coldWaterInfo.Value / 1000D).ToString(CultureInfo.CurrentCulture))
				.Replace("%date_data_valid_on%", DateTime.Now.ToString("D"));

			var message = new MailMessage()
			{
				Body =  letterBody,
				From = new MailAddress(_config.EmailSender.From),
				Subject =  _config.EmailSender.Subject
			};

			SetMailAddresses(message.To, _config.EmailSender.To);
			SetMailAddresses(message.Bcc, _config.EmailSender.Bcc);
			
			await _smtp.SendMailAsync(message);
		}

		private void SetMailAddresses(
			MailAddressCollection collectionToUpdate,
			IEnumerable<string> addressesToAdd)
		{
			foreach (var address in addressesToAdd)
			{ 
				collectionToUpdate.Add(address);
			}
		}
	}
}
