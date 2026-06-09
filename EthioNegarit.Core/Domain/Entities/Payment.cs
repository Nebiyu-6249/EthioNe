using System;
using EthioNegarit.Utilities;
using EthioNegarit.Core.Domain.Enums;
namespace EthioNegarit.Core.Domain.Entities
{
    public partial class Payment : Entity
    {
        public Payment()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public int Month { get; set; }
        public string Reference { get; set; }       
        public double Amount { get; set; }
        public double TotalPrice { get; set; }
        public DateTime DateTransfer { get; set; }
        public DateTime DateSubmitted { get; set; }
        public DateTime? DateEdited { get; set; }
        public Bank Bank { get; set; }
        public string PaymentMode { get; set; }
        public DateTime DateConfirmed { get; set; }
        public Guid? ConfirmedBy { get; set; }
        public string Comment { get; set; }
        public Advertisement Advertisement { get; set; }
        public Package Package { get; set; }
        public MembershipUser User { get; set; }
        //User Account Number we use InCase of Refund Request
        public string AccountNo { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        //   public double GrossCost { get; set; }
        public double Discount { get; set; }
        public int InvoiceNo { get; set; }
        //   public double TotalCost { get; set; }
    }
}
