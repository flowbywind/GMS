using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GMS.Cms.Contract;
using GMS.Account.Contract;
using GMS.Web.Admin.Common;
using GMS.Framework.Utility;

namespace GMS.Web.Admin.Areas.Cms.Controllers
{
    [Permission(EnumBusinessPermission.CmsManage_Article)]
    public class ArticleController : AdminControllerBase
    {
        //
        // GET: /Cms/Article/

        public ActionResult Index(ArticleRequest request)
        {
            var channelList = this.CmsService.GetChannelList(new ChannelRequest() { IsActive = true });
            this.ViewBag.ChannelId = new SelectList(channelList, "ID", "Name");
            
            var result = this.CmsService.GetArticleList(request);
            return View(result);
        }

        //
        // GET: /Cms/Article/Create

        public ActionResult Create()
        {
            var channelList = this.CmsService.GetChannelList(new ChannelRequest() { IsActive = true});
            this.ViewBag.ChannelId = new SelectList(channelList, "ID", "Name");
            this.ViewBag.Tags = this.CmsService.GetTagList(new TagRequest() { Top = 20, Orderby = Orderby.Hits });

            var model = new Article() { IsActive = true };
            return View("Edit", model);
        }

        //
        // POST: /Cms/Article/Create

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(FormCollection collection)
        {
            var model = new Article() { UserId = this.UserContext.LoginInfo.UserID, UserName = this.UserContext.LoginInfo.LoginName };
            this.TryUpdateModel<Article>(model);

            this.CmsService.SaveArticle(model);

            return this.RefreshParent();
        }

        //
        // GET: /Cms/Article/Edit/5

        public ActionResult Edit(int id)
        {
            var model = this.CmsService.GetArticle(id);

            var channelList = this.CmsService.GetChannelList(new ChannelRequest() { IsActive = true });
            this.ViewBag.ChannelId = new SelectList(channelList, "ID", "Name");
            this.ViewBag.Tags = this.CmsService.GetTagList(new TagRequest() { Top = 20, Orderby = Orderby.Hits });

            return View(model);
        }

        //
        // POST: /Cms/Article/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var model = this.CmsService.GetArticle(id);
            this.TryUpdateModel<Article>(model);

            this.CmsService.SaveArticle(model);

            return this.RefreshParent();
        }

        // POST: /Cms/Article/Delete/5

        [HttpPost]
        public ActionResult Delete(List<int> ids)
        {
            this.CmsService.DeleteArticle(ids);
            return RedirectToAction("Index");
        }
    }
}
