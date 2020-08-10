import $ from "jquery";
import "bootstrap";

$(() => {
	if (window.location.hash) {
		const target = $(window.location.hash);

		if (target.hasClass("collapse"))
			target.collapse("show");
	}

	$("[data-modal-initially-open='true']").modal("show");
});