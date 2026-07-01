using EthioNegarit.Core.Domain.Attributes;
using EthioNegarit.Core.Domain.Enums;
using EthioNegarit.Core.Domain.General;
using EthioNegarit.Utilities;
using System;
using System.Collections.Generic;

namespace EthioNegarit.Core.Domain.Entities
{
    public partial class Advertisement : Entity
    {
        public Advertisement()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }  
        public string Name { get; set; }
        public string MotoSlogan { get; set; }
        public string About { get; set; }
        public string Line { get; set; }
        public string Email { get; set; }      
        public string Address { get; set; }    
        public string LinkUrl { get; set; }
        public string LinkText { get; set; }
        public int? PostCount { get; set; }       
        public string Slug { get; set; }
        public string Country { get; set; }
        public string Image { get; set; }
        public bool EnableReminder { get; set; }
        public DateTime? ReminderLastSent { get; set; }
        public MembershipUser User { get; set; }      
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdate { get; set; }
        public DateTime DateLastActive { get; set; }
        public int Month { get; set; }
        public virtual Package Package { get; set; }
        public AdStatus AdStatus { get; set; }
        public virtual IList<Payment> Payments { get; set; }
        public virtual IList<AdvertisementFile> Files { get; set; }
        public virtual IList<Location> Locations { get; set; }
     //   public DateTime? NextShowTime { get; set; }
     //   public int TimeSpan { get; set; }
     //   public AdLocation AdLocation { get; set; }
        public string NiceUrl => UrlTypes.GenerateUrl(UrlType.Advertisement, Slug);
    }
}
/*
     a notice or announcement in a public medium promoting a product, service, or event or publicizing a job vacancy.
     */
