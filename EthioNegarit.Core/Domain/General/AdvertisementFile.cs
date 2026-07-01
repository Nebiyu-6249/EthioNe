using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Utilities;
using System;

namespace EthioNegarit.Core.Domain.General
{
  public partial class AdvertisementFile
    {
        public AdvertisementFile()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public string Filename { get; set; }     
       public string FileType { get; set; }
       public string ContentType { get; set; }
       public byte[] Data { get; set; }
        public virtual MembershipUser MembershipUser { get; set; }
        public virtual Advertisement Advertisement { get; set; }
        public DateTime DateCreated { get; set; }

        public string FriendlyFilename
        {
            get { return Filename.Split('_')[1]; }
        }
        public string FilePath
        {
            get { return string.Format("~/content/uploads/{0}/{1}", Advertisement.Id, Filename); }
        }
    }
}
