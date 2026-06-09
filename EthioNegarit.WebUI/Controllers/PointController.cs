namespace EthioNegarit.WebUI.Controllers
{
    using Core.Interfaces.Services;
    using Core.Interfaces.UnitOfWork;
    using Core.Constants;
    using System;
    using System.Web.Mvc;
    using ViewModels;
    using Core.Domain.Enums;
    using System.Globalization;
    using System.Collections.Generic;
    using System.Linq;

    public partial class PointController : BaseController
    {
        private readonly IMembershipUserPointsService _membershipUserPointsService;

        public PointController(ILogAuditService logAuditService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService,
            ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService,
            IMembershipUserPointsService membershipUserPointsService, ICacheService cacheService)
            : base(logAuditService, loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService, cacheService)
        {
            _membershipUserPointsService = membershipUserPointsService;
        }
        [ChildActionOnly]
        [OutputCache(Duration = (int)CacheTimes.TwoHours)]
        public PartialViewResult CurrentWeekHighPointUsers()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var highEarners = _membershipUserPointsService.GetCurrentWeeksPoints(10);
                var viewModel = new HighEarnersPointViewModel { HighEarners = highEarners };
                return PartialView(viewModel);
            }
        }

        [Authorize]
        public ActionResult Index(int? y)
        {
            var min = 0;
            var max = 100;
          //  var thisYear = DateTime.UtcNow.Year;
            var year = y ?? DateTime.UtcNow.Year;

            //  string currentYear = year.ToString();
            var january = GetDateFromString(string.Concat(AppConstants.January, year.ToString()));

            var february = GetDateFromString(string.Concat(AppConstants.February, year.ToString()));
            var march = GetDateFromString(string.Concat(AppConstants.March, year.ToString()));
            var april = GetDateFromString(string.Concat(AppConstants.April, year.ToString()));
            var may = GetDateFromString(string.Concat(AppConstants.May, year.ToString()));
            var june = GetDateFromString(string.Concat(AppConstants.June, year.ToString()));
            var july = GetDateFromString(string.Concat(AppConstants.July, year.ToString()));
            var august = GetDateFromString(string.Concat(AppConstants.August, year.ToString()));
            var september = GetDateFromString(string.Concat(AppConstants.September, year.ToString()));
            var october = GetDateFromString(string.Concat(AppConstants.October, year.ToString()));
            var november = GetDateFromString(string.Concat(AppConstants.November, year.ToString()));
            var december = GetDateFromString(string.Concat(AppConstants.December, year.ToString()));

            var janpoint = 0;
            var febpoint = 0;
            var marpoint = 0;
            var aprilpoint = 0;
            var maypoint = 0;
            var junepoint = 0;
            var julypoint = 0;
            var augpoint = 0;
            var seppoint = 0;
            var octpoint = 0;
            var novpoint = 0;
            var decpoint = 0;
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);

                //Get Points
                janpoint = _membershipUserPointsService.GetUserPoint(loggedOnUser, january, january.AddDays(DateTime.DaysInMonth(year, 1)));

                if (janpoint < min)
                {
                    min = janpoint;
                }
                if (janpoint >= max)
                {
                    max = janpoint;
                }
                febpoint = _membershipUserPointsService.GetUserPoint(loggedOnUser, february, february.AddDays(DateTime.DaysInMonth(year, 2)));
                if (febpoint < min)
                {
                    min = febpoint;
                }
                if (febpoint > max)
                {
                    max = febpoint;
                }
                marpoint = _membershipUserPointsService.GetUserPoint(loggedOnUser, march, march.AddDays(DateTime.DaysInMonth(year, 3)));

                if (marpoint < min)
                {
                    min = marpoint;
                }
                if (marpoint > max)
                {
                    max = marpoint;
                }
                aprilpoint = _membershipUserPointsService.GetUserPoint(loggedOnUser, april, april.AddDays(DateTime.DaysInMonth(year, 4)));

                if (aprilpoint < min)
                {
                    min = aprilpoint;
                }
                if (aprilpoint > max)
                {
                    max = aprilpoint;
                }

                maypoint = _membershipUserPointsService.GetUserPoint(loggedOnUser, may, may.AddDays(DateTime.DaysInMonth(year, 5)));
                if (maypoint < min)
                {
                    min = maypoint;
                }
                if (maypoint > max)
                {
                    max = maypoint;
                }

                junepoint = _membershipUserPointsService.GetUserPoint(loggedOnUser, june, june.AddDays(DateTime.DaysInMonth(year, 6)));

                if (junepoint < min)
                {
                    min = junepoint;
                }
                if (junepoint > max)
                {
                    max = junepoint;
                }

                julypoint = _membershipUserPointsService.GetUserPoint(loggedOnUser, july, july.AddDays(DateTime.DaysInMonth(year, 7)));

                if (julypoint < min)
                {
                    min = julypoint;
                }
                if (julypoint > max)
                {
                    max = julypoint;
                }
                augpoint = _membershipUserPointsService.GetUserPoint(loggedOnUser, august, august.AddDays(DateTime.DaysInMonth(year, 8)));

                if (augpoint < min)
                {
                    min = augpoint;
                }
                if (augpoint > max)
                {
                    max = augpoint;
                }

                seppoint = _membershipUserPointsService.GetUserPoint(loggedOnUser, september, september.AddDays(DateTime.DaysInMonth(year, 9)));

                if (seppoint < min)
                {
                    min = seppoint;
                }
                if (seppoint > max)
                {
                    max = seppoint;
                }

                octpoint = _membershipUserPointsService.GetUserPoint(loggedOnUser, october, october.AddDays(DateTime.DaysInMonth(year, 10)));

                if (octpoint < min)
                {
                    min = octpoint;
                }
                if (octpoint > max)
                {
                    max = octpoint;
                }

                novpoint = _membershipUserPointsService.GetUserPoint(loggedOnUser, november, november.AddDays(DateTime.DaysInMonth(year, 11)));

                if (novpoint < min)
                {
                    min = novpoint;
                }
                if (novpoint > max)
                {
                    max = novpoint;
                }
                decpoint = _membershipUserPointsService.GetUserPoint(loggedOnUser, december, december.AddDays(DateTime.DaysInMonth(year, 12)));
                //  febpoint = febpoint + _membershipUserPointsService.GetUserMonthPoints(loggedOnUser, febStart, febEnd);
                if (decpoint < min)
                {
                    min = decpoint;
                }
                if (decpoint > max)
                {
                    max = decpoint;
                }
            }
            // var myChartModel = new ChartModel();
            ChartViewModel myChartModel = new ChartViewModel();

            myChartModel.pointchart_ds1_point_J = janpoint.ToString();
            myChartModel.pointchart_ds1_point_F = febpoint.ToString();
            myChartModel.pointchart_ds1_point_M = marpoint.ToString();
            myChartModel.pointchart_ds1_point_A = aprilpoint.ToString();

            myChartModel.pointchart_ds1_point_O = maypoint.ToString();
            myChartModel.pointchart_ds1_point_N = junepoint.ToString();
            myChartModel.pointchart_ds1_point_D = julypoint.ToString();
            myChartModel.pointchart_ds1_point_MY = augpoint.ToString();


            myChartModel.pointchart_ds1_point_JU = seppoint.ToString();
            myChartModel.pointchart_ds1_point_JY = octpoint.ToString();
            myChartModel.pointchart_ds1_point_AG = novpoint.ToString();
            myChartModel.pointchart_ds1_point_S = decpoint.ToString();

            myChartModel.YMin = min;
            myChartModel.YMax = max;

            myChartModel.Year = year;

            return PartialView(myChartModel);
        }
        public DateTime GetDateFromString(string dateString)
        {
            string format;
            format = "d";
            DateTime result;

            CultureInfo provider = CultureInfo.InvariantCulture;
            //  provider = new CultureInfo("fr-FR");
            result = DateTime.ParseExact(dateString, format, provider);
            /* try
             {
                 result = DateTime.ParseExact(dateString, format, provider);
                 Console.WriteLine("{0} converts to {1}.", dateString, result.ToString());
             }
             catch (FormatException)
             {
                 Console.WriteLine("{0} is not in the correct format.", dateString);
             }*/
            return result;
        }
        // public static DateTime ParseExact(string s, string format, IFormatProvider? provider);

        [Authorize]
        public PartialViewResult MemberPoint(int? y)
        {
            var min = 0;
            var max = 100;
        //    var thisYear = DateTime.UtcNow.Year;
            var year = y ?? DateTime.UtcNow.Year;
            //  string currentYear = year.ToString();
            var january = GetDateFromString(string.Concat(AppConstants.January, year.ToString()));
            var february = GetDateFromString(string.Concat(AppConstants.February, year.ToString()));
            var march = GetDateFromString(string.Concat(AppConstants.March, year.ToString()));
            var april = GetDateFromString(string.Concat(AppConstants.April, year.ToString()));
            var may = GetDateFromString(string.Concat(AppConstants.May, year.ToString()));
            var june = GetDateFromString(string.Concat(AppConstants.June, year.ToString()));
            var july = GetDateFromString(string.Concat(AppConstants.July, year.ToString()));
            var august = GetDateFromString(string.Concat(AppConstants.August, year.ToString()));
            var september = GetDateFromString(string.Concat(AppConstants.September, year.ToString()));
            var october = GetDateFromString(string.Concat(AppConstants.October, year.ToString()));
            var november = GetDateFromString(string.Concat(AppConstants.November, year.ToString()));
            var december = GetDateFromString(string.Concat(AppConstants.December, year.ToString()));

            var janpoint = 0;
            var febpoint = 0;
            var marpoint = 0;
            var aprilpoint = 0;
            var maypoint = 0;
            var junepoint = 0;
            var julypoint = 0;
            var augpoint = 0;
            var seppoint = 0;
            var octpoint = 0;
            var novpoint = 0;
            var decpoint = 0;
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);

                //Get Points
                janpoint = _membershipUserPointsService.GetUserPoint(loggedOnUser, january, january.AddDays(DateTime.DaysInMonth(year, 1)));

                if (janpoint < min)
                {
                    min = janpoint;
                }
                if (janpoint >= max)
                {
                    max = janpoint;
                }
                febpoint = _membershipUserPointsService.GetUserPoint(loggedOnUser, february, february.AddDays(DateTime.DaysInMonth(year, 2)));
                if (febpoint < min)
                {
                    min = febpoint;
                }
                if (febpoint > max)
                {
                    max = febpoint;
                }
                marpoint = _membershipUserPointsService.GetUserPoint(loggedOnUser, march, march.AddDays(DateTime.DaysInMonth(year, 3)));

                if (marpoint < min)
                {
                    min = marpoint;
                }
                if (marpoint > max)
                {
                    max = marpoint;
                }
                aprilpoint = _membershipUserPointsService.GetUserPoint(loggedOnUser, april, april.AddDays(DateTime.DaysInMonth(year, 4)));

                if (aprilpoint < min)
                {
                    min = aprilpoint;
                }
                if (aprilpoint > max)
                {
                    max = aprilpoint;
                }

                maypoint = _membershipUserPointsService.GetUserPoint(loggedOnUser, may, may.AddDays(DateTime.DaysInMonth(year, 5)));
                if (maypoint < min)
                {
                    min = maypoint;
                }
                if (maypoint > max)
                {
                    max = maypoint;
                }

                junepoint = _membershipUserPointsService.GetUserPoint(loggedOnUser, june, june.AddDays(DateTime.DaysInMonth(year, 6)));

                if (junepoint < min)
                {
                    min = junepoint;
                }
                if (junepoint > max)
                {
                    max = junepoint;
                }

                julypoint = _membershipUserPointsService.GetUserPoint(loggedOnUser, july, july.AddDays(DateTime.DaysInMonth(year, 7)));

                if (julypoint < min)
                {
                    min = julypoint;
                }
                if (julypoint > max)
                {
                    max = julypoint;
                }
                augpoint = _membershipUserPointsService.GetUserPoint(loggedOnUser, august, august.AddDays(DateTime.DaysInMonth(year, 8)));

                if (augpoint < min)
                {
                    min = augpoint;
                }
                if (augpoint > max)
                {
                    max = augpoint;
                }

                seppoint = _membershipUserPointsService.GetUserPoint(loggedOnUser, september, september.AddDays(DateTime.DaysInMonth(year, 9)));

                if (seppoint < min)
                {
                    min = seppoint;
                }
                if (seppoint > max)
                {
                    max = seppoint;
                }

                octpoint = _membershipUserPointsService.GetUserPoint(loggedOnUser, october, october.AddDays(DateTime.DaysInMonth(year, 10)));

                if (octpoint < min)
                {
                    min = octpoint;
                }
                if (octpoint > max)
                {
                    max = octpoint;
                }

                novpoint = _membershipUserPointsService.GetUserPoint(loggedOnUser, november, november.AddDays(DateTime.DaysInMonth(year, 11)));

                if (novpoint < min)
                {
                    min = novpoint;
                }
                if (novpoint > max)
                {
                    max = novpoint;
                }
                decpoint = _membershipUserPointsService.GetUserPoint(loggedOnUser, december, december.AddDays(DateTime.DaysInMonth(year, 12)));
                if (decpoint < min)
                {
                    min = decpoint;
                }
                if (decpoint > max)
                {
                    max = decpoint;
                }
            }
            // var myChartModel = new ChartModel();
            ChartViewModel myChartModel = new ChartViewModel();

            myChartModel.pointchart_ds1_point_J = janpoint.ToString();
            myChartModel.pointchart_ds1_point_F = febpoint.ToString();
            myChartModel.pointchart_ds1_point_M = marpoint.ToString();
            myChartModel.pointchart_ds1_point_A = aprilpoint.ToString();

            myChartModel.pointchart_ds1_point_MY = maypoint.ToString();
            myChartModel.pointchart_ds1_point_JU = junepoint.ToString();
            myChartModel.pointchart_ds1_point_JY = julypoint.ToString();
            myChartModel.pointchart_ds1_point_AG = augpoint.ToString();

            myChartModel.pointchart_ds1_point_S = seppoint.ToString();       
            myChartModel.pointchart_ds1_point_O = octpoint.ToString();
            myChartModel.pointchart_ds1_point_N = novpoint.ToString();
            myChartModel.pointchart_ds1_point_D = decpoint.ToString();
          

            myChartModel.YMin = min;
            myChartModel.YMax = max;
            myChartModel.Year = year;
            myChartModel.Years = new SelectList(YearSelection(), "Text", "Value");
            return PartialView(myChartModel);
        }
        public List<SelectListItem> YearSelection()
        {
            var points = _membershipUserPointsService.GetUserPoint(LoggedOnReadOnlyUser);
            var selectyears = points.Select(x => x.DateAdded.Year).Distinct();
            var years = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            foreach (var year in selectyears)
            {
                years.Add(new SelectListItem { Text = year.ToString(), Value = year.ToString() });
            }
            return years;
        }
    }
}
