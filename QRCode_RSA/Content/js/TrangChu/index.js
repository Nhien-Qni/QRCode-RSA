"use strict";

focustb();
$("#txt_barcode").keyup(function (event) {
    if (event.keyCode == 13) {
        QuetMa();
    }
});
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
                // Get the modal
                var modal = document.getElementById("myModal");

                // Get the image and insert it inside the modal - use its "alt" text as a caption
                var modalImg = document.getElementById("img01");
                //var captionText = document.getElementById("caption");
                modal.style.display = "block";
                modalImg.src = data.DuLieu.savefilename;
                //captionText.innerHTML = this.alt;

                // Get the <span> element that closes the modal
                var span = document.getElementsByClassName("close")[0];

                // When the user clicks on <span> (x), close the modal
                span.onclick = function () {
                    modal.style.display = "none";
                }
                toastr.success(data.isSuccess);
                $("#HinhAnh").html(data.DuLieu.Avatar).attr({ "src": data.DuLieu.Avatar == null ? "/images/person.png" : data.DuLieu.Avatar });
                $("#HoTen").html(data.DuLieu.HoTen);
                $("#NgaySinh").html(data.DuLieu.NgaySinhString);
                $("#XepLoai").html(data.DuLieu.XepLoai);
                $("#HinhThuc").html(data.DuLieu.HinhThuc);
                $("#NganhDaoTao").html(data.DuLieu.NganhDaoTao);
                $("#GioiTinh").html(data.DuLieu.GioiTinh == true ? "Nam" : "Nữ");

                $("#SoHieu").html(data.DuLieu.SoHieu);
                $("#SoBangCap").html(data.DuLieu.SoBangCap);
                $('#txt_barcode').val("");
                focustb();
            }
            else {
                toastr.error(data.isError);
                $("#HinhAnh").attr({ "src": "/images/person.png" });
                $("#NgaySinh").html("");
                $("#HoTen").html("");
                $("#SoHieu").html("");
                $("#SoBangCap").html("");
                $("#XepLoai").html("");
                $("#HinhThuc").html("");
                $("#NganhDaoTao").html("");
                $("#GioiTinh").html("");
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
function focustb() {
    $("#txt_barcode").focus();
}
setInterval(focustb, 5000);
//$("#txt_barcode").bind("change keyup", debounce(function () {
//    QuetMa();
//    focustb();
//}, 1000));
$("#txt_barcode").on("change", debounce(function () {
    QuetMa();
    focustb();
}));