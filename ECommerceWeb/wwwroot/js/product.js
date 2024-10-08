﻿var datatable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    datatable = $('#tblProduct').DataTable({
        "ajax": {
            url: '/admin/product/getproducts'
        },
        "columns": [
            { data: 'title', "width": "20%" },
            { data: 'isbn', "width": "15%" },
            { data: 'author', "width": "15%" },
            { data: 'listPrice', "width": "10%" },
            { data: 'category.name', "width": "10%" },
            //{ data: 'id', "width": "10%" },
            {
                data: 'id', 
                "render": function (data) {
                    return ` <div class="w-75 btn-group" role="group">
                        <a href="/admin/product/upsert?productId=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i> Edit</a>

                        <a onClick=Delete('/admin/product/delete?productId=${data}') class="btn btn-danger mx-2"><i class="bi bi-trash3-fill"></i> Delete</a>
                    </div>
                    `
                },
                "width": "25%"
            }
            //The number of columns here can not be exceed than <th> of table
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
