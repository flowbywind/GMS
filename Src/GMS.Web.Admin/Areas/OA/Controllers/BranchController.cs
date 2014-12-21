using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GMS.OA.Contract;
using GMS.Account.Contract;
using GMS.Web.Admin.Common;
using GMS.Framework.Utility;

namespace GMS.Web.Admin.Areas.OA.Controllers
{
    [Permission(EnumBusinessPermission.OAManage_Branch)]
    public class BranchController : AdminControllerBase
    {
        //
        // GET: /OA/Branch/

        public ActionResult Index(BranchRequest request)
        {
            var result = this.OAService.GetBranchList(request);
            return View(result);
        }

        //
        // GET: /OA/Branch/Create

        public ActionResult Create()
        {
            var model = new Branch();
            return View("Edit", model);
        }

        //
        // POST: /OA/Branch/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            var model = new Branch();
            this.TryUpdateModel<Branch>(model);

            this.OAService.SaveBranch(model);

            return this.RefreshParent();
        }

        //
        // GET: /OA/Branch/Edit/5

        public ActionResult Edit(int id)
        {
            var model = this.OAService.GetBranch(id);
            return View(model);
        }

        //
        // POST: /OA/Branch/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var model = this.OAService.GetBranch(id);
            this.TryUpdateModel<Branch>(model);

            this.OAService.SaveBranch(model);

            return this.RefreshParent();
        }

        // POST: /OA/Branch/Delete/5

        [HttpPost]
        public ActionResult Delete(List<int> ids)
        {
            this.OAService.DeleteBranch(ids);
            return RedirectToAction("Index");
        }
    }
}
