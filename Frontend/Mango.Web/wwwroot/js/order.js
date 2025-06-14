var datatable;

$(document).ready(function () {
    loadDataTable()
})

function loadDataTable() {

    $('#tblData').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            "url": "/orders/GetOrders",
            "type": "POST"
        },
        "columns": [
            { "data": "orderId","width":"5%" },
            { "data": "email", "width": "25%" },
            { "data": "name", "width": "20%" },
            { "data": "phone", "width": "10%" },
            { "data": "status" },
            {
                "data": "orderHeaderId",
                "render": function (data) {
                    return `<div class="text-centerw-75 btn-group" role="group">
                                <a href="/orders/orderDetail?orderId=${data}" class="btn btn-primary text-white" style="cursor:pointer;">
                                    <i class="bi bi-pencil-square"></i> Details
                                </a>
                            </div>`;
                }
            }
        ]
    });
}