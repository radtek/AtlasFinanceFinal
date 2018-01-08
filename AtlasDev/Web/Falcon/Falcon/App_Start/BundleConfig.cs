using System.Web.Optimization;

namespace Falcon
{
  public class BundleConfig
  {
    // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862

    public class BundlesFormat
    {
      public const string STYLE_COLOR = @"<link href=""{0}"" rel=""stylesheet"" type=""text/css"" id=""style_color"" />";
    }
    public static void RegisterBundles(BundleCollection bundles)
    {
      // Boot strap bundles
      bundles.Add(new StyleBundle("~/mandatory").Include(
        "~/assets/plugins/font-awesome/css/font-awesome.min.css",
        "~/assets/plugins/bootstrap/css/bootstrap.min.css",
        "~/assets/plugins/bootstrap-select/bootstrap-select.min.css",
        "~/assets/plugins/jquery-multi-select/css/multi-select.css",
        "~/assets/plugins/bootstrap-datetimepicker/css/datetimepicker.css",
        "~/assets/plugins/select2/select2.css",
        "~/assets/plugins/simple-line-icons/simple-line-icons.min.css",
        "~/assets/plugins/uniform/css/uniform.default.css",
        "~/assets/css/ng-tags-input.css",
        "~/assets/css/ng-tags-input.bootstrap.css",
        "~/assets/css/animate.css",
        "~/assets/css/components-rounded.css",
        "~/assets/css/plugins.css",
        "~/assets/css/layout.css",
        "~/assets/css/themes/red-sunglo.css",
        "~/assets/css/toaster.css",
        "~/assets/css/ng-table.css",
        "~/assets/css/custom.css",
        "~/assets/css/devexpress/dx.common.css",
        "~/assets/css/devexpress/dx.dark.css",
        "~/assets/css/devexpress/dx.light.css",
        "~/assets/scripts/angular-multi.select/css/angular-multi-select.css",
        "~/assets/plugins/bootstrap-datepicker/css/datepicker3.css"));

      bundles.Add(new StyleBundle("~/loginCss").Include(
         "~/assets/css/pages/login.css",
         "~/assets/css/animate.css"));

      bundles.Add(new StyleBundle("~/lockOut").Include(
        "~/assets/css/pages/lock.css"));

      bundles.Add(new StyleBundle("~/AccountSearchCSS").Include(
        "~/assets/css/pages/search.css"));

      bundles.Add(new StyleBundle("~/LockOutCSS").Include(
        "~/assets/css/pages/lock.css"));

      bundles.Add(new ScriptBundle("~/StandardJs").Include(
      "~/assets/plugins/jquery-2.1.1.js",
      "~/assets/plugins/jquery-slimscroll/jquery.slimscroll.min.js",
      "~/assets/plugins/jquery.blockui.min.js",
      "~/assets/plugins/jquery.cokie.min.js",
      "~/assets/plugins/jquery-validation/js/jquery.validate.min.js",
      "~/assets/plugins/jquery-idle-timeout/jquery.idletimeout.js",
      "~/assets/plugins/jquery-idle-timeout/jquery.idletimer.js",
      "~/assets/plugins/jquery-inputmask/jquery.inputmask.bundle.js",
      "~/assets/plugins/bootstrap-select/bootstrap-select.min.js",
      "~/assets/plugins/select2/select2.min.js",
      "~/assets/plugins/jquery-multi-select/js/jquery.multi-select.js",
      "~/assets/plugins/jquery-price-format/jquery.price_format.2.0.min.js",
      "~/assets/plugins/jquery-validation/dist/jquery.validate.min.js",
      "~/assets/plugins/bootstrap/js/bootstrap.min.js",
      "~/assets/plugins/bootstrap-hover-dropdown/bootstrap-hover-dropdown.min.js",
      "~/assets/plugins/bootstrap-datepicker/js/bootstrap-datepicker.js",
      "~/assets/plugins/bootstrap-datetimepicker/js/bootstrap-datetimepicker.min.js",
      "~/assets/plugins/jquery.pulsate.min.js",
      "~/assets/plugins/uniform/jquery.uniform.min.js",
      "~/assets/plugins/select2/select2.min.js",
      "~/assets/scripts/xsockets/XSockets.latest.js",
      "~/assets/scripts/components-form-tools.js",
      "~/assets/scripts/components-pickers.js",
      "~/assets/scripts/components-jqueryui-sliders.js",
      "~/assets/scripts/ui-idletimeout.js",
      "~/assets/scripts/metronic.js",
      "~/assets/scripts/layout.js",
      "~/assets/scripts/quick-sidebar.js",
      "~/assets/scripts/platform.js",
      "~/assets/scripts/moment.js",
      "~/assets/scripts/json2.js",
      "~/assets/scripts/linq/linq.js",
      "~/assets/scripts/linq/linq.jquery.js",
      "~/assets/scripts/custom/tab-restore.js",
      "~/assets/scripts/custom/init.js",
      "~/assets/scripts/index.js",
      "~/assets/scripts/login.js",
      "~/assets/scripts/custom.js",
      "~/assets/scripts/signalr/jquery.signalR-2.2.0.js",
      "~/assets/scripts/Queue.src.js",
      "~/assets/scripts/angular/1.3/angular.js",
      "~/assets/scripts/angular/1.3/angular-route.js",
      "~/assets/scripts/angular/1.3/angular-resource.js",
      "~/assets/scripts/angular/1.3/angular-cookies.js",
      "~/assets/scripts/angular/1.3/i18n/angular-locale_en-za.js",
      "~/assets/scripts/angular/1.3/plugins/ng-table.js",
      "~/assets/scripts/angular/1.3/plugins/angular-moment.js",
      "~/assets/scripts/angular/1.3/angular-animate.js",
      "~/assets/scripts/angular/1.3/plugins/angular-toastr.js",
      "~/assets/scripts/angular/1.3/plugins/ngRemoteValidate.0.4.1.js",
      "~/assets/scripts/angular/1.3/plugins/ng-tags-input.js",
      "~/assets/scripts/angular/1.3/plugins/smart-table.debug.js",
      "~/assets/scripts/angular/1.3/plugins/angular-signalR.js",
      "~/assets/scripts/angular/1.3/plugins/si-table.js",
      "~/assets/scripts/angular/1.3/plugins/ng-context-menu.js",
      "~/assets/scripts/angular/1.3/angular-sanitize.js",
      "~/assets/scripts/angular-multi.select/js/angular-multi-select.js",
      "~/assets/scripts/ngprogress.min.js",
      "~/assets/plugins/jquery.globalize/globalize.js",
      "~/assets/plugins/jquery.globalize/cultures/globalize.culture.en-ZA.js",
      "~/assets/scripts/printThis.js"
        //,
        //"~/assets/plugins/devexpress/dx.all.js",
        //"~/assets/plugins/devexpress/dx.chartjs.debug.js",
        //"~/assets/plugins/devexpress/dx.exporter.debug.js",
        //"~/assets/plugins/devexpress/dx.webappjs.debug.js"
        //"~/assets/plugins/filesaver/FileSaver.js",
      ).IncludeDirectory(
      "~/app", "*.js").IncludeDirectory("~/app/factories", "*.js").IncludeDirectory(
      "~/app/common", "*.js").IncludeDirectory(
      "~/app/directives", "*.js").IncludeDirectory(
      "~/app/controllers/account", "*.js").IncludeDirectory(
      "~/app/controllers/naedo", "*.js").IncludeDirectory(
      "~/app/controllers/target", "*.js").IncludeDirectory(
      "~/app/controllers", "*.js"));

#if DEBUG
      BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
    }
  }
}
