using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PM.MVC.TagHelpers
{
    [HtmlTargetElement("li", Attributes = "active-when")]
    public class LiTagHelper : TagHelper
    {
        public string ActiveWhen { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContextData { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (ActiveWhen == null)
            {
                return;
            }

            var targetController = ActiveWhen.Split("/")[1];
            var targetAction = ActiveWhen.Split("/")[2];

            var currentController = ViewContextData.RouteData.Values["controller"]?.ToString();
            var currentAction = ViewContextData.RouteData.Values["action"]?.ToString();
            var currentPage = ViewContextData.RouteData.Values["page"]?.ToString();

            if (currentController != null && currentAction != null && currentController.Equals(targetController) && currentAction.Equals(targetAction))
            {
                AddClass(output);
            }
            else if (currentPage != null && currentPage.Equals(ActiveWhen))
            {
                AddClass(output);
            }
        }

        private static void AddClass(TagHelperOutput output)
        {
            if (output.Attributes.ContainsName("class"))
            {
                output.Attributes.SetAttribute("class", $"{output.Attributes["class"].Value} active");
            }
            else
            {
                output.Attributes.SetAttribute("class", "active");
            }
        }
    }
}
