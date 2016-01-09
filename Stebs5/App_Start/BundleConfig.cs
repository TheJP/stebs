using System.Web;
using System.Web.Optimization;

namespace Stebs5
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                    "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                    "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                    "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                    "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/stebs").Include(
                    "~/Scripts/jquery-{version}.js",
                    "~/Scripts/jquery.signalR-2.2.0.min.js",
                    "~/Scripts/codemirror-5.9.min.js",
                    "~/Scripts/mode.assembler.js",
                    "~/Scripts/mousetrap-1.5.3.min.js",
                    "~/Scripts/mousetrap-global-bind.min.js",
                    "~/Scripts/main.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                    "~/Content/bootstrap.css",
                    "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/stebscss").Include(
                    "~/Content/normalize.css",
                    "~/Content/stebs.css",
                    "~/Content/codemirror.css"));
        }
    }
}
