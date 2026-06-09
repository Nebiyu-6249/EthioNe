using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Domain.Enums;
using EthioNegarit.WebUI.Application;

namespace EthioNegarit.WebUI.ViewModels
{
    public class CreateEditPaymentViewModel
    {
        public Package AdPackage { get; set; }
        [HiddenInput]
        public Guid Id { get; set; }
        [HiddenInput]
        public Guid PackageId { get; set; }
        [HiddenInput]
        public Guid AdvertId { get; set; }
        public Advertisement Advertisement { get; set; }
        public MembershipUser User { get; set; }
       
        [ForumResourceDisplayName("Payment.Label.GrossPrice")]
        public double GrossPrice { get; set; }
        [ForumResourceDisplayName("Payment.Label.TotalPrice")]
        public double Discount { get; set; }
        [ForumResourceDisplayName("Payment.Discount")]
        public double TotalPrice { get; set; }
        [Required]
        [ForumResourceDisplayName("Payment.AmountTransferred")]
      //  [System.ComponentModel.DataAnnotations.Compare("TotalCost")]
        public double Amount { get; set; }
     
        [Required]
        [ForumResourceDisplayName("Payment.DateTransferred")]
        public DateTime DateTransfer { get; set; }
        public string Payment_Mode { get; set; }
        [ForumResourceDisplayName("Payment.Bank")]
        public Bank Bank { get; set; }
        [ForumResourceDisplayName("Payment.AccountNo")]
        public string AccountNo { get; set; }
        [Required]
        [ForumResourceDisplayName("Payment.Reference")]
        public string Reference { get; set; }
        public string Comment { get; set; }
        public int Month { get; set; }
    }
    public class PaymentsListViewModel
    {
        public List<Payment> Payments { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int? TotalPages { get; set; }
    }
    public class PaymentConfirmationViewModel
    {
        public Guid Id { get; set; }
    }
    public class PaymentDeleteViewModel
    {
        public Guid Id { get; set; }
    }
    public class PaymentRejectionViewModel
    {
        public Guid Id { get; set; }
    }
    public class MemberPaymentsViewModel
    {
        public IList<Payment> Payments { get; set; }
    }
    public class InvoiceViewModel
    {
        public Package Package { get; set; }
        public Payment Payment { get; set; }

        public MembershipUser User { get; set; }

        public Advertisement Advertisement { get; set; }
    }
    public class RefundRequestViewModel
    {
        [Required]
        [ForumResourceDisplayName("Payment.Reference")]
        public string Reference { get; set; }
        [ForumResourceDisplayName("Payment.Bank")]
        public Bank Bank { get; set; }
        [Required]
        [ForumResourceDisplayName("Payment.DateTransferred")]
        public DateTime DateTransfer { get; set; }
        [ForumResourceDisplayName("Payment.RefundRequest.Reason")]
        public string Reason { get; set; }
    }
}