using System.ComponentModel;
using EthioNegarit.Core.Interfaces;
using EthioNegarit.Core.Interfaces.Services;
namespace EthioNegarit.WebUI.Application
{
    public class ForumResourceDisplayName : DisplayNameAttribute, IModelAttribute
    {
        private string _resourceValue = string.Empty;
        private readonly ILocalizationService _localizationService;
        public ForumResourceDisplayName(string resourceKey)
            : base(resourceKey)
        {
            ResourceKey = resourceKey;
            _localizationService = ServiceFactory.Get<ILocalizationService>();
        }
        public string ResourceKey { get; set; }
        public override string DisplayName
        {
            get
            {
                _resourceValue = _localizationService.GetResourceString(ResourceKey.Trim());
                return _resourceValue;
            }
        }
        public string Name
        {
           // get { return "FMVCResourceDisplayName"; }
            get { return "EthiopicalResourceDisplayName"; }
        }

    }
}