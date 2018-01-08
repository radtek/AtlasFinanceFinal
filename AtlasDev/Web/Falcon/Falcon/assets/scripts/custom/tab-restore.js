var TabRestore = function() {
  return {
    Restore: function() {
      var json, tabsState;
      $('a[data-toggle="tab"]').on('shown.bs.tab', function(e) {
        var href, json, parentId, tabsState;

        tabsState = localStorage.getItem("tabs-state");
        json = JSON.parse(tabsState || "{}");
        parentId = $(e.target).parents("ul.nav.nav-tabs").attr("id");
        href = $(e.target).attr('href');
        json[parentId] = href;

        return localStorage.setItem("tabs-state", JSON.stringify(json));
      });

      tabsState = localStorage.getItem("tabs-state");
      json = JSON.parse(tabsState || "{}");

      $.each(json, function(containerId, href) {
        return $("#" + containerId + " a[href=" + href + "]").tab('show');
      });

      $("ul.nav.nav-tabs").each(function() {
        if (!json[$(this).attr("id")])
          return $(this).find("a[data-toggle=tab]:first").tab("show");
      });
    }
  };
}();