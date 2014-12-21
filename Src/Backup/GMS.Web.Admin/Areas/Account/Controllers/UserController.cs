using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GMS.Account.Contract;
using GMS.Web.Admin.Common;
using GMS.Framework.Utility;
using GMS.Framework.Contract;

namespace GMS.Web.Admin.Areas.Account.Controllers
{
    [Permission(EnumBusinessPermission.AccountManage_User)]
    public class UserController : AdminControllerBase
    {
        //
        // GET: /Account/User/

        public ActionResult Index(UserRequest request)
        {
            var result = this.AccountService.GetUserList(request);
            return View(result);
        }

        //
        // GET: /Account/User/Create

        public ActionResult Create()
        {
            var roles = this.AccountService.GetRoleList();
            this.ViewBag.RoleIds = new SelectList(roles, "ID", "Name");
            
            var model = new User();
            model.Password = "111111";
            return View("Edit", model);
        }

        //
        // POST: /Account/User/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            var model = new User();
            this.TryUpdateModel<User>(model);
            model.Password = "111111";
            model.Password = Encrypt.MD5(model.Password);

            try
            {
                this.AccountService.SaveUser(model);
            }
            catch (BusinessException e)
            {
                this.ModelState.AddModelError(e.Name, e.Message);

                var roles = this.AccountService.GetRoleList();
                this.ViewBag.RoleIds = new SelectList(roles, "ID", "Name");

                return View("Edit", model);
            }

            return this.RefreshParent();
        }

        //
        // GET: /Account/User/Edit/5

        public ActionResult Edit(int id)
        {
            var model = this.AccountService.GetUser(id);

             var roles = this.AccountService.GetRoleList();
             this.ViewBag.RoleIds = new SelectList(roles, "ID", "Name", string.Join(",", model.Roles.Select(r => r.ID)));

            return View(model);
        }

        //
        // POST: /Account/User/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var model = this.AccountService.GetUser(id);
            this.TryUpdateModel<User>(model);
            this.AccountService.SaveUser(model);

            return this.RefreshParent();
        }

        // POST: /Account/User/Delete/5

        [HttpPost]
        public ActionResult Delete(List<int> ids)
        {
            this.AccountService.DeleteUser(ids);
            return RedirectToAction("Index");
        }
    }
}
