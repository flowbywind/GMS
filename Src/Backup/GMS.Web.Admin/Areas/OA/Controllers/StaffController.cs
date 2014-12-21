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
    [Permission(EnumBusinessPermission.OAManage_Staff)]
    public class StaffController : AdminControllerBase
    {
        //
        // GET: /OA/Staff/

        public ActionResult Index(StaffRequest request)
        {
            var result = this.OAService.GetStaffList(request);
            return View(result);
        }

        //
        // GET: /OA/Staff/Create

        public ActionResult Create()
        {
            var model = new Staff() { };
            this.RenderMyViewData(model);
            return View("Edit", model);
        }

        //
        // POST: /OA/Staff/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            var model = new Staff();
            this.TryUpdateModel<Staff>(model);

            this.OAService.SaveStaff(model);

            return this.RefreshParent();
        }

        //
        // GET: /OA/Staff/Edit/5

        public ActionResult Edit(int id)
        {
            var model = this.OAService.GetStaff(id);
            this.RenderMyViewData(model);
            return View(model);
        }

        //
        // POST: /OA/Staff/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var model = this.OAService.GetStaff(id);
            this.TryUpdateModel<Staff>(model);

            this.OAService.SaveStaff(model);

            return this.RefreshParent();
        }

        // POST: /OA/Staff/Delete/5

        [HttpPost]
        public ActionResult Delete(List<int> ids)
        {
            this.OAService.DeleteStaff(ids);
            return RedirectToAction("Index");
        }

        private void RenderMyViewData(Staff model)
        {
            ViewData.Add("Position", new SelectList(EnumHelper.GetItemValueList<EnumPosition>(), "Key", "Value", model.Position));
            ViewData.Add("Gender", new SelectList(EnumHelper.GetItemValueList<EnumGender>(), "Key", "Value", model.Gender));
        }
    }
}
