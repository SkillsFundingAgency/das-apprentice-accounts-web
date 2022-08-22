using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SFA.DAS.ApprenticeAccounts.Web.TagHelpers
{
    [HtmlTargetElement("div", Attributes = "validation-row-status")]
    public class ValidationRowHelper : TagHelper
    {
        [ViewContext]
        public ViewContext ViewContext { get; set; } = null!;

        public string PropertyName { get; set; } = null!;
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (PropertyIsInvalid())
            {
                var builder = new TagBuilder("div");
                builder.Attributes.Add("class", "govuk-form-group--error");
                output.MergeAttributes(builder);
            }
        }

        bool PropertyIsInvalid()
        {
            return ViewContext?.ModelState[PropertyName]?.ValidationState == ModelValidationState.Invalid;
        }
    }
}