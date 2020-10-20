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
			_smtpClient = new SmtpClient(appSettings.Application.EmailSender.Host, appSettings.Application.EmailSender.Port)
			{
				Credentials = new NetworkCredential(appSettings.Application.EmailSender.Login, appSettings.Application.EmailSender.Password),
				EnableSsl = true,
			};
		}

		public async Task<string> SendStatisticsAsync(WaterCounterInfo hotWaterInfo, WaterCounterInfo coldWaterInfo)
		{
			var template = await File.ReadAllTextAsync(_appSettings.Application.EmailSender.TemplatePath);
			var letterBody = template
				.Replace("%hot_water_counter_value%", (hotWaterInfo.Value / 1000D).ToString(CultureInfo.CurrentCulture))
				.Replace("%cold_water_counter_value%", (coldWaterInfo.Value / 1000D).ToString(CultureInfo.CurrentCulture))
				.Replace("%date_data_valid_on%", DateTime.Now.ToString("D", CultureInfo.GetCultureInfo("RU-ru")));

			var message = new MailMessage()
			{
				Body =  letterBody,
				From = new MailAddress(_appSettings.Application.EmailSender.From),
				Subject =  _appSettings.Application.EmailSender.Subject
			};

			SetMailAddresses(message.To, _appSettings.Application.EmailSender.To);
			SetMailAddresses(message.Bcc, _appSettings.Application.EmailSender.Bcc);
			
			await _smtpClient.SendMailAsync(message);

			return letterBody;
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
