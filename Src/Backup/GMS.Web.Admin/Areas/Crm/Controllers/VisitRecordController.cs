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
using GMS.Core.Cache;

namespace GMS.Web.Admin.Areas.Crm.Controllers
{
    [Permission(EnumBusinessPermission.CrmManage_VisitRecord)]
    public class VisitRecordController : AdminControllerBase
    {
        //
        // GET: /Crm/VisitRecord/

        public ActionResult Index(VisitRecordRequest request)
        {
            this.TryUpdateModel<VisitRecord>(request.VisitRecord);

            this.ModelState.Clear();
            
            this.RenderMyViewData(request.VisitRecord, true);
            var areas = this.AreaDic.Values.Select(c => new { Id = c.ID, Name = c.Name + "-" + this.CityDic[c.CityId].Name });
            ViewData.Add("AreaId", new SelectList(areas, "Id", "Name", request.VisitRecord.AreaId));
            
            var result = this.CrmService.GetVisitRecordList(request);
            return View(result);
        }

        //
        // GET: /Crm/VisitRecord/Create

        public ActionResult Create()
        {
            var model = new VisitRecord();

            this.RenderMyViewData(model);

            return View("Edit", model);
        }

        //
        // POST: /Crm/VisitRecord/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            var model = new VisitRecord();
            this.TryUpdateModel<VisitRecord>(model);

            model.CreateTime = model.VisitTime = DateTime.Now;
            model.Username = this.UserContext.LoginInfo.LoginName;
            model.UserId = this.UserContext.LoginInfo.UserID;

            try
            {
                this.CrmService.SaveVisitRecord(model);
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
        // GET: /Crm/VisitRecord/Edit/5

        public ActionResult Edit(int id)
        {
            var model = this.CrmService.GetVisitRecord(id);
            this.RenderMyViewData(model);
            return View(model);
        }

        //
        // POST: /Crm/VisitRecord/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var model = this.CrmService.GetVisitRecord(id);
            this.TryUpdateModel<VisitRecord>(model);

            try
            {
                this.CrmService.SaveVisitRecord(model);
            }
            catch (BusinessException e)
            {
                this.ModelState.AddModelError(e.Name, e.Message);
                this.RenderMyViewData(model);
                return View("Edit", model);
            }

            return this.RefreshParent();
        }

        // POST: /Crm/VisitRecord/Delete/5

        [HttpPost]
        public ActionResult Delete(List<int> ids)
        {
            this.CrmService.DeleteVisitRecord(ids);
            return RedirectToAction("Index");
        }

        public ActionResult GetArea(int cityId)
        {
            var areas = this.AreaDic.Values.Where(a => a.CityId == cityId);
            ViewData.Add("AreaId", new SelectList(areas, "Id", "Name"));

            return PartialView("AreaSelect");
        }

        private void RenderMyViewData(VisitRecord model, bool isBasic = false)
        {
            ViewData.Add("VisitWay", new SelectList(EnumHelper.GetItemValueList<EnumVisitWay>(), "Key", "Value", model.VisitWay));
            ViewData.Add("FollowLevel", new SelectList(EnumHelper.GetItemValueList<EnumFollowLevel>(), "Key", "Value", model.FollowLevel));
            ViewData.Add("FollowStep", new SelectList(EnumHelper.GetItemValueList<EnumFollowStep>(), "Key", "Value", model.FollowStep));
            ViewData.Add("ProductType", new SelectList(EnumHelper.GetItemValueList<EnumProductType>(), "Key", "Value", model.ProductType));

            ViewData.Add("Focus", new SelectList(EnumHelper.GetItemList<EnumFocus>(), "Key", "Value", (EnumFocus)model.Focus));
            ViewData.Add("CognitiveChannel", new SelectList(EnumHelper.GetItemList<EnumCognitiveChannel>(), "Key", "Value", (EnumCognitiveChannel)model.CognitiveChannel));
            ViewData.Add("PriceResponse", new SelectList(EnumHelper.GetItemValueList<EnumPriceResponse>(), "Key", "Value", model.PriceResponse));
            ViewData.Add("AreaDemand", new SelectList(EnumHelper.GetItemValueList<EnumAreaDemand>(), "Key", "Value", model.AreaDemand));
            ViewData.Add("Motivation", new SelectList(EnumHelper.GetItemValueList<EnumMotivation>(), "Key", "Value", model.Motivation));

            ViewData.Add("ProjectId", new SelectList(this.CrmService.GetProjectList(), "Id", "Name", model.ProjectId));

            if (isBasic)
                return;

            ViewData.Add("CityId", new SelectList(this.CityDic.Values, "Id", "Name", model.CityId));

            if (model.CityId == 0)
                model.CityId = this.CityDic.First().Key;

            var areas = this.AreaDic.Values.Where(a => a.CityId == model.CityId);
            ViewData.Add("AreaId", new SelectList(areas, "Id", "Name", model.AreaId));

            var request = new CustomerRequest();
            request.Customer.UserId = this.UserContext.LoginInfo.UserID;
            var customerList = this.CrmService.GetCustomerList(request).ToList();
            customerList.ForEach(c => c.Name = string.Format("{0}({1})", c.Name, c.Tel));
            ViewData.Add("CustomerId", new SelectList(customerList, "Id", "Name", model.CustomerId));

        }

        public Dictionary<int, City> CityDic
        {
            get
            {
                return AdminCacheContext.Current.CityDic;
            }
        }

        public Dictionary<int, Area> AreaDic
        {
            get
            {
                return AdminCacheContext.Current.AreaDic;
            }
        }
    }
}
