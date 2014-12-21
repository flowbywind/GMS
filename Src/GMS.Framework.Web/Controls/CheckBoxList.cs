using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;

namespace GMS.Framework.Web.Controls
{
    public static class CheckBoxListHelper
    {
        public static MvcHtmlString CheckBoxList(this HtmlHelper helper, string name, bool isHorizon = true)
        {
            return CheckBoxList(helper, name, helper.ViewData[name] as IEnumerable<SelectListItem>, new { }, isHorizon);
        }

        public static MvcHtmlString CheckBoxList(this HtmlHelper helper, string name, IEnumerable<SelectListItem> selectList, bool isHorizon = true)
        {
            return CheckBoxList(helper, name, selectList, new { }, isHorizon);
        }

        public static MvcHtmlString CheckBoxListFor<TModel, TProperty>(this HtmlHelper helper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, bool isHorizon = true)
        {
            string[] propertys  = expression.ToString().Split(".".ToCharArray());
            string id = string.Join("_", propertys, 1, propertys.Length - 1);
            string name = string.Join(".", propertys, 1, propertys.Length - 1);

            return CheckBoxList(helper, id, name, selectList, new { }, isHorizon);
        }

        public static MvcHtmlString CheckBoxList(this HtmlHelper helper, string name, IEnumerable<SelectListItem> selectList, object htmlAttributes, bool isHorizon = true)
        {
            return CheckBoxList(helper, name, name, selectList, htmlAttributes, isHorizon);
        }

        public static MvcHtmlString CheckBoxList(this HtmlHelper helper, string id, string name, IEnumerable<SelectListItem> selectList, object htmlAttributes, bool isHorizon = true)
        {
            IDictionary<string, object> HtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            HashSet<string> set = new HashSet<string>();
            List<SelectListItem> list = new List<SelectListItem>();
            string selectedValues = (selectList as SelectList).SelectedValue == null ? string.Empty : Convert.ToString((selectList as SelectList).SelectedValue);
            if (!string.IsNullOrEmpty(selectedValues))
            {
                if (selectedValues.Contains(","))
                {
                    string[] tempStr = selectedValues.Split(',');
                    for (int i = 0; i < tempStr.Length; i++)
                    {
                        set.Add(tempStr[i].Trim());
                    }

                }
                else
                {
                    set.Add(selectedValues);
                }
            }

            foreach (SelectListItem item in selectList)
            {
                item.Selected = (item.Value != null) ? set.Contains(item.Value) : set.Contains(item.Text);
                list.Add(item);
            }
            selectList = list;

            HtmlAttributes.Add("type", "checkbox");
            HtmlAttributes.Add("id", id);
            HtmlAttributes.Add("name", name);
            HtmlAttributes.Add("style", "border:none;");

            StringBuilder stringBuilder = new StringBuilder();

            foreach (SelectListItem selectItem in selectList)
            {
                IDictionary<string, object> newHtmlAttributes = HtmlAttributes.DeepCopy();
                newHtmlAttributes.Add("value", selectItem.Value);
                if (selectItem.Selected)
                {
                    newHtmlAttributes.Add("checked", "checked");
                }

                TagBuilder tagBuilder = new TagBuilder("input");
                tagBuilder.MergeAttributes<string, object>(newHtmlAttributes);
                string inputAllHtml = tagBuilder.ToString(TagRenderMode.SelfClosing);
                string containerFormat = isHorizon ? @"<label> {0}  {1}</label>" : @"<p><label> {0}  {1}</label></p>";
                stringBuilder.AppendFormat(containerFormat,
                   inputAllHtml, selectItem.Text);
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

