using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Web.Routing;
using GMS.Framework.Contract;


namespace GMS.Framework.Web.Controls
{
    /// <summary>
    /// 分页控件专为SEO，显示为1-20, 21-40.....
    /// </summary>
    public static class PagerForSeo
    {
        public static MvcHtmlString SeoPager(this HtmlHelper helper, IPagedList pagedList, string pageIndexParameterName = "id", int sectionSize = 20)
        {
            var sb = new StringBuilder();

            int pageCount = pagedList.TotalItemCount / pagedList.PageSize + (pagedList.TotalItemCount % pagedList.PageSize == 0 ? 0 : 1);

            if (pageCount > 1)
            {
                var pages = new List<int>();
                for (int i = 1; i <= pageCount; i++)
                    pages.Add(i);

                var sections = pages.GroupBy(p => (p - 1) / sectionSize);

                var currentSection = sections.Single(s => s.Key == (pagedList.CurrentPageIndex - 1) / sectionSize);

                foreach (var p in currentSection)
                {
                    if (p == pagedList.CurrentPageIndex)
                        sb.AppendFormat("<span>{0}</span>", p);
                    else
                        sb.AppendFormat("<a href=\"{1}\">{0}</a>", p, PrepearRouteUrl(helper, pageIndexParameterName, p));
                }

                if (sections.Count() > 1)
                {
                    sb.Append("<br/>");

                    foreach (var s in sections)
                    {
                        if (s.Key == currentSection.Key)
                            sb.AppendFormat("<span>{0}-{1}</span>", s.First(), s.Last());
                        else
                            sb.AppendFormat("<a href=\"{2}\">{0}-{1}</a>", s.First(), s.Last(), PrepearRouteUrl(helper, pageIndexParameterName, s.First()));
                    }
                }
            }

            return MvcHtmlString.Create(sb.ToString());
        }

        private static string PrepearRouteUrl(HtmlHelper helper, string pageIndexParameterName, int pageIndex)
        {
            var routeValues = new RouteValueDictionary();
            routeValues["action"] = helper.ViewContext.RequestContext.RouteData.Values["action"];
            routeValues["controller"] = helper.ViewContext.RequestContext.RouteData.Values["controller"];
            routeValues[pageIndexParameterName] = pageIndex;
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            return urlHelper.RouteUrl(routeValues);
        }

    }
}
