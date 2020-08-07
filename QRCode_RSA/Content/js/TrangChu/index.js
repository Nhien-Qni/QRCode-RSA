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
        data: { "MaDaiBieu": $("#txt_barcode").val() },
        success: function (data) {
            if (data.Data) {
                toastr.success(data.Message);
                $("#HinhAnhDaiBieu").html(data.daiBieu.Avatar).attr({ "src": data.daiBieu.Avatar == null ? "/images/person.png" : data.daiBieu.Avatar });
                $("#HoTen").html(data.daiBieu.HoTen)
                $("#ChucVu").html(data.daiBieu.ChucVu)
                $("#DonVi").html(data.daiBieu.DonVi)
                $("#ViTriGhe").html(data.daiBieu.SoGhe)
                $('#txt_barcode').val("");
                focustb();
            }
            else {
                toastr.error(data.Message);
                $("#HinhAnhDaiBieu").attr({ "src": "/images/person.png" });
                $("#HoTen").html("")
                $("#ChucVu").html("")
                $("#ViTriGhe").html("")
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
