﻿var datatable;
$(document).ready(function () {
    var url = window.location.search;       //To get the current URL
    if (url.includes("inprocess")) {
        loadDataTable("inprocess");
    }
    else if (url.includes("pending")) {
        loadDataTable("pending");
    }
    else if (url.includes("completed")) {
        loadDataTable("completed");
    }
    else if (url.includes("approved")) {
        loadDataTable("approved");
    }
    else {
        loadDataTable("all");
    }
    
});

function loadDataTable(status) {
    datatable = $('#tblOrder').DataTable({
        "ajax": {
            url: '/admin/order/getorders?status='+status
        },
        "columns": [
            { data: 'id', "width": "10%" },
            { data: 'name', "width": "15%" },
            { data: 'phoneNumber', "width": "15%" },
            { data: 'applicationUser.email', "width": "20%" },
            { data: 'orderStatus', "width": "10%" },
            { data: 'orderTotal', "width": "10%" },
            //{ data: 'id', "width": "10%" },
            {
                data: 'id', 
                "render": function (data) {
                    return ` <div class="w-75 btn-group" role="group">
                        <a href="/admin/order/details?orderId=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i> </a>
                    </div>
                    `
                },
                "width": "25%"
            }
            //The number of columns here can not be exceed than <th> of table
        ]
    });
}
