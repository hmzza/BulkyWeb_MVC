﻿$(document).ready(function () {
    loadDataTable();
});

var dataTable;
function loadDataTable(){
    dataTable = $('#tblData').DataTable({
        "ajax": { 
            url: '/admin/product/getall',
            dataSrc: 'data' // Ensure 'data' property is being accessed correctly
        },
        "columns": [
            //column count here must match with Index page columns
            { data: 'title', "width": "25%" },
            { data: 'isbn', "width": "15%" },
            { data: 'listPrice', "width": "10%" },
            { data: 'author', "width": "15%" },
            { data: 'category.name', "width": "10%" },
            {
                data: 'id',
                render: function (data) {
                    return `<div class="w-75 btn-group" role="group">
                              <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-1"><i class="bi bi-pencil-square"></i>Edit</a>
                              <a onClick=Delete('/admin/product/delete/${data}') class="btn btn-danger mx-1"><i class="bi bi-trash-fill"></i>Delete</a>
                            </div>`
                },
                "width": "25%"
            }
        ]
    });
}
$.ajax({
    url: '/admin/product/getall',
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