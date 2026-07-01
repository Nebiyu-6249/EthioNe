using EthioNegarit.Core.Constants;
using EthioNegarit.Core.Domain.Entities;
using EthioNegarit.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EthioNegarit.WebUI.Areas.Admin.ViewModels
{
    public class ListFAQViewModel
    {
        public IEnumerable<FAQ> Faqs { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public string Search { get; set; }
    }

    public class CreateEditFAQViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }
      //  public Language Language { get; set; }
        public string Language { get; set; }
        [DisplayName("Question")]
        [UIHint(AppConstants.EditorType), AllowHtml]
        public string Question { get; set; }
        [DisplayName("Replay")]
        [UIHint(AppConstants.EditorType), AllowHtml]
        public string Replay { get; set; }
        public IEnumerable<SelectListItem> Languages { get; set; }
    }
    public class DeleteFAQViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }
      //  public Help Help { get; set; }
    }
}