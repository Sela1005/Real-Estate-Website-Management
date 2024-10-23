using System.Web;
using System.Web.Optimization;

namespace BatDongSan
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/jsabc").Include("~/js/bootstrap.bundle.min.js"));
            bundles.Add(new ScriptBundle("~/jsb").Include("~/js/tiny-slider.js"));
            bundles.Add(new ScriptBundle("~/jsc").Include("~/js/aos.js"));
            bundles.Add(new ScriptBundle("~/jsd").Include("~/js/navbar.js"));
            bundles.Add(new ScriptBundle("~/jse").Include("~/js/counter.js"));
            bundles.Add(new ScriptBundle("~/jsf").Include("~/js/custom.js"));



            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new StyleBundle("~/cssa").Include("~/fonts/icomoon/style.css"));
            bundles.Add(new StyleBundle("~/cssb").Include("~/fonts/flaticon/font/flaticon.css"));
            bundles.Add(new StyleBundle("~/cssc").Include("~/css/tiny-slider.css"));
            bundles.Add(new StyleBundle("~/cssd").Include("~/css/aos.css"));
            bundles.Add(new StyleBundle("~/csse").Include("~/css/style.css"));
        }
    }
}
