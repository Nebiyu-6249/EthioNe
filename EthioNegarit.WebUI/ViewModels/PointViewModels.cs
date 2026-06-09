using EthioNegarit.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EthioNegarit.WebUI.ViewModels
{
    public class HighEarnersPointViewModel
    {
        public Dictionary<MembershipUser, int> HighEarners { get; set; }
    }

    public class MemberPointsViewModel
    {
      
      //  public int? PointsGet { get; set; }
       
      //  public int? PointsDeduct { get; set; }
        public IList<MembershipUserPoints> PointsGet { get; set; }
        public IList<MembershipUserPoints> PointsDeduct { get; set; }
        public int? TotalPoint { get; set; }
       
       
        public Topic Topic { get; set; }
    }
}