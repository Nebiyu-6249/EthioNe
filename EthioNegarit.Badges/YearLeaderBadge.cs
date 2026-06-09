using System;
using EthioNegarit.Core.Domain.Attributes;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Interfaces.Badges;

namespace EthioNegarit.Badges
{
   /* [Id("3255ed94-c82f-4e72-8324-b15d013aa9c6")]
    [Name("YearLeader")]
    [DisplayName("Badge.YearLeader.Name")]
    [Description("Badge.YearLeader.Desc")]
    [Image("MemberForAYearBadge.png")]
    [AwardsPoints(100)]
    */
 public class YearLeaderBadge //: IPointBadge
    {
        /* public bool Rule(MembershipUser user)
         {
             var anniversary = new DateTime(user.CreateDate.Year + 1, user.CreateDate.Month, user.CreateDate.Day);
             return DateTime.UtcNow >= anniversary;
              var points = user.Points.Sum(x => x.Points);
             return points >= 1000;
         }*/
    }
}
