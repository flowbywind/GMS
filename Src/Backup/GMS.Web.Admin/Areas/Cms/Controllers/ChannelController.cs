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
    [Permission(EnumBusinessPermission.CmsManage_Channel)]
    public class ChannelController : AdminControllerBase
    {
        //
        // GET: /Cms/Channel/

        public ActionResult Index(ChannelRequest request)
        {
            var result = this.CmsService.GetChannelList(request);
            return View(result);
        }

        //
        // GET: /Cms/Channel/Create

        public ActionResult Create()
        {
            var model = new Channel() { IsActive = true };
            return View("Edit", model);
        }

        //
        // POST: /Cms/Channel/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            var model = new Channel();
            this.TryUpdateModel<Channel>(model);

            this.CmsService.SaveChannel(model);

            return this.RefreshParent();
        }

        //
        // GET: /Cms/Channel/Edit/5

        public ActionResult Edit(int id)
        {
            var model = this.CmsService.GetChannel(id);
            return View(model);
        }

        //
        // POST: /Cms/Channel/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var model = this.CmsService.GetChannel(id);
            this.TryUpdateModel<Channel>(model);

            this.CmsService.SaveChannel(model);

            return this.RefreshParent();
        }

        // POST: /Cms/Channel/Delete/5

        [HttpPost]
        public ActionResult Delete(List<int> ids)
        {
            this.CmsService.DeleteChannel(ids);
            return RedirectToAction("Index");
        }
    }
}
