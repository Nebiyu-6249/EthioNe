using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EthioNegarit.WebUI.ViewModels
{
 public class CreateEditPackageOrderViewModel
    {
        public Package AdPackage { get; set; }
        [HiddenInput]
        public Guid PackageId { get; set; }
        public Advertisement Advertisement { get; set; }
        public MembershipUser User { get; set; }
        [Required]
        [DisplayName("Amount Transfer")]
        public double Amount { get; set; }
        [Required]
        [DisplayName("Date Transfer")]
        public DateTime Date { get; set; }
        public string Payment_Mode { get; set; }
        public Bank Bank { get; set; }
        public string Reference { get; set; }
        [DisplayName("Date Transfer")]
        public string Comment { get; set; }
    }
   
  /*  public class GetPackageViewModel
    {
        public IEnumerable<Package> AdPackages { get; set; }
        public MembershipUser User { get; set; }
        public Advertisement Advertisement { get; set; }
        public bool UserCanCreateAd { get; set; }
    }*/
    public class PackagesListViewModel
    {
        public IEnumerable<Package> AdPackages { get; set; }
		public Guid AdvertId{ get; set; }
        public int Month { get; set; }

    }
}
