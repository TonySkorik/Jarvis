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
		private readonly AppSettings _appSettings;
		private readonly SmtpClient _smtpClient;

		public EmailSender(AppSettings appSettings)
		{
			_appSettings = appSettings;
			_smtpClient = new SmtpClient(appSettings.MainSettings.EmailSender.Host, appSettings.MainSettings.EmailSender.Port)
			{
				Credentials = new NetworkCredential(appSettings.MainSettings.EmailSender.Login, appSettings.MainSettings.EmailSender.Password)
			};
		}

		public async Task SendStatisticsAsync(WaterCounterInfo hotWaterInfo, WaterCounterInfo coldWaterInfo)
		{
			var template = await File.ReadAllTextAsync(_appSettings.MainSettings.EmailSender.TemplatePath);
			var letterBody = template
				.Replace("%hot_water_counter_value%", (hotWaterInfo.Value / 1000D).ToString(CultureInfo.CurrentCulture))
				.Replace("%cold_water_counter_value%", (coldWaterInfo.Value / 1000D).ToString(CultureInfo.CurrentCulture))
				.Replace("%date_data_valid_on%", DateTime.Now.ToString("D", CultureInfo.GetCultureInfo("RU-ru")));

			var message = new MailMessage()
			{
				Body =  letterBody,
				From = new MailAddress(_appSettings.MainSettings.EmailSender.From),
				Subject =  _appSettings.MainSettings.EmailSender.Subject
			};

			SetMailAddresses(message.To, _appSettings.MainSettings.EmailSender.To);
			SetMailAddresses(message.Bcc, _appSettings.MainSettings.EmailSender.Bcc);
			
			await _smtpClient.SendMailAsync(message);
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
