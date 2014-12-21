using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace GMS.Framework.Web.Controls
{
    public static class RadioBoxListHelper
    {
        public static MvcHtmlString RadioBoxList(this HtmlHelper helper, string name)
        {
            return RadioBoxList(helper, name, helper.ViewData[name] as IEnumerable<SelectListItem>, new { });
        }
        
        public static MvcHtmlString RadioBoxList(this HtmlHelper helper, string name, IEnumerable<SelectListItem> selectList)
        {
            return RadioBoxList(helper, name, selectList, new { });
        }

        public static MvcHtmlString RadioBoxList(this HtmlHelper helper, string name, IEnumerable<SelectListItem> selectList, object htmlAttributes)
        {
            IDictionary<string, object> HtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            HtmlAttributes.Add("type", "radio");
            HtmlAttributes.Add("name", name);

            StringBuilder stringBuilder = new StringBuilder();
            int i = 0;
            int j = 0;
            foreach (SelectListItem selectItem in selectList)
            {
                string id = string.Format("{0}{1}", name, j++);
                
                IDictionary<string, object> newHtmlAttributes = HtmlAttributes.DeepCopy();
                newHtmlAttributes.Add("value", selectItem.Value);
                newHtmlAttributes.Add("id", id);
                var selectedValue = (selectList as SelectList).SelectedValue;
                if (selectedValue == null)
                {
                    if (i++ == 0)
                        newHtmlAttributes.Add("checked", null);
                }
                else if (selectItem.Value == selectedValue.ToString())
                {
                    newHtmlAttributes.Add("checked", null);
                }

                TagBuilder tagBuilder = new TagBuilder("input");
                tagBuilder.MergeAttributes<string, object>(newHtmlAttributes);
                string inputAllHtml = tagBuilder.ToString(TagRenderMode.SelfClosing);
                stringBuilder.AppendFormat(@" {0}  <label for='{2}'>{1}</label>",
                   inputAllHtml, selectItem.Text, id);
            }
            return MvcHtmlString.Create(stringBuilder.ToString());

        }
        private static IDictionary<string, object> DeepCopy(this IDictionary<string, object> ht)
        {
            Dictionary<string, object> _ht = new Dictionary<string, object>();

            foreach (var p in ht)
            {
                _ht.Add(p.Key, p.Value);
            }
            return _ht;
        } 
    }

}

