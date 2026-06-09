using System;
namespace EthioNegarit.Core.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
   public class VideoAttribute : Attribute
    {
        public bool EnableVideo { get; set; }

        public VideoAttribute(bool enableVideo)
        {
            EnableVideo = enableVideo;
        }
    }
}

