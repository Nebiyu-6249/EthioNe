

namespace EthioNegarit.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Constants;
    using Core.Domain.Entities;
    using Core.Domain.General;
    using Core.Interfaces;
    using Core.Interfaces.Services;
    using Data.Context;
    using Utilities;
    using System.Web.Hosting;
    using System.Net.Mail;
    using System.Net;
    using System.IO;

    public partial class EmailService : IEmailService
    {
        
              private readonly ILoggingService _loggingService;
        private readonly ISettingsService _settingsService;
        private readonly EthioNegaritContext _context;

        public EmailService(ILoggingService loggingService, ISettingsService settingsService, IEthioNegaritContext context)
        {
            _loggingService = loggingService;
            _settingsService = settingsService;
            _context = context as EthioNegaritContext;
        }

        public Email Add(Email email)
        {
            return _context.Email.Add(email);
        }

        public void Delete(Email email)
        {
            _context.Email.Remove(email);
        }

        public List<Email> GetAll(int amountToTake)
        {
            return _context.Email.OrderBy(x => x.DateCreated).Take(amountToTake).ToList();
        }

        public void ProcessMail(int amountToSend)
        {
            try
            {
                // Get the amount of emails to send in this batch
                var emails = GetAll(amountToSend);

                // See if there are any
                if (emails != null && emails.Any())
                {
                    // Get the mails settings
                    var settings = _settingsService.GetSettings(false);
                    var smtp = settings.SMTP;
                    var smtpUsername = settings.SMTPUsername;
                    var smtpPassword = settings.SMTPPassword;
                    var smtpPort = settings.SMTPPort;
                    var smtpEnableSsl = settings.SMTPEnableSSL;
                    var fromEmail = settings.NotificationReplyEmail;

                    // Fall back to Web.config <appSettings> when the DB has no SMTP host, so the
                    // owner can keep SMTP credentials in the SERVER Web.config (excluded from FTP,
                    // never committed) instead of the database. DB settings win when present.
                    if (string.IsNullOrEmpty(smtp))
                    {
                        smtp = ConfigUtils.GetAppSetting("Smtp:Host", string.Empty);
                        smtpUsername = ConfigUtils.GetAppSetting("Smtp:Username", string.Empty);
                        smtpPassword = ConfigUtils.GetAppSetting("Smtp:Password", string.Empty);
                        smtpPort = ConfigUtils.GetAppSetting("Smtp:Port", string.Empty);
                        var sslConfig = ConfigUtils.GetAppSetting("Smtp:EnableSsl", string.Empty);
                        bool sslParsed;
                        if (!string.IsNullOrEmpty(sslConfig) && bool.TryParse(sslConfig, out sslParsed))
                        {
                            smtpEnableSsl = sslParsed;
                        }
                        var fromConfig = ConfigUtils.GetAppSetting("Smtp:From", string.Empty);
                        if (!string.IsNullOrEmpty(fromConfig))
                        {
                            fromEmail = fromConfig;
                        }
                    }

                    // No SMTP host configured anywhere - nothing to do. (Not logged: this runs
                    // every few seconds and would flood the log.)
                    if (string.IsNullOrEmpty(smtp) || string.IsNullOrEmpty(fromEmail))
                    {
                        return;
                    }

                    // Set up the SMTP Client object and settings
                    var mySmtpClient = new SmtpClient(smtp);
                    if (!string.IsNullOrEmpty(smtpUsername) && !string.IsNullOrEmpty(smtpPassword))
                    {
                        mySmtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    }

                    if (smtpEnableSsl != null)
                    {
                        mySmtpClient.EnableSsl = (bool)smtpEnableSsl;
                    }

                    if (!string.IsNullOrEmpty(smtpPort))
                    {
                        mySmtpClient.Port = Convert.ToInt32(smtpPort);
                    }

                    // List to store the emails to delete after they are sent
                    var emailsToDelete = new List<Email>();

                    // Loop through email email create a mailmessage and send it
                    foreach (var message in emails)
                    {
                        // Isolate each send: one bad address or transient failure must not
                        // abort the whole batch. Only successfully-sent mails are dequeued;
                        // failures are logged and left for the next cycle to retry.
                        try
                        {
                            var msg = new MailMessage
                            {
                                IsBodyHtml = true,
                                Body = message.Body,
                                From = new MailAddress(fromEmail),
                                Subject = message.Subject
                            };
                            msg.To.Add(message.EmailTo);
                            mySmtpClient.Send(msg);

                            emailsToDelete.Add(message);
                        }
                        catch (Exception exSend)
                        {
                            _loggingService.Error(exSend);
                        }
                    }

                    // Loop through the sent emails and delete them
                    foreach (var sentEmail in emailsToDelete)
                    {
                        Delete(sentEmail);
                    }
                   
                }
            }
            catch (Exception ex)
            {
                _loggingService.Error(ex);
            }
        }

        /// <summary>
        /// Returns the HTML email template with values replaced
        /// </summary>
        /// <param name="to"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public string EmailTemplate(string to, string content)
        {
            var settings = _settingsService.GetSettings();
            return EmailTemplate(to, content, settings);
        }

        public string EmailTemplate(string to, string content, Settings settings)
        {
            using (var sr = File.OpenText(HostingEnvironment.MapPath(@"~/Content/Emails/EmailNotification.htm")))
            {
                var sb = sr.ReadToEnd();
                sr.Close();
                sb = sb.Replace("#CONTENT#", content);
                sb = sb.Replace("#SITENAME#", settings.ForumName);
                sb = sb.Replace("#SITEURL#", settings.ForumUrl);
                if (!string.IsNullOrEmpty(to))
                {
                    to = $"<p>{to},</p>";
                    sb = sb.Replace("#TO#", to);
                }

                return sb;
            }
        }

        public void SendMail(Email email, Settings settings)
        {
            SendMail(new List<Email> { email }, settings);
        }

        /// <summary>
        /// Send a single email
        /// </summary>
        /// <param name="email"></param>
        public void SendMail(Email email)
        {
            SendMail(new List<Email> { email });
        }

        /// <summary>
        /// Send multiple emails
        /// </summary>
        /// <param name="emails"></param>
        public void SendMail(List<Email> emails)
        {
            var settings = _settingsService.GetSettings();
            SendMail(emails, settings);
        }

        public void SendMail(List<Email> emails, Settings settings)
        {
            // Add all the emails to the email table
            // They are sent every X seconds by the email sending task
            foreach (var email in emails)
            {

                // Sort local images in emails
                email.Body = StringUtils.AppendDomainToImageUrlInHtml(email.Body, settings.ForumUrl.TrimEnd('/'));
                Add(email);
            }
        }
        
    }
}
