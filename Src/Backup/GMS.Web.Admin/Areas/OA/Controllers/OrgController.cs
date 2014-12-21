using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using GMS.OA.Contract;
using GMS.Account.Contract;
using GMS.Web.Admin.Common;
using GMS.Framework.Utility;

namespace GMS.Web.Admin.Areas.OA.Controllers
{
    [Permission(EnumBusinessPermission.OAManage_Org)]
    public class OrgController : AdminControllerBase
    {
        //
        // GET: /OA/Org/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetOrg()
        {
            var rootBranch = this.OAService.GetOrg();

            return Json(rootBranch, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveOrg()
        {
            try
            {
                int bytelength = Request.ContentLength;
                byte[] inputbytes = Request.BinaryRead(bytelength);
                string message = System.Text.Encoding.UTF8.GetString(inputbytes);
                var branch = JsonConvert.DeserializeObject<Branch>(message);

                this.OAService.SaveOrg(branch);
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }

            return Content("保存成功！");
        }
    }
}
