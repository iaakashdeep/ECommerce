﻿var datatable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    datatable = $('#tblCompany').DataTable({
        "ajax": {
            url: '/admin/company/getcompany'
        },
        "columns": [
            { data: 'name', "width": "20%" },
            { data: 'city', "width": "10%" },
            { data: 'state', "width": "10%" },
            { data: 'phoneNumber', "width": "15%" },
            { data: 'postalCode', "width": "15%" },
            {
                data: 'id',
                "render": function (data) {
                    return ` <div class="w-75 btn-group" role="group">
                        <a href="/admin/company/upsert?companyId=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i> Edit</a>

                        <a onClick=Delete('/admin/company/delete?companyId=${data}') class="btn btn-danger mx-2"><i class="bi bi-trash3-fill"></i> Delete</a>
                    </div>
                    `
                },
                "width": "25%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    datatable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    })
}