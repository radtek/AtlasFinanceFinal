jQuery(window).ready(function()
{
    var primaryNavigation = jQuery("#PrimaryNavigation");
    var primaryTrigger = primaryNavigation.children("span.trigger").eq(0);
    var primaryUl = primaryNavigation.children("ul.menu").eq(0);
    var isPrimaryNavigationVisible = false;

    var accountNavigation;
    var accountTrigger;
    var accountUl;
    var isAccountNavigationVisible;

    var isDesktopView = false;
    var mobileBreakpointWidth = 768;

    if (jQuery("#AccountNavigation").length)
    {
        accountNavigation = jQuery("#AccountNavigation");
        accountTrigger = accountNavigation.children("span.trigger").eq(0);
        accountMenuDiv = accountNavigation.children("div.mobile-menu").eq(0);
        accountNavigationVisible = false;

        accountTrigger.bind("click", function (e)
        {
            e.preventDefault();

            isAccountNavigationVisible ? hideAccountNavigation(true) : showAccountNavigation(true);
        });

        accountMenuDiv.find("a").bind("click", function (e)
        {
            hideAccountNavigation(true);
        });
    }

    function arrange()
    {
        var windowWidth = jQuery(window).width();

        if ((windowWidth < mobileBreakpointWidth) && isDesktopView)
        {
            isDesktopView = false;

            hideprimaryNavigation(false);
        }
        else if ((windowWidth > mobileBreakpointWidth) && !isDesktopView)
        {
            isDesktopView = true;

            showprimaryNavigation(false);
        }
    }

    function showPrimaryNavigation(animateIn)
    {
        if (animateIn)
        {
            var targetHeight = 0;

            primaryUl.css({ "display": "block", "height": "auto" });
            
            targetHeight = primaryUl.outerHeight();

            primaryUl.css({"height": 0}).animate({"height": targetHeight}, "fast", function()
            {
                isPrimaryNavigationVisible = true;
            });
        }
        else
        {
            primaryUl.css({ "display": "block", "height": "auto" });

            isPrimaryNavigationVisible = true;
        }
    }

    function hidePrimaryNavigation(animateOut)
    {
        if (animateOut)
        {
            primaryUl.animate({ "height": 0 }, "fast", function()
            {
                primaryUl.css({ "display": "none" });

                isPrimaryNavigationVisible = false;
            });
        }
        else
        {
            primaryUl.css({ "display": "none" });

            isPrimaryNavigationVisible = false;
        }
    }

    function showAccountNavigation(animateIn)
    {
        accountTrigger.addClass("active");

        if (animateIn)
        {
            var targetHeight = 0;

            accountMenuDiv.css({ "display": "block", "width": "100%", "height": "auto" });

            targetHeight = accountMenuDiv.outerHeight();

            accountMenuDiv.css({ "width": 0, "height": 0 }).animate({ "width": "100%", "height": targetHeight }, "fast", function ()
            {
                isAccountNavigationVisible = true;
            });
        }
        else
        {
            accountMenuDiv.css({ "display": "block", "height": "auto" });

            isAccountNavigationVisible = true;
        }
    }

    function hideAccountNavigation(animateOut)
    {
        accountTrigger.removeClass("active");

        if (animateOut)
        {
            accountMenuDiv.animate({ "width": 0, "height": 0 }, "fast", function ()
            {
                accountMenuDiv.css({ "display": "none" });

                isAccountNavigationVisible = false;
            });
        }
        else
        {
            accountMenuDiv.css({ "display": "none" });

            isAccountNavigationVisible = false;
        }
    }

    primaryTrigger.bind("click", function (e)
    {
        e.preventDefault();

        isPrimaryNavigationVisible ? hidePrimaryNavigation(true) : showPrimaryNavigation(true);
    });

    jQuery(window).resize(function()
    {
        arrange();
    });

    arrange();
});