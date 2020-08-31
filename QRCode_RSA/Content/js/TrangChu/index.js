"use strict";

focustb();
function QuetMa() {
    if ($("#txt_barcode").val() == "" || $("#txt_barcode").val().trim() == "") {
        return;
    }
    $.ajax({
        url: '/TrangChu/QuetMa',
        type: "POST",
        dataType: "json",
        data: { "key": $("#txt_barcode").val() },
        success: function (data) {
            if (data != null && data.isError == null) {
                toastr.success(data.isSuccess);
                //$("#HinhAnh").html(data.Avatar).attr({ "src": data.Avatar == null ? "/images/person.png" : data.Avatar });
                //$("#HoTen").html(data.HoTen)
                //$("#NoiCuTru").html(data.NoiCuTru)
                //$("#QuocGia").html(data.QuocGia)
                $('#txt_barcode').val("");
                focustb();
            }
            else {
                toastr.error(data.isError);
                $("#HinhAnh").attr({ "src": "/images/person.png" });
                $("#HoTen").html("")
                $("#NoiCuTru").html("")
                $("#QuocGia").html("")
                $('#txt_barcode').val("");
                focustb();
            }
        },
        error: function (ex) {
            console.log(ex)
            toastr.error(translate("loi_xay_ra"));
            focustb();
        }
    })
}

function debounce(func, wait, immediate) {
    let timeout;
    return function () {
        let context = this, args = arguments;
        let later = function () {
            timeout = null;
            if (!immediate) func.apply(context, args);
        };
        let callNow = immediate && !timeout;
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
        if (callNow) func.apply(context, args);
    };
};
function focustb()
{
    $("#txt_barcode").focus();
}
//setInterval(focustb, 5000);
$("#txt_barcode").bind("change keyup", debounce(function () {
    QuetMa();
    focustb();
}, 1000));
