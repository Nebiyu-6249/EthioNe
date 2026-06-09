using System;
using System.Collections.Generic;
using EthioNegarit.Core.Constants;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Domain.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EthioNegarit.WebUI.Areas.Admin.ViewModels
{
    public class EditPrivacyPolicyViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }
        [DisplayName("Last UpDate")]
        public string LastUpdate { get; set; }
        [DisplayName("Effective Date")]
        public DateTime EffectiveDate { get; set; }      
        [DisplayName("Version")]
        public string Version { get; set; }
        [DisplayName("Terms and Conditions")]
        [UIHint(AppConstants.EditorType), AllowHtml]
        public string Terms { get; set; }
        [DisplayName("Terms and Conditions Amharic")]
        [UIHint(AppConstants.EditorType), AllowHtml]
        public string TermsAm { get; set; }
        [DisplayName("Privacy Policy")]
        [UIHint(AppConstants.EditorType), AllowHtml]
        public string Policy { get; set; }
        [DisplayName("Privacy Policy Amharic")]
        [UIHint(AppConstants.EditorType), AllowHtml]
        public string PolicyAm { get; set; }

    }
}