namespace EthioNegarit.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Entity;
    using Core.Domain.Entities;
    using Core.Interfaces;
    using Core.Interfaces.Services;
    using Data.Context;
    using Core.Constants;
    using Core.Domain.Enums;
    using Core.Domain.General;

    public partial class PaymentService : IPaymentService
    {     
        private readonly ILocalizationService _localizationService;
        private readonly ILoggingService _loggingService;
        private readonly IReflectionService _reflectionService;
        private readonly EthioNegaritContext _context;
        private readonly ICacheService _cacheService;
        public PaymentService(ILoggingService loggingService, ILocalizationService localizationService,
         IReflectionService reflectionService, IEthioNegaritContext context, ICacheService cacheService)
        {
            _loggingService = loggingService;
            _localizationService = localizationService;
            _reflectionService = reflectionService;
            _cacheService = cacheService;
            _context = context as EthioNegaritContext;
        }
     
        public PagedList<Payment> GetPayments(int pageIndex, int pageSize, int amountToTake, PaymentViewOrder view)
        {

            var cacheKey = string.Concat(CacheKeys.Payment.StartsWith, "GetPayments-", pageIndex, "-", pageSize, "-", amountToTake, "-", view);
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                // We might only want to display the top 100
                // but there might not be 100 topics
                //    var total = _context.Payment.AsNoTracking().Count(x => x.Pending != true);
                var total = _context.Payment.AsNoTracking().Count();
                if (amountToTake < total)
                {
                    total = amountToTake;
                }
                // Get the topics using an efficient
                var results = _context.Payment
                                    .AsNoTracking()
                                    .Include(x => x.User)
                                     .Include(x => x.Advertisement)
                                      .Include(x => x.Package);

                // Sort what order the Payments are sorted in
                switch (view)
                {
                    case PaymentViewOrder.All:
                        //  results = results.OrderByDescending(x => x.PaymentStatus == PaymentStatus.Pending).ThenByDescending(x => x.DateSubmitted);
                      //  results = results.Where(x => x.PaymentStatus == PaymentStatus.Pending).OrderByDescending(x => x.DateSubmitted);
                        results = results.OrderByDescending(x => x.PaymentStatus == PaymentStatus.Pending).ThenByDescending(x => x.DateSubmitted);
                        break;
                    case PaymentViewOrder.Pending:
                      //  results = results.OrderByDescending(x => x.PaymentStatus == PaymentStatus.Pending).ThenByDescending(x => x.DateSubmitted);
                        results = results.Where(x => x.PaymentStatus == PaymentStatus.Pending).OrderByDescending(x => x.DateSubmitted);

                        break;

                    case PaymentViewOrder.Confirmed:
                        results = results.Where(x => x.PaymentStatus == PaymentStatus.Confirmed).OrderByDescending(x => x.DateSubmitted);

                      //  results = results.OrderByDescending(x => x.PaymentStatus == PaymentStatus.Confirmed).ThenByDescending(x => x.DateSubmitted);
                        break;
                    case PaymentViewOrder.Rejected:
                        results = results.Where(x => x.PaymentStatus == PaymentStatus.Rejected).OrderByDescending(x => x.DateSubmitted);

                      //  results = results.OrderByDescending(x => x.PaymentStatus == PaymentStatus.Rejected).ThenByDescending(x => x.DateSubmitted);

                        break;

                    default:

                        results = results.OrderByDescending(x => x.PaymentStatus == PaymentStatus.Pending).ThenByDescending(x => x.DateSubmitted);

                        break;
                }
                // sort the paging out
                var payments = results.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                // Return a paged list
                return new PagedList<Payment>(payments, pageIndex, pageSize, total);

            });
        }
        public Payment Add(Payment payment)
        {
            payment.DateSubmitted = DateTime.UtcNow;
            payment.DateEdited = DateTime.UtcNow;
            return _context.Payment.Add(payment);
        }
         public void Delete(Payment payment)
         {
             _context.Payment.Remove(payment);
         }

        public Payment Get(Guid paymentId)
         {
             var payment = _context.Payment

                             .Include(x => x.User)
                             .Include(x => x.Package)
                             .Include(x => x.Advertisement)
                             .FirstOrDefault(x => x.Id == paymentId);

             return payment;
         }

        public Payment GetPayment(Guid packageId, Guid advertId)
    {
        //  var cacheKey = string.Concat(CacheKeys.Advertisement.StartsWith, "Get-", advertId);
        // return _cacheService.CachePerRequest(cacheKey, () =>
        //  {
        var payment = _context.Payment

                      //  .Include(x => x.User)
                      // .Where(x => x.Pending != true && x.DateEnd > date && x.User.Id == user.Id)
                      .Where(x => x.PaymentStatus == PaymentStatus.Pending)
                       .FirstOrDefault(x => x.Package.Id == packageId && x.Advertisement.Id == advertId);

        return payment;
        // });
    }
        public int GetPendingPaymentsCount()
        {
            var cacheKey = string.Concat(CacheKeys.Payment.StartsWith, "GetPendingPaymentsCount-");
            return _cacheService.CachePerRequest(cacheKey, () =>
            {

                return _context.Payment.AsNoTracking().Count(x => x.PaymentStatus == PaymentStatus.Pending);
            });

        }

        public PagedList<Payment> GetMemberPayments(int pageIndex, int pageSize, int amountToTake, MembershipUser user)
        {

            var cacheKey = string.Concat(CacheKeys.Payment.StartsWith, "GetMemberPayments-", pageIndex, "-", pageSize, "-", amountToTake, "-", user);
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                // We might only want to display the top 100
                // but there might not be 100 payments
                //    var total = _context.Payment.AsNoTracking().Count(x => x.Pending != true);
                var total = _context.Payment.AsNoTracking().Count();
                if (amountToTake < total)
                {
                    total = amountToTake;
                }
                // Get the Payments using an efficient
                var results = _context.Payment
                                    .AsNoTracking()
                                    .Include(x => x.User)
                                     .Include(x => x.Advertisement)
                                      .Include(x => x.Package)
                                         .Where(x => x.User.Id == user.Id);
                                        // .Skip((pageIndex - 1) * pageSize)
                              //  .Take(pageSize);                                      
                //   .Where(x => x.PaymentStatus == PaymentStatus.Pending && x.User.Id == user.Id);
          //      results = results.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                // Return a paged list
                return new PagedList<Payment>(results, pageIndex, pageSize, total);

            });
        }
        public IList<Payment> GetPaymentByReference(string referenceId)
        {
            return _context.Payment
                  .Include(x => x.Advertisement)
                  .Include(x => x.Package)
                  .Include(x => x.User)
                  .Where(x => x.Reference == referenceId)
                //  .Where(x => x.IsSolution && x.Pending != true)
                 // .OrderByDescending(x => x.DateCreated)
                  .ToList();
        }
        public Payment GetPaymentByReference(string reference, Guid userId)
        {
            var payment = _context.Payment
                          //  .Include(x => x.User)
                          // .Where(x => x.Pending != true && x.DateEnd > date && x.User.Id == user.Id)
                         // .Where(x => x.PaymentStatus == PaymentStatus.Pending)
                           .FirstOrDefault(x => x.Reference == reference && x.User.Id == userId);
            return payment;
            // });
        }
        public int GetConfirmedPaymentsCount()
        {
            var cacheKey = string.Concat(CacheKeys.Payment.StartsWith, "GetConfirmedPaymentsCount-");
            return _cacheService.CachePerRequest(cacheKey, () =>
            {

                return _context.Payment.AsNoTracking().Count(x => x.PaymentStatus == PaymentStatus.Confirmed);
            });

        }
    }
}
