using System.Web.Mvc;

namespace Avarez.Areas.NewVer
{
    public class NewVerAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "NewVer";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "NewVer_default",
                "NewVer/{controller}/{action}/{id}",
                new { controller = "First", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
