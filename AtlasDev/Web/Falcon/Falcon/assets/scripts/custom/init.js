jQuery(document).ready(function() {

  if (platform.name !== 'Chrome' && platform.name !== 'Chrome Mobile') {
        $('body').append('<div class="modal fade" id="old-browser" data-backdrop="static" data-keyboard="false" ><div class="modal-dialog modal-small"><div class="modal-content"><div class="modal-header"><h4 class="modal-title"> <center>Your browser is not supported.</center></h4></div><div class="modal-body"><p>This web application is currently only supported on chrome.</p></div><div class="modal-footer"></div></div></div></div>');
        $('#old-browser').modal('show');
    } else {
        Metronic.init(); // init metronic core componets
        Layout.init(); // init layout
        QuickSidebar.init()
        Index.init();
        UIIdleTimeout.init();
        TabRestore.Restore();       
        //ComponentsjQueryUISliders.init();
    }
});