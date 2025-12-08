using System.Web.Mvc;

namespace Avarez.Areas.Tax
{
    public class TaxAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Tax";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Tax_default",
                "Tax/{controller}/{action}/{id}",
                new { controller = "TaxHome", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
