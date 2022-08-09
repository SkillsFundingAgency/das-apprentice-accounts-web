using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
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
                CreateOrMergeAttribute("class", "govuk-form-group--error", output);
        }

        bool PropertyIsInvalid()
        {
            return ViewContext?.ModelState[PropertyName]?.ValidationState == ModelValidationState.Invalid;
        }

        void CreateOrMergeAttribute(string name, object content, TagHelperOutput output)
        {
            var currentAttribute = output.Attributes.FirstOrDefault(attribute => attribute.Name == name);
            if (currentAttribute == null)
            {
                var attribute = new TagHelperAttribute(name, content);
                output.Attributes.Add(attribute);
            }
            else
            {
                var newAttribute = new TagHelperAttribute(
                    name,
                    $"{currentAttribute.Value.ToString()} {content.ToString()}",
                    currentAttribute.ValueStyle);
                output.Attributes.Remove(currentAttribute);
                output.Attributes.Add(newAttribute);
            }
        }
    }
}