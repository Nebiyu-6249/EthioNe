using Akismet.NET;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Interfaces.Services;
using EthioNegarit.Utilities;
using System;
using System.Linq;
using System.Web;

namespace EthioNegarit.WebUI.Application.Akismet
{
    public class AkismetHelper
    {
        private readonly ISettingsService _settingsService;

        public AkismetHelper(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        /// <summary>
        /// Check whether a post is spam or not.
        /// </summary>
        public bool IsSpam(Post post)
        {
            if (post == null || post.User == null)
            {
                return false;
            }
            return CheckSpam(BuildComment(post.User.UserName, post.User.Email, post.PostContent));
        }

        /// <summary>
        /// Check whether a topic is spam or not.
        /// </summary>
        public bool IsSpam(Topic topic)
        {
            var starter = topic?.Posts?.FirstOrDefault(x => x.IsTopicStarter);
            // No starter post to inspect -> fail open (don't block the topic). Previously
            // this returned true, which blocked otherwise-legitimate topics.
            if (starter == null || topic.User == null)
            {
                return false;
            }
            return CheckSpam(BuildComment(topic.User.UserName, topic.User.Email, starter.PostContent));
        }

        private Comment BuildComment(string author, string authorEmail, string content)
        {
            var request = HttpContext.Current != null ? HttpContext.Current.Request : null;
            return new Comment
            {
                blog = StringUtils.CheckLinkHasHttp(_settingsService.GetSettings().ForumUrl),
                comment_type = "comment",
                comment_author = author,
                comment_author_email = authorEmail,
                comment_content = content,
                permalink = string.Empty,
                referrer = request?.ServerVariables["HTTP_REFERER"],
                user_agent = request?.ServerVariables["HTTP_USER_AGENT"],
                user_ip = request?.ServerVariables["REMOTE_ADDR"]
            };
        }

        /// <summary>
        /// Runs the Akismet check. Gated on the EnableAkisment setting AND a key being present
        /// (DB Settings.AkismentKey, else Web.config appSetting "Akismet:ApiKey"). FAILS OPEN:
        /// if Akismet is unreachable/errors, the content is treated as not-spam and the error is
        /// logged, so a spam-service problem never blocks a genuine post.
        /// </summary>
        private bool CheckSpam(Comment comment)
        {
            var settings = _settingsService.GetSettings();
            if (settings.EnableAkisment != true)
            {
                return false;
            }

            var apiKey = !string.IsNullOrEmpty(settings.AkismentKey)
                ? settings.AkismentKey
                : ConfigUtils.GetAppSetting("Akismet:ApiKey", string.Empty);
            if (string.IsNullOrEmpty(apiKey))
            {
                return false;
            }

            try
            {
                return new Validator(apiKey).IsSpam(comment);
            }
            catch (Exception ex)
            {
                ServiceFactory.Get<ILoggingService>().Error(ex);
                return false;
            }
        }
    }
}
