using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Domain.Enums;
using EthioNegarit.Core.Domain.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthioNegarit.Core.Interfaces.Services
{
    public partial interface IPaymentService
    {
        PagedList<Payment> GetPayments(int pageIndex, int pageSize, int amountToTake, PaymentViewOrder order);
        
        Payment GetPayment(Guid packageId, Guid advertId);
        Payment Get(Guid paymentId);
        int GetPendingPaymentsCount();        
        PagedList<Payment> GetMemberPayments(int pageIndex, int pageSize, int amountToTake, MembershipUser user);
      
        Payment Add(Payment payment);
        void Delete(Payment payment);
        IList<Payment> GetPaymentByReference(string referenceId);
       Payment GetPaymentByReference(string reference, Guid userId);
        int GetConfirmedPaymentsCount();
    }
}
