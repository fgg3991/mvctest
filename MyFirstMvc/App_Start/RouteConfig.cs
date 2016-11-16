using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MyFirstMvc
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            //无法显示默认的首页
            routes.MapRoute(
                name: "Default",
                url: "X{controller}-{action}",
                defaults: new { controller = "HomePage", action = "Index" }
                );
            
            //routes.MapRoute(
            //name: "Default1",
            //url: "{controller}/{action}",
            //defaults: new { controller = "HomePage", action = "Index" }
            //);
        }
    }
}