using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NorthwindMvcClient.ViewModels;

namespace NorthwindMvcClient.Views.TagHelpers
{
    public class PageLinkTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory urlHelperFactory;

        public PageLinkTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            this.urlHelperFactory = urlHelperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public PageViewModel PageModel { get; set; }

        public string PageAction { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "div";
            var tag = new TagBuilder("ul");
            tag.AddCssClass("pagination");

            if (PageModel.HasPreviousPage)
            {
                if (PageModel.TotalPages > 5 && PageModel.PageNumber > 3)
                {
                    var first = CreatePageTag(urlHelper, 1, "|<");
                    tag.InnerHtml.AppendHtml(first);
                }

                var previous = CreatePageTag(urlHelper, PageModel.PageNumber - 1, "<");
                tag.InnerHtml.AppendHtml(previous);
            }

            var bounds = GetPagesBounds(PageModel.PageNumber);
            for (int i = bounds.left; i <= bounds.right; i++)
            {
                tag.InnerHtml.AppendHtml(CreatePageTag(urlHelper, i));
            }

            if (PageModel.HasNextPage)
            {
                var next = CreatePageTag(urlHelper, this.PageModel.PageNumber + 1, ">");
                tag.InnerHtml.AppendHtml(next);
                if (PageModel.TotalPages > 5 && PageModel.PageNumber < PageModel.TotalPages - 2)
                {
                    var last = CreatePageTag(urlHelper, this.PageModel.TotalPages, ">|");
                    tag.InnerHtml.AppendHtml(last);
                }
            }

            output.Content.AppendHtml(tag);
        }

        private TagBuilder CreatePageTag(IUrlHelper urlHelper, int pageNumber, string label = null)
        {
            var item = new TagBuilder("li");
            var link = new TagBuilder("a");
            if (pageNumber == PageModel.PageNumber)
            {
                item.AddCssClass("active");
            }
            else
            {
                link.Attributes["href"] = (pageNumber, PageModel.PageSize) switch
                {
                    (1, PageViewModel.BasePageSize) => urlHelper.Action(PageAction),
                    (_, PageViewModel.BasePageSize) => urlHelper.Action(PageAction, new { page = pageNumber }),
                    (1, _) => urlHelper.Action(PageAction, new { limit = PageModel.PageSize }),
                    _ => urlHelper.Action(PageAction, new { page = pageNumber, limit = PageModel.PageSize }),
                };
            }
            
            item.AddCssClass("page-item");
            link.AddCssClass("page-link");
            link.InnerHtml.Append(label ?? pageNumber.ToString());
            item.InnerHtml.AppendHtml(link);

            return item;
        }

        private (int left, int right) GetPagesBounds(int pageNumber) =>
            pageNumber switch
            {
                <= 2 => (1, PageModel.TotalPages > 5 ? 5 : PageModel.TotalPages),
                _ when pageNumber + 2 > PageModel.TotalPages => (PageModel.TotalPages > 5 ? (PageModel.TotalPages - 4) : 1, PageModel.TotalPages),
                _ => (pageNumber - 2, pageNumber + 2),
            };
    }
}
