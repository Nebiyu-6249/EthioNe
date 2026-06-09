using System;
using EthioNegarit.Utilities;

namespace EthioNegarit.Core.Domain.Entities
{
    public partial class Attachment : Entity
    {
        public Attachment()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public string Filename { get; set; }
        public virtual MembershipUser MembershipUser { get; set; }
        public virtual PrivateMessage PrivateMessage { get; set; }
        public DateTime DateCreated { get; set; }
        public string ContentType { get; set; }
        /*
           public string FileType { get; set; }
           public string ContentType { get; set; }
           public byte[] Data { get; set; }
        */

        public string FriendlyFilename
        {
            get { return Filename.Split('_')[1]; }
        }
        public string FilePath
        {
            get { return string.Format("~/content/uploads/{0}/{1}", MembershipUser.Id, Filename); }
        }
    }
}
