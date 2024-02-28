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

	$('#myonoffswitch55').click(function() {
        if (this.checked) {
			$('body').addClass('rtl');
			$('body').removeClass('ltr');
			$("html[lang=en]").attr("dir", "rtl");
			$(".select2-container").attr("dir", "rtl");
			localStorage.setItem("rtl", "True");
			$("head link#style").attr("href", $(this));
			(document.getElementById("style").setAttribute("href", "../assets/plugins/bootstrap/css/bootstrap.rtl.min.css"));
			var carousel = $('.owl-carousel');
			$.each(carousel ,function( index, element ) {
			// element == this
				var carouselData = $(element).data('owl.carousel');
				carouselData.settings.rtl = true; //don't know if both are necessary
				carouselData.options.rtl = true;
				$(element).trigger('refresh.owl.carousel');
			});
        } else {
			$('body').addClass('ltr');
			$('body').removeClass('rtl');
			$("html[lang=en]").attr("dir", "ltr");
			$(".select2-container").attr("dir", "ltr");
			localStorage.setItem("rtl", "false");
			$("head link#style").attr("href", $(this));
			(document.getElementById("style").setAttribute("href", "../assets/plugins/bootstrap/css/bootstrap.min.css"));
         }
    });

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
			(document.getElementById("style").setAttribute("href", "../assets/plugins/bootstrap/css/bootstrap.rtl.min.css"));
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
			(document.getElementById("style").setAttribute("href", "../assets/plugins/bootstrap/css/bootstrap.min.css"));
		};
	});

	/***************** RTL End *********************/

	/***************** LTR Start *********************/

    $('#myonoffswitch54').click(function() {
        if (this.checked) {
			$('body').addClass('ltr');
			$('body').removeClass('rtl');
			$("html[lang=en]").attr("dir", "ltr");
			$(".select2-container").attr("dir", "ltr");
            localStorage.setItem("rtl", "false");
            $("head link#style").attr("href", $(this));
            (document.getElementById("style").setAttribute("href", "../assets/plugins/bootstrap/css/bootstrap.min.css"));
			var carousel = $('.owl-carousel');
			$.each(carousel ,function( index, element ) {
			// element == this
				var carouselData = $(element).data('owl.carousel');
				carouselData.settings.rtl = false; //don't know if both are necessary
				carouselData.options.rtl = false;
				$(element).trigger('refresh.owl.carousel');
			});
        } else {
			$('body').addClass('rtl');
			$('body').removeClass('ltr');
			$("html[lang=en]").attr("dir", "rtl");
			$(".select2-container").attr("dir", "rtl");
            localStorage.setItem("ltr", "false");
            $("head link#style").attr("href", $(this));
            (document.getElementById("style").setAttribute("href", "../assets/plugins/bootstrap/css/bootstrap.rtl.min.css"));
         }
    });

	/***************** LTR End *********************/

	/***************** Light Theme Start *********************/

	$('#myonoffswitch56').click(function() {
        if (this.checked) {
			$('body').addClass('light-theme');
			$('body').removeClass('dark-theme');
			$('body').removeClass('transparent-theme');

			// remove dark theme properties
			localStorage.removeItem('darkPrimary')
			localStorage.removeItem('darksecondary')

			// remove light theme properties
			localStorage.removeItem('primaryColor')
			localStorage.removeItem('primaryHoverColor')
			localStorage.removeItem('primaryBorderColor')
			localStorage.removeItem('secondaryColor')
			localStorage.removeItem('secondaryHoverColor')
			localStorage.removeItem('secondaryBorderColor')
			document.querySelector('html').style.removeProperty('--primary-bg-color');
			document.querySelector('html').style.removeProperty('--primary-bg-hover');
			document.querySelector('html').style.removeProperty('--primary-bg-border');
			document.querySelector('html').style.removeProperty('--secondary-color');
			document.querySelector('html').style.removeProperty('--secondary-hover');
			document.querySelector('html').style.removeProperty('--secondary-border');
			document.querySelector('html').style.removeProperty('--dark-primary');
			document.querySelector('html').style.removeProperty('--dark-secondary');

			// remove transparent theme properties
			localStorage.removeItem('transparentPrimary')
			localStorage.removeItem('transparentsecondary')
			localStorage.removeItem('transparentcolor')

			$('#myonoffswitch56').prop('checked', true);
			$('#myonoffswitch57').prop('checked', false);
			$('#myonoffswitch58').prop('checked', false);

			const root = document.querySelector(':root');
			root.style = "";
			names()
        } else {
			$('body').removeClass('light-theme');
			$('body').addClass('dark-theme');
         }
    });

	/***************** Light Theme End *********************/

	/***************** Dark Theme Start *********************/

	$('#myonoffswitch57').click(function() {
        if (this.checked) {
			$('body').addClass('dark-theme');
			$('body').removeClass('light-theme');
			$('body').removeClass('transparent-theme');

			// remove light theme properties
			localStorage.removeItem('primaryColor')
			localStorage.removeItem('primaryHoverColor')
			localStorage.removeItem('primaryBorderColor')
			localStorage.removeItem('secondaryColor')
			localStorage.removeItem('secondaryHoverColor')
			localStorage.removeItem('secondaryBorderColor')
			localStorage.removeItem('darkPrimary')
			localStorage.removeItem('darksecondary')
			document.querySelector('html').style.removeProperty('--primary-bg-color');
			document.querySelector('html').style.removeProperty('--primary-bg-hover');
			document.querySelector('html').style.removeProperty('--primary-bg-border');
			document.querySelector('html').style.removeProperty('--secondary-color');
			document.querySelector('html').style.removeProperty('--secondary-hover');
			document.querySelector('html').style.removeProperty('--secondary-border');
			document.querySelector('html').style.removeProperty('--dark-primary');
			document.querySelector('html').style.removeProperty('--dark-secondary');

			// remove transparent theme properties
			localStorage.removeItem('transparentPrimary')
			localStorage.removeItem('transparentsecondary')
			localStorage.removeItem('transparentcolor')

			// removing light theme data
			localStorage.removeItem('primaryColor')
			localStorage.removeItem('primaryHoverColor')
			localStorage.removeItem('primaryBorderColor')
			localStorage.removeItem('secondaryColor')
			localStorage.removeItem('secondaryHoverColor')
			localStorage.removeItem('secondaryBorderColor')

			$('#myonoffswitch57').prop('checked', true);
			$('#myonoffswitch56').prop('checked', false);
			$('#myonoffswitch58').prop('checked', false);
			const root = document.querySelector(':root');
			root.style = "";
			names()
        } else {
			$('body').addClass('light-theme');
			$('body').removeClass('dark-theme');
         }
    });

	/***************** Dark Theme End *********************/

	/***************** Transparent Theme Start *********************/

	$(document).on("click", '#myonoffswitch58', function () {
		if (this.checked) {
			$('body').addClass('transparent-theme');
			$('body').removeClass('dark-theme');
			$('body').removeClass('light-theme');

			// remove dark theme properties
			localStorage.removeItem('darkPrimary')
			localStorage.removeItem('darksecondary')
			// remove transparent theme properties
			localStorage.removeItem('transparentPrimary')
			localStorage.removeItem('transparentsecondary')
			localStorage.removeItem('transparentcolor')

			// remove light theme properties
			localStorage.removeItem('primaryColor')
			localStorage.removeItem('primaryHoverColor')
			localStorage.removeItem('primaryBorderColor')
			localStorage.removeItem('primaryTransparent')
			localStorage.removeItem('secondaryColor')
			localStorage.removeItem('secondaryHoverColor')
			localStorage.removeItem('secondaryTransparentColor')

			$('#myonoffswitch58').prop('checked', true);
			$('#myonoffswitch56').prop('checked', false);
			$('#myonoffswitch57').prop('checked', false);

			const root = document.querySelector(':root');
			root.style = "";
			names()
		} else {
			$('body').removeClass('transparent-theme');
			localStorage.removeItem("transparent-theme");
		}
		localStorageBackup()
	});

	/***************** Transparent Theme End *********************/

	/*Header Style 1*/
	$(document).on('click', '#myonoffswitch', function(e){
		if (this.checked) {
			$('body').addClass('default-header');
			$('body').removeClass('headerstyle1');
			$('body').removeClass('headerstyle2');
			localStorage.setItem("default-header", "True");
		}
		else {
			$('body').removeClass('default-header');
			localStorage.setItem("default-header", "false");
		}
	});

	/*Header Style 2*/
	$(document).on('click', '#myonoffswitch1', function(e){
		if (this.checked) {
			$('body').addClass('headerstyle1');
			$('body').removeClass('dark-header');
			$('body').removeClass('headerstyle2');
			localStorage.setItem("headerstyle1", "True");
		}
		else {
			$('body').removeClass('headerstyle1');
			localStorage.setItem("headerstyle1", "false");
		}
	});

	/*Header Style 3*/
	$(document).on('click', '#myonoffswitch2', function(e){
		if (this.checked) {
			$('body').addClass('headerstyle2');
			$('body').removeClass('default-header');
			$('body').removeClass('headerstyle1');
			localStorage.setItem("headerstyle2", "True");
		}
		else {
			$('body').removeClass('headerstyle2');
			localStorage.setItem("headerstyle2", "false");
		}
	});

	/*Horizontal-Menu Style 1*/
	$(document).on('click', '#myonoffswitch3', function(e){
		if (this.checked) {
			$('body').addClass('default-menu');
			$('body').removeClass('menu-style1');
			$('body').removeClass('menu-style2');
			localStorage.setItem("default-menu", "True");
		}
		else {
			$('body').removeClass('default-menu');
			localStorage.setItem("default-menu", "false");
		}
	});

	/*Horizontal-Menu Style 2*/
	$(document).on('click', '#myonoffswitch4', function(e){
		if (this.checked) {
			$('body').addClass('menu-style1');
			$('body').removeClass('default-menu');
			$('body').removeClass('menu-style2');
			localStorage.setItem("menu-style1", "True");
		}
		else {
			$('body').removeClass('menu-style1');
			localStorage.setItem("menu-style1", "false");
		}
	});

	/*Horizontal-Menu Style 3*/
	$(document).on('click', '#myonoffswitch5', function(e){
		if (this.checked) {
			$('body').addClass('menu-style2');
			$('body').removeClass('default-menu');
			$('body').removeClass('menu-style1');
			localStorage.setItem("menu-style2", "True");
		}
		else {
			$('body').removeClass('menu-style2');
			localStorage.setItem("menu-style2", "false");
		}
	});
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