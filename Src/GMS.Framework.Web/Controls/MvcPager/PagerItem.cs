/*
 ASP.NET MvcPager 分页组件
 Copyright:2009-2011 陕西省延安市吴起县 杨涛\Webdiyer (http://www.webdiyer.com)
 Source code released under Ms-PL license
 */
namespace GMS.Framework.Web.Controls
{
    internal class PagerItem
    {
        public PagerItem(string text, int pageIndex, bool disabled, PagerItemType type)
        {
            Text = text;
            PageIndex = pageIndex;
            Disabled = disabled;
            Type = type;
        }

        internal string Text { get; set; }
        internal int PageIndex { get; set; }
        internal bool Disabled { get; set; }
        internal PagerItemType Type { get; set; }
    }

    internal enum PagerItemType:byte
    {
        FirstPage,
        NextPage,
        PrevPage,
        LastPage,
        MorePage,
        NumericPage
    }
}
