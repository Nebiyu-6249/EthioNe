using EthioNegarit.Core.Domain.Entities;
using System;
using System.Collections.Generic;

namespace EthioNegarit.WebUI.ViewModels
{
    public class LanguageListAllViewModel
    {
        public IEnumerable<Language> Alllanguages { get; set; }
        public Guid CurrentLanguage { get; set; }
    }
}