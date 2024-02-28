(function($) {
	"use strict";

	// ______________ SWITCHER-toggle ______________//

	$('.layout-setting').on("click", function(e) {
		if (document) {
			$('body').toggleClass('dark-theme');
		} else {
			$('body').removeClass('dark-theme');
			$('body').addClass('light-theme');
		}
	});

	/***************** RTL Start *********************/

	/* RTL version Start */
		// $('body').addClass('rtl');
	/* RTL version End */

	//when body conatins 'rtl' class
	$(document).ready(function () {
		let bodyRtl = $('body').hasClass('rtl');
		if (bodyRtl) {
			$('body').addClass('rtl');
			$('body').removeClass('ltr');
			$("html[lang=en]").attr("dir", "rtl");
			$(".select2-container").attr("dir", "rtl");
			localStorage.setItem("rtl", "True");
			$("head link#style").attr("href", $(this));
			(document.getElementById("style").setAttribute("href", "/assets/plugins/bootstrap/css/bootstrap.rtl.min.css"));
			var carousel = $('.owl-carousel');
			$.each(carousel ,function( index, element ) {
			// element == this
				var carouselData = $(element).data('owl.carousel');
				carouselData.settings.rtl = true; //don't know if both are necessary
				carouselData.options.rtl = true;
				$(element).trigger('refresh.owl.carousel');
			});
		}
		else {
			$('body').addClass('ltr');
			$('body').removeClass('rtl');
			$("html[lang=en]").attr("dir", "ltr");
			$(".select2-container").attr("dir", "ltr");
			localStorage.setItem("rtl", "false");
			$("head link#style").attr("href", $(this));
			(document.getElementById("style").setAttribute("href", "/assets/plugins/bootstrap/css/bootstrap.min.css"));
		};
	});

	/***************** RTL End *********************/

	/* Light Theme Start */
		// $('body').addClass('light-theme');
	/* Light Theme End */

	/* Dark Theme Start */
		// $('body').addClass('dark-theme');
	/* Dark Theme End */

	/* Transparent Theme Start */
		// $('body').addClass('transparent-theme');
	/* Transparent Theme End */

	/* Default Header Start */
		// $('body').addClass('default-header');
	/* Default Header End */

	/* Header Style 1 Start */
		// $('body').addClass('headerstyle1');
	/* Header Style 1 End */

	/* Header Style 2 Start */
		// $('body').addClass('headerstyle2');
	/* Header Style 2 End */

	/* Default Menu Start */
		// $('body').addClass('default-menu');
	/* Default Menu End */

	/* Menu Style 1 Start */
		// $('body').addClass('menu-style1');
	/* Menu Style 1 End */

	/* Menu Style 2 Start */
		// $('body').addClass('menu-style2');
	/* Menu Style 2 End */
})(jQuery);

//Reset all

function resetData() {
	$('#myonoffswitch57').prop('checked', false);
	$('#myonoffswitch56').prop('checked', true);
	$('#myonoffswitch58').prop('checked', false);
	$('#myonoffswitch').prop('checked', true);
	$('#myonoffswitch3').prop('checked', true);
	$('#myonoffswitch1').prop('checked', false);
	$('#myonoffswitch2').prop('checked', false);
	$('#myonoffswitch4').prop('checked', false);
	$('#myonoffswitch16').prop('checked', false);
	$('#myonoffswitch17').prop('checked', false);
	$('#myonoffswitch18').prop('checked', false);
	$('body')?.removeClass('dark-theme');
	$('body')?.removeClass('transparent-theme');
	$('body')?.removeClass('default-header');
	$('body')?.removeClass('default-menu');
	$('body')?.removeClass('headerstyle1');
	$('body')?.removeClass('headerstyle2');
	$('body')?.removeClass('menu-style1');
	$('body')?.removeClass('menu-style2');

	names()
}