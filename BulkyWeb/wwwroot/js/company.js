﻿$(document).ready(function () {
    loadDataTable();
});

var dataTable;
function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/admin/company/getall',
            dataSrc: 'data'  // Your response is wrapped in a 'data' object, so this is correct
        },
        "columns": [
            { data: 'name', "width": "15%" },  // Matching 'name' property
            { data: 'streetAddress', "width": "15%" },  // Matching 'streetAddress' property
            { data: 'city', "width": "15%" },  // Matching 'city' property
            { data: 'state', "width": "15%" },  // Matching 'state' property
            { data: 'phoneNumber', "width": "15%" },  // Matching 'phoneNumber' property
            {
                data: 'id',
                render: function (data) {
                    return `<div class="w-75 btn-group" role="group">
                              <a href="/admin/company/upsert?id=${data}" class="btn btn-primary mx-1"><i class="bi bi-pencil-square"></i>Edit</a>
                              <a onClick=Delete('/admin/company/delete/${data}') class="btn btn-danger mx-1"><i class="bi bi-trash-fill"></i>Delete</a>
                            </div>`;
                },
                "width": "25%"
            }
        ]
    });
}

$.ajax({
    url: '/admin/company/getall',
    method: 'GET',
    success: function (response) {
        console.log(response);  // Log the response to ensure data structure is correct
    }
});


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
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    });
}