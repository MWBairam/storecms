//Ajax Show in PopUp, then AddOrEdit Post method, and Delete method:
//used in Index view.

//This is used to call the AddOrEdit HttpGet method in TransactionController which returns the AddOrEdit.cshtml view,
//this returned view will be shown as bootstrap pop up in the div written in _UILayout.cshtml for that purpose.
showInPopup = (url, title) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {
            $('#form-modal .modal-body').html(res);
            $('#form-modal .modal-title').html(title);
            $('#form-modal').modal('show');
            // to make popup draggable
            $('.modal-dialog').draggable({
                handle: ".modal-header"
            });
        },
        error: function (err) {
            //do nothing here, ~/js/ajaxErrorHandler will handle the error message and display an error notification.
        }
    })
}

//Then after filling the data in the above view in the popup, send them in post request to AddOrEdit HttpPost method in Controller:
jQueryAjaxPost = form => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    $('#view-all').html(res.html)  //view-all is a div in Index.cshtml to carry the table.
                    $('#form-modal .modal-body').html('');
                    $('#form-modal .modal-title').html('');
                    $('#form-modal').modal('hide');
                    //display successfully submitted using notify.j existed in lib/notify.js or using toastr library:
                    //$.notify('Successfully Submitted', { globalPosition: 'top-center', className: 'success' });
                    toastr.success('Successfully Submitted', '', { timeOut: 3000 });
                    convertToDataTable(); //after request is successfull, Datatable features will be erased because controller method returns partial view of the table, so convert it again to datatable
                }
                else
                    //the result is isInvalid, and the controller method will return the model and the partial view with the validation/result errors.
                    $('#form-modal .modal-body').html(res.html);
            },
            //in case we returned http errors:
            error: function (err) {
                //do nothing here, ~/js/ajaxErrorHandler will handle the error message and display an error notification.
            }
        })
        //to prevent default form submit event
        return false;
    } catch (ex) {
        console.log(ex)
    }
}

//post requests to Delete method in Controller:
jQueryAjaxDelete = form => {
    Swal.fire(
        {
            title: 'Are you sure?',
            text: 'You wont be able to revert this ...',
            icon: 'warning',
            showCancelButton: true,
            confirmationButtonColor: '#3085dc6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, go ahead.',
            inputType: "submit",
            closeOnConfirm: false,
        }).then((result) => {
            if (result['isConfirmed']) {
                try {
                    $.ajax({
                        type: 'POST',
                        url: form.action,
                        data: new FormData(form),
                        contentType: false,
                        processData: false,
                        success: function (res) {
                            $('#view-all').html(res.html);
                            Swal.fire('', 'Deleted Successfully !', 'success');
                            convertToDataTable(); //after request is successfull, Datatable features will be erased because controller method returns partial view of the table, so convert it again to datatable
                        },
                        error: function (err) {
                            //do nothing here, ~/js/ajaxErrorHandler will handle the error message and display an error notification.
                        }
                    })
                } catch (ex) {
                    console.log(ex)
                }
            }
        })

    //prevent default form submit event
    return false;
}





//function to prepare the DataTable.
//Backend Controller will send us the full table, here will be paginated, with search and sort features.
//Refrences:
//https://datatables.net/examples/styling/bootstrap4
//https://datatables.net/examples/api/regex.html
//https://datatables.net/examples/api/counter_columns.html
//https://www.youtube.com/watch?v=yGBk9Nalyq8
//https://www.youtube.com/watch?v=uhdxAlzbiks&list=RDCMUCdhV8d9wLRI1WUB4pnV0Row&index=3
//https://datatables.net/extensions/buttons/examples/styling/bootstrap4.html
//https://stackoverflow.com/questions/36544644/datatables-dom-positioning
//https://datatables.net/reference/option/dom
function convertToDataTable() {
    //In _ViewAll we added another table above our table to prepare search functionality,
    //here do the jquery part:
    function filterGlobal() {
        $('#DataTable').DataTable().search(
            $('#global_filter').val(),
            $('#global_regex').prop('checked'),
            $('#global_smart').prop('checked')
        ).draw();
    }

    function filterColumn(i) {
        $('#DataTable').DataTable().column(i).search(
            $('#col' + i + '_filter').val(),
            $('#col' + i + '_regex').prop('checked'),
            $('#col' + i + '_smart').prop('checked')
        ).draw();
    }


    //Prepare the DataTable with custome settings we want:
    var table = $('#DataTable').DataTable({
        //use dom to position the different element: l:length-control,B:export Buttons, f:filter or search, t:table, i:summary, p:pagination
        "dom": "<'row'<'col-sm-3'l><'col-sm-5 text-center'B><'col-sm-4'f>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-6'i><'col-sm-6'p>>",
        //define pagination type:
        pagingType: 'full_numbers',
        //generate row number, with ordering it from 1:
        //"columnDefs": [{
        //    "width": "auto", //fit the "row number column width to 10%.
        //    "targets": [0, 1] //also the column where we have the buttons, make the width smaller.
        //}],
        "order": [[2, 'asc']], //order according to first column where data is existed, it is after the row number and columns of buttons.
        "autoWidth": true, //fit the width of other columns to its appropriate size. 
        "columnDefs": [{
            "searchable": false,
            "orderable": false,
            "targets": [0, 1] //make also the column where we have the buttons and the first cololumn where we have row numbers, unsearchable and unsortable.
        }],
        lengthChange: true,
        buttons: ['copy', 'csv', 'excel', 'pdf', 'colvis'], //export buttons
        select: true //allow export of teh selected rows only
    });

    //now draw the table, while mentioning that the row number will be reset with each search operation to start again from 1:
    table.on('order.dt search.dt', function () {
        table.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();


    //append the search functionality:
    //also, the advanced search table is hidden, will be shown once we click on advanced search, the code for that is also below:
    $('input.global_filter').on('keyup click', function () {
        filterGlobal();
    });
    $('input.column_filter').on('keyup click', function () {
        filterColumn($(this).parents('tr').attr('data-column'));
    });
    $('#AdvancedSearchButton').on('click', function () {
        if ($("#AdvancedSearchTable").hasClass('hide')) {
            $("#AdvancedSearchTable").removeClass('hide');
        }
        else {
            $("#AdvancedSearchTable").addClass('hide');
        }

    });

    //append the export buttons:
    table.buttons().container().appendTo('#DataTable_wrapper .col-md-6:eq(0)');
}

//call the previous method upon html doc load,
//also it is called in the above ajax methods agter the Post request are done.
$(document).ready(function () {
    convertToDataTable();
})
