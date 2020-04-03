"use strict";
// Class definition

var PayoutsDatatable = function () {
	// Private functions

	// demo initializer
	let payouts = function () {

		$('.select2').select2({
			theme: "bootstrap",
			width: '100%',
			placeholder: 'Select an option'
		});

		$('.datepicker').datetimepicker({
			format: 'MM/DD/YYYY',
		});

		let datatable = $('#payouts-datatable').DataTable({
			responsive: true,
			lengthChange: true,
			processing: true,
			searching: true,
			language: {
				processing: '<div class="spinner-border text-dark" role="status"></div>',
				emptyTable: "No collection form available at the moment"
			},
			serverSide: true,
			ajax: {
				url: `/payouts/forms`,
				data: {},
				dataFilter: function (data) {
					var json = jQuery.parseJSON(data);
					json.recordsTotal = json.totalCount;
					json.recordsFiltered = json.totalCount;
					json.data = json.forms;

					return JSON.stringify(json); // return JSON string
				}
			},
			order: [[0, 'asc']],
			columns: [
				{ data: "id" },
				{ data: "dateCreated" },
				{ data: "name" },
				{ data: "businessid" },
				{ data: "status" },
				{ data: 'Actions', responsivePriority: -1 },
			],
			columnDefs: [
				{
					targets: -5,
					render: function (data, type, row) {
						if (type === "sort" || type === "type") {
							return data;
						}
						return moment(data).format('LLL');
					}
				},
				{
					// hide columns by index number
					targets: [-6],
					visible: false,
				},
				{
					targets: -1,
					title: 'Action',
					autoHide: false,
					render: function (data, type, full, meta) {
						return `<div class="form-button-action">
                                            <button type="button" data-toggle="tooltip" data-view-record-id="${full.id}" class="btn btn-link btn-primary" data-original-title="View form details" style="margin:0 15px 0 0;padding:0;">
                                                <i class="fa fa-eye"></i>
                                            </button>
                                            <button type="button" data-toggle="tooltip" data-edit-record-id="${full.id}" class="btn btn-link btn-primary" data-original-title="Edit form details" style="margin:0 15px 0 0;padding:0;">
                                                <i class="fa fa-edit"></i>
                                            </button>
                                            <button type="button" data-toggle="tooltip" data-delete-record-id="${full.id}" class="btn btn-link btn-danger" data-original-title="Remove" style="padding:0;">
                                                <i class="fa fa-times"></i>
                                            </button>
                                        </div>`;
					}
				}
			]
		});

		datatable.on('click', '[data-view-record-id]', (e) => {
			e.preventDefault();
			XenteApp.block($('#payouts-datatable'));
			let formId = e.currentTarget.getAttribute('data-view-record-id');
			fetch(`/payouts/forms/${formId}`).then((response) => response.text()).then((data) => {
				$('#payout-form-details-container').html(data);
				XenteApp.unblock($('#collections-datatable'));
				$('#payout-form-details-modal').modal({
					backdrop: 'static',
					Keyboard: false
				});

				// reload table on details modal close
				$('#view_collection_form_modal').on('hidden.bs.modal', () => {
					datatable.ajax.reload();
				});

			});
		});

		datatable.on('click', '[data-delete-record-id]', (e) => {
			e.preventDefault();

			let formId = e.currentTarget.getAttribute('data-delete-record-id');

			const swalWithBootstrapButtons = Swal.mixin({
				customClass: {
					confirmButton: 'btn btn-outline-primary waves-effect waves-light btn-sm mr-2',
					cancelButton: 'btn btn-outline-danger waves-effect waves-light btn-sm',
					title: 'sweet-alert-title',
					content: 'sweet-alert-text'
				},
				buttonsStyling: false
			});


			swalWithBootstrapButtons.fire({
				title: 'Are you sure?',
				text: "You won't be able to revert this!",
				icon: 'warning',
				backdrop: true,
				allowOutsideClick: false,
				showCancelButton: true,
				confirmButtonColor: '#3085d6',
				cancelButtonColor: '#d33',
				confirmButtonText: 'Yes, delete it!',
				padding: '1rem',
				width: '20rem'
			}).then((result) => {
				if (result.value) {
					fetch(`/payouts/forms/${formId}`, {
						method: 'DELETE'
					}).then((response) => response.json())
						.then((data) => {
							if (data.status === 'success') {
								datatable.ajax.reload();
								swalWithBootstrapButtons.fire({
									title: 'Deleted!',
									text: "Payout form has been deleted.",
									icon: 'success',
									padding: '1rem',
									width: '20rem'
								});
							} else {
								swalWithBootstrapButtons.fire({
									title: 'Failed!',
									text: data.message,
									icon: 'error',
									padding: '1rem',
									width: '20rem'
								});
							}
						});
				}
			})
		});


	}

	return {
		// Public functions
		init: function () {
			// init payments
			payouts();
		},
	};
}();

const PayoutAdd = function () {

	const addPayout = () => {
		$('#add-payout-button').click(() => {
			$('#addPayoutModal').modal({
				backdrop: 'static',
				keyboard: false
			});
		});

		document.getElementById('add-payout-form').addEventListener('submit', (e) => {
			e.preventDefault();
			const form = document.addpayoutform;
			if (form.checkValidity() === false) {
				e.stopPropagation();
				form.classList.add('was-validated');
				return;
			}

			form.classList.add('was-validated');
			$("#add-payout-submit-button").attr("disabled", true);
			$('#add-payout-submit-button').html(`<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Processing...`);

			const formData = new URLSearchParams();
			for (const pair of new FormData(form)) {
				formData.append(pair[0], pair[1]);
			}

			fetch('/payouts/forms/add', {
				method: 'POST',
				body: formData

			}).then((response) => response.json()).
				then((data) => {
					if (!!data) {
						if (data.status == 'success') {
							form.reset();
						} else {

						}

					} else {

					}
					$("#add-payout-submit-button").attr("disabled", false);
					$('#add-payout-submit-button').html(`Add Payout Form`);

				});
		});
	};

	return {
		init: () => {
			addPayout();
		}
	}
}();

$(document).ready(function () {
	if (document.getElementById('payouts-datatable') != null) {
		PayoutsDatatable.init();
		PayoutAdd.init();
	}

});
