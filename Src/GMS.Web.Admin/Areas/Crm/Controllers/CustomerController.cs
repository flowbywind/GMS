using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GMS.Crm.Contract;
using GMS.Account.Contract;
using GMS.Web.Admin.Common;
using GMS.Framework.Utility;
using GMS.Framework.Contract;

namespace GMS.Web.Admin.Areas.Crm.Controllers
{
    [Permission(EnumBusinessPermission.CrmManage_Customer)]
    public class CustomerController : AdminControllerBase
    {
        //
        // GET: /Crm/Customer/

        public ActionResult Index(CustomerRequest request)
        {
            this.TryUpdateModel<Customer>(request.Customer);

            this.ModelState.Clear();
            
            this.RenderMyViewData(request.Customer, true);
            
            var result = this.CrmService.GetCustomerList(request);
            return View(result);
        }

        //
        // GET: /Crm/Customer/Create

        public ActionResult Create()
        {
            var model = new Customer();

            this.RenderMyViewData(model);

            return View("Edit", model);
        }

        //
        // POST: /Crm/Customer/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            var model = new Customer();
            this.TryUpdateModel<Customer>(model);

            try
            {
                this.CrmService.SaveCustomer(model);
            }
            catch (BusinessException e)
            {
                this.ModelState.AddModelError(e.Name, e.Message);
                this.RenderMyViewData(model);
                return View("Edit", model);
            }
            

            return this.RefreshParent();
        }

        //
        // GET: /Crm/Customer/Edit/5

        public ActionResult Edit(int id)
        {
            var model = this.CrmService.GetCustomer(id);
            this.RenderMyViewData(model);
            return View(model);
        }

        //
        // POST: /Crm/Customer/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var model = this.CrmService.GetCustomer(id);
            this.TryUpdateModel<Customer>(model);

            try
            {
                this.CrmService.SaveCustomer(model);
            }
            catch (BusinessException e)
            {
                this.ModelState.AddModelError(e.Name, e.Message);
                this.RenderMyViewData(model);
                return View("Edit", model);
            }

            return this.RefreshParent();
        }

        // POST: /Crm/Customer/Delete/5

        [HttpPost]
        public ActionResult Delete(List<int> ids)
        {
            this.CrmService.DeleteCustomer(ids);
            return RedirectToAction("Index");
        }

        private void RenderMyViewData(Customer model, bool isBasic = false)
        {
            ViewData.Add("Gender", new SelectList(EnumHelper.GetItemValueList<EnumGender>(), "Key", "Value", model.Gender));
            ViewData.Add("Category", new SelectList(EnumHelper.GetItemValueList<EnumCategory>(), "Key", "Value", model.Category));
            ViewData.Add("Profession", new SelectList(EnumHelper.GetItemValueList<EnumProfession>(), "Key", "Value", model.Profession));
            ViewData.Add("AgeGroup", new SelectList(EnumHelper.GetItemValueList<EnumAgeGroup>(), "Key", "Value", model.AgeGroup));

            if (isBasic)
                return;

            ViewData.Add("UserId", new SelectList(this.AccountService.GetUserList(), "ID", "LoginName", model.UserId));
        }
    }
}
