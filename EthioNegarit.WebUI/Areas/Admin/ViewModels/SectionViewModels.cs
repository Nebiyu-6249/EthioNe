using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EthioNegarit.Core.Constants;
using EthioNegarit.Core.Domain.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using EthioNegarit.Core.Domain.General;
using EthioNegarit.Core.Domain;

namespace EthioNegarit.WebUI.Areas.Admin.ViewModels
{
    public class SectionAddEditViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [DisplayName("Section Name")]
        [Required]
        [StringLength(600)]
        public string Name { get; set; }

        [DisplayName("Section Description")]
        [DataType(DataType.MultilineText)]
        [UIHint(AppConstants.EditorType)]
        [AllowHtml]
        public string Description { get; set; }

        [DisplayName("Sort Order")]
        [Range(0, int.MaxValue)]
        public int SortOrder { get; set; }

        public bool IsEdit { get; set; }
    }
    public class SectionViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }
        [DisplayName("Section Name")]
        [Required]
        [StringLength(600)]
        public string Name { get; set; }
        [DisplayName("Sort Order")]
        [Range(0, int.MaxValue)]
        public int SortOrder { get; set; }
        [DisplayName("Section Description")]
        [DataType(DataType.MultilineText)]
        [UIHint(AppConstants.EditorType), AllowHtml]
        public string Description { get; set; }
    }
    public class ListSectionViewModel
    {
        public IEnumerable<Section> Sections { get; set; }
    }
    public class SectionListViewModel
    {
        public Section Section { get; set; }
        public Dictionary<CategorySummary, PermissionSet> AllPermissionSets { get; set; }
    }
    public class DeleteSectionViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }
        public Section Section { get; set; }
    }



}