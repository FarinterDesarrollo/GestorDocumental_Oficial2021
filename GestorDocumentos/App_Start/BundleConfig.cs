using System.Web;
using System.Web.Optimization;

namespace GestorDocumentos
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/popper").Include(
                      "~/Scripts/umd/popper.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/main").Include(
                      "~/Scripts/main.js"));

            bundles.Add(new ScriptBundle("~/bundles/pagination").Include(
                      "~/Scripts/Paginacion.js"));

            bundles.Add(new ScriptBundle("~/bundles/Scripts/sweetalert").Include(
                      "~/Scripts/sweetalert.js",
                      "~/Scripts/sweetalert.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/DataTables/jquery").Include(
                      "~/Scripts/DataTables/jquery.dataTables.js"));

            bundles.Add(new ScriptBundle("~/bundles/DataTables/jquery").Include(
                      "~/Scripts/DataTables/jquery.dataTables.min.js"));

            ScriptBundle scriptBndl = new ScriptBundle("~/bundles/moment");

            scriptBndl.Include(
                            "~/Scripts/moment.js",
                            "~/Scripts/moment-with-locales.js"
                          );

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/inicio.css",
                      "~/Content/DataTables/css/jquery.dataTables.min.css",
                      "~/Content/sweetalert/sweet-alert.css"));

        }
    }
}
