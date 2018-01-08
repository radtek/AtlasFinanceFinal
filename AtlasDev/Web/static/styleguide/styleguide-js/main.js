jQuery(function($) {
	'use strict';

	//	*****************************************************************************
	//	jQuery UI Date Picker
	//	*****************************************************************************
	if ($.fn.datepicker) {
		$('#datepicker').datepicker({
			minDate: -20,
			maxDate: "+1M +10D",
			dateFormat: 'd M, yy'
		});
	}

	//	*****************************************************************************
	//	Verify Spinner
	//	*****************************************************************************
	var loader = document.getElementById('verify-spinner-inner'),
			α = 0,
			π = Math.PI,
			t = 30;

	(function draw() {
		α++;
		α %= 360;
		var r = ( α * π / 180 ),
				x = Math.sin( r ) * 83,
				y = Math.cos( r ) * - 83,
				mid = ( α > 180 ) ? 1 : 0,
				anim = 'M 0 0 v -83 A 83 83 1 ' +
								mid + ' 1 ' +
								 x  + ' ' +
								 y  + ' z';
		//[x,y].forEach(function( d ){
		//  d = Math.round( d * 1e3 ) / 1e3;
		//});

		loader.setAttribute( 'd', anim );

		setTimeout(draw, t); // Redraw
	})();


	//	*****************************************************************************
	//	Dropkick Select Menus
	//	*****************************************************************************
	if ($.fn.dropkick) {
		var $select = $('.select-dropkick');

		for (var i = $select.length - 1; i >= 0; i--) {
			var $el = $($select[i]);

			$el.dropkick();

			// make select fluid
			if ($el.hasClass('d-block')) {
				$el.siblings('.dk_container').css({'float': 'none'});
				$el.siblings('.dk_container').children('.dk_toggle').css({'width': 'auto'}).addClass('d-block');
			}

			// make select disabled
			if ($el.hasClass('disabled')) {
				$el.siblings('.dk_container').addClass('disabled');
			}

		}

		$('.disabled .dk_toggle').live('click', function(e){
			$(this).parent('.dk_container').removeClass('dk_open');
		});

	}

	//	*****************************************************************************
	//	Tooltipster
	//	*****************************************************************************
	$('.tooltip-help').tooltipster({
		'trigger' : 'click'
	});


	//	*****************************************************************************
	//	Bootstrap Modal
	//	*****************************************************************************
	$('#modal').modal('hide');

	// READY
	// Remove .no-js
	$('.no-js').removeClass('no-js');

	$('#powerTip, .alert').on('click', '.close', function(e) { $(this).parent().fadeOut(); });

});