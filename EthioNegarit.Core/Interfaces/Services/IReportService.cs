//using EthioNegaret.Core.Domain.General;

using EthioNegarit.Core.Domain;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Domain.General;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EthioNegarit.Core.Interfaces.Services
    {
    public partial interface IReportService
        {
        void MemberReport (Report report);
        Report Get (Guid reportId);
        IEnumerable<Report> GetByGuid (Guid guid);
        Report Add (Report report);
        void Delete (Report report);
        IEnumerable<Report> GetAll ();
        IList<Report> PostReports (MembershipUser user, Guid postId);
        void UpdateReports ();
        IList<Report> GetRecentReports (int amountToTake, DateTime? from);
        PagedList<Report> GetPostReports(int pageIndex, int pageSize, int amountToTake, Guid postId);
        PagedList<Report> GetAllReports (int pageIndex, int pageSize, int amountToTake);

     //   PagedList<Report> GetReports (int pageIndex, int pageSize, int amountToTake);
        IList<Report> GetReportsOnPost (Guid postId);

        IList<Report> GetReportsOnUser (MembershipUser user, DateTime? from);
        }
    }
