$(document).ready(function () {
    loadDataTable();
});


function loadDataTable(){
    dataTable = $('#tblData').DataTable({
        "ajax": { 
            url: '/admin/product/getall',
            dataSrc: 'data' // Ensure 'data' property is being accessed correctly
        },
        "columns": [
            { data: 'title', "width": "25%" },
            { data: 'isbn', "width": "15%" },
            { data: 'listPrice', "width": "10%" },
            { data: 'author', "width": "20%" },
            { data: 'category.name', "width": "15%" }
            //column count here must match with Index page columns
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
