using System.Web;
using System.Web.Optimization;

namespace Atlas.Online.Web
{
  public class BundleConfig
  {
    public static void RegisterScripts(BundleCollection bundles)
    {
      // Application Core
      bundles.Add(new ScriptBundle("~/bundles/core").Include(
        "~/Assets/js/vendor/jquery/jquery.dropkick-1.0.0.js",

        "~/Assets/js/vendor/bootstrap/bootstrap-tooltip.js",

        "~/Assets/js/vendor/angular/angular.js",
        "~/Assets/js/vendor/angular/angulartics.js",
        "~/Assets/js/vendor/angular/angulartics-google-analytics.js",
        "~/Assets/js/vendor/angular/angular-resource.js",
        "~/Assets/js/vendor/angular/angular-cookies.js",
        "~/Assets/js/vendor/angular/angular-ui-date.js",
        "~/Assets/js/vendor/angular/angular-locale_en-za.js",
        "~/Assets/js/Vendor/angular/angular-ui-dialog.js",
        "~/Assets/js/Vendor/angular/angular-ui-transition.js",
 
        "~/Assets/js/vendor/jquery/jquery.tooltipster.js",

        "~/Assets/js/shared/polyfills.js",
        "~/Assets/js/shared/config.js",
        "~/Assets/js/shared/filters.js",
        "~/Assets/js/shared/services.js",
        "~/Assets/js/shared/factories.js",
        "~/Assets/js/shared/directives.js",
        "~/Assets/js/shared/ui.js",
        "~/Assets/js/shared/core.js",
        "~/Assets/js/shared/navigation.js"
      ));

      // Bootstrap
      bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
        "~/Assets/js/vendor/bootstrap/bootstrap-collapse.js",
        "~/Assets/js/vendor/bootstrap/bootstrap-modal.js",
        //"~/Assets/js/vendor/bootstrap/bootstrap-tab.js",
        "~/Assets/js/vendor/bootstrap/bootstrap-transition.js"
      ));

      // Home application
      bundles.Add(new ScriptBundle("~/bundles/atlas-home").Include(
        "~/Assets/js/controllers/Slider.js"
      ));

      // Application
      bundles.Add(new ScriptBundle("~/bundles/atlas-application").Include(
        "~/Assets/js/controllers/application/Application.js",

        "~/Assets/js/controllers/application/LoanPicker.js",
        "~/Assets/js/controllers/application/BankingDetails.js",

        "~/Assets/js/controllers/application/PersonalDetails.js",
        "~/Assets/js/controllers/application/EmployerDetails.js",
        "~/Assets/js/controllers/application/IncomeExpenses.js",
        "~/Assets/js/controllers/application/ConfirmVerify.js",
        "~/Assets/js/controllers/application/Verify.js"
      ));

      // Affordability
      bundles.Add(new ScriptBundle("~/bundles/atlas-affordability").Include(
        "~/Assets/js/controllers/Affordability.js"
      ));

      // Verification
      bundles.Add(new ScriptBundle("~/bundles/atlas-verification").Include(
        "~/Assets/js/controllers/Verification.js"
      ));

      // Quote acceptance
      bundles.Add(new ScriptBundle("~/bundles/atlas-quoteacceptance").Include(
        "~/Assets/js/controllers/QuoteAcceptance.js"
      ));

      // My Account
      bundles.Add(new ScriptBundle("~/bundles/atlas-myaccount").Include(
        "~/Assets/js/controllers/MyAccount.js"
      ));

      // OTP
      bundles.Add(new ScriptBundle("~/bundles/atlas-otp").Include(
        "~/Assets/js/controllers/OtpController.js"
      ));

      // Vendor
      bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                  "~/Assets/js/vendor/jquery/jquery-{version}.js"));

      bundles.Add(new ScriptBundle("~/bundles/jquery-ui").Include(
                  "~/Assets/js/vendor/jquery/jquery-ui-{version}.custom.js"));
    }

    public static void RegisterStyles(BundleCollection bundles)
    {
      bundles.Add(new StyleBundle("~/assets/bundles/css").Include(
        //"~/Assets/css/vendor/bootstrap.css",
        "~/Assets/css/style.css"
      ));
    }

    // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
    public static void RegisterBundles(BundleCollection bundles)
    {
      bundles.UseCdn = true;
      
#if DEBUG
      BundleTable.EnableOptimizations = false;
#else
      BundleTable.EnableOptimizations = true;
#endif

      BundleConfig.RegisterScripts(bundles);
      BundleConfig.RegisterStyles(bundles);
    }
  }
}