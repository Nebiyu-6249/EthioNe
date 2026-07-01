using EthioNegarit.WebUI.Application;
using EthioNegarit.WebUI.Application.ActionFilterAttributes;

namespace EthioNegarit.WebUI.ViewModels
{
    public class PrivacyPolicyViewModel
    {
        //  public string CurrentLanguage { get; set; } = "en";
        //  public DateTime EffectiveDate { get; set; }
        [ForumResourceDisplayName("TermsAndConditions.Label.Agree")]
        [MustBeTrue(ErrorMessage = "TermsAndConditions.Label.AgreeError")]
        public bool Agree { get; set; }
        public string EffectiveDate { get; set; }
        public string Version { get; set; } = "1.0";
      /*  public string ForumName { get; set; } = string.Empty;
        public string ForumUrl { get; set; } = string.Empty;
        public string PrivacyEmail { get; set; } = string.Empty;
        public string DpoEmail { get; set; } = string.Empty;
        public string SecurityEmail { get; set; } = string.Empty;
        public string SupportEmail { get; set; } = string.Empty;
        public List<PolicySection> Sections { get; set; }*/
        public string PrivacyPolicy { get; set; }
        public string TermsAndConditions { get; set; }
    }
    public class AgreeToTermsAndConditionsViewModel
    {
        [ForumResourceDisplayName("TermsAndConditions.Label.Agree")]
        [MustBeTrue(ErrorMessage = "TermsAndConditions.Label.AgreeError")]
        public bool Agree { get; set; }
    }
    /*  public class PolicySection
      {
          public string Id { get; set; } = string.Empty;
          public string Number { get; set; } = string.Empty;
          public string Title { get; set; } = string.Empty;
          public string Content { get; set; } = string.Empty;
      }*/

}