using System;
using EthioNegarit.Core.Domain.Attributes;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Interfaces.Badges;
using EthioNegarit.Core.Interfaces.Services;
using System.Web.Mvc;

namespace EthioNegarit.Badges
{
    [Id("098bbaf1-7de4-43c3-95e3-b15d013aa9c2")]
    [Name("MonthLeader")]
    [DisplayName("Badge.MonthLeader.Name")]
    [Description("Badge.MonthLeader.Desc")]
    [Image("MemberForAYearBadge.png")]
    [AwardsPoints(10)]
    public class MonthLeaderBadge : IPointBadge
    {
        private readonly IMembershipUserPointsService _membershipUserPointsService;

        public MonthLeaderBadge()
        {
            _membershipUserPointsService = DependencyResolver.Current.GetService<IMembershipUserPointsService>();
        }

        public bool Rule(MembershipUser user)
        {
            var today = DateTime.Today;
            var month = new DateTime(today.Year, today.Month, 1);
            var monthFirst = month.AddMonths(-1);
            var monthLast = month.AddDays(-1);

            var userMonthPoint = _membershipUserPointsService.GetUserPoint(user, monthFirst, monthLast);
            return _membershipUserPointsService.CheckHighestPoint(monthFirst, monthLast, userMonthPoint);

        }

        }
}



/*
 DateTime now = DateTime.Now;
int prevMonth = now.AddMonths(-1).Month;
int year = now.AddMonths(-1).Year;
int daysInPrevMonth = DateTime.DaysInMonth(year, prevMonth);
DateTime firstDayPrevMonth = new DateTime(year, prevMonth, 1);
DateTime lastDayPrevMonth = new DateTime(year, prevMonth, daysInPrevMonth);
Console.WriteLine("{0} {1}", firstDayPrevMonth.ToShortDateString(),
  lastDayPrevMonth.ToShortDateString());*/
