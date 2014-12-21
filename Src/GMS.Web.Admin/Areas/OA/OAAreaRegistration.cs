using System.Web.Mvc;

namespace GMS.Web.Admin.Areas.OA
{
    public class OAAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "OA";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "OA_default",
                "OA/{controller}/{action}/{id}",
                new { action = "Default", id = UrlParameter.Optional }
            );
        }
    }
}
