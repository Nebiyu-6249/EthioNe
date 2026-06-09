using System;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Utilities;
using System.Collections.Generic;

namespace EthioNegarit.Core.Domain.Entities
{
   /* public enum ReportType
    {
        SpamMessage,
        SpamReplay,
        SpamPost
    }*/
    public enum ReportReason
    {
        ViolentOrRepulsiveContent,
        HatefullOrAbusiveContent,
        Harasement,
        HarmfullOrDangerousAct,
        Missinformation,
        PromotesTerrorism,
        SpamOrMisleading
    }
    public partial class Report : Entity
    {
        public Report()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }

        public string Reason { get; set; }     
        public virtual MembershipUser Reporter { get; set; }
        public virtual MembershipUser ReportedMember { get; set; }
        public virtual Post ReportedPost { get; set; }
        public DateTime DateReport { get; set; } 
    }
}


//  public virtual MembershipUser Reported { get; set; }
// public Guid ReferenceId { get; set; }
//  public ReportType ReportType { get; set; }
// public bool Pending { get; set; }
//  public bool IsConfirmed { get; set; }

//  public DateTime Timestamp { get; set; }

//   public DateTime RemoveAfterDate { get; set; }




/*

  public string Reason { get; set; }
public virtual MembershipUser Reporter { get; set; }
public virtual MembershipUser ReportedMember { get; set; }
public virtual Post ReportedPost { get; set; }*/
