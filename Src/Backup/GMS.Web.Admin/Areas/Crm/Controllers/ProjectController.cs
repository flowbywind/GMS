using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GMS.Crm.Contract;
using GMS.Account.Contract;
using GMS.Web.Admin.Common;
using GMS.Framework.Utility;

namespace GMS.Web.Admin.Areas.Crm.Controllers
{
    [Permission(EnumBusinessPermission.CrmManage_Project)]
    public class ProjectController : AdminControllerBase
    {
        //
        // GET: /Crm/Project/

        public ActionResult Index(ProjectRequest request)
        {
            var result = this.CrmService.GetProjectList(request);
            return View(result);
        }

        //
        // GET: /Crm/Project/Create

        public ActionResult Create()
        {
            var model = new Project() {  };
            return View("Edit", model);
        }

        //
        // POST: /Crm/Project/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            var model = new Project();
            this.TryUpdateModel<Project>(model);

            this.CrmService.SaveProject(model);

            return this.RefreshParent();
        }

        //
        // GET: /Crm/Project/Edit/5

        public ActionResult Edit(int id)
        {
            var model = this.CrmService.GetProject(id);
            return View(model);
        }

        //
        // POST: /Crm/Project/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var model = this.CrmService.GetProject(id);
            this.TryUpdateModel<Project>(model);

            this.CrmService.SaveProject(model);

            return this.RefreshParent();
        }

        // POST: /Crm/Project/Delete/5

        [HttpPost]
        public ActionResult Delete(List<int> ids)
        {
            this.CrmService.DeleteProject(ids);
            return RedirectToAction("Index");
        }
    }
}
