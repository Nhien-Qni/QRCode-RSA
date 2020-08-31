function isEmpty(obj) {
    for (let key in obj) {
        if (obj.hasOwnProperty(key))
            return false;
    }
    return true;
}

function translate(key, params = {}) {
    var keyContent = {
        btn_them: "Thêm mới",
        cap_nhat: "Cập nhật",
        cap_nhat_thanh_cong: "Cập nhật thành công",
        export_thanh_cong: "Xuất báo cáo thành công",
        khoi_phuc_thanh_cong: "Khôi phục thành công",
        them_moi: "Thêm mới",
        them_thanh_cong: "Thêm thành công",
        xoa_thanh_cong: "Xóa thành công",
        khong_co_file: "Không có file",
        chua_co_chi_bo: "Chưa có chi bộ hiện tại",
        loi_xay_ra: "Đã có lỗi xảy ra vui lòng tải lại trang. <p><a href='javascript: window.location.reload(true)'><i class='fa fa-refresh'></i> Tải lại trang.</a></p>",
        bs_action_update: "<button id='btn-update-item' class='btn btn-primary btn-sm m-l-5 m-r-5 m-t-5 m-b-5'><i class='fa fa-pencil'></i> Sửa</button>",
        bs_action_delete: "<button id='btn-delete-item' class='btn btn-danger btn-sm m-l-5 m-r-5 m-t-5 m-b-5'><i class='fa fa-trash'></i> Xóa</button>",
        bs_management_action: "<button id='btn-update-item' class='btn btn-primary btn-sm m-l-5 m-r-5 m-t-5 m-b-5'><i class='fa fa-pencil'></i> Sửa</button>"
            + "<button id='btn-delete-item' class='btn btn-danger btn-sm m-l-5 m-r-5 m-t-5 m-b-5'><i class='fa fa-trash'></i> Xóa</button>",
        bs_management_action_delete_only: "<button id='btn-delete-item' class='btn btn-danger btn-sm m-l-5 m-r-5 m-t-5 m-b-5'><i class='fa fa-trash'></i> Xóa</button>",
        bs_management_action_QHGD: "<button id='btn-update-item-QHGD' class='btn btn-primary btn-sm m-l-5 m-r-5 m-t-5 m-b-5'><i class='fa fa-pencil'></i> Sửa</button>"
            + "<button id='btn-delete-item-QHGD' class='btn btn-danger btn-sm m-l-5 m-r-5 m-t-5 m-b-5'><i class='fa fa-trash'></i> Xóa</button>",
        bs_management_action_QTDH: "<button id='btn-update-item-QTDH' class='btn btn-primary btn-sm m-l-5 m-r-5 m-t-5 m-b-5'><i class='fa fa-pencil'></i> Sửa</button>"
            + "<button id='btn-delete-item-QTDH' class='btn btn-danger btn-sm m-l-5 m-r-5 m-t-5 m-b-5'><i class='fa fa-trash'></i> Xóa</button>",
        bs_management_action_QTCT: "<button id='btn-update-item-QTCT' class='btn btn-primary btn-sm m-l-5 m-r-5 m-t-5 m-b-5'><i class='fa fa-pencil'></i> Sửa</button>"
            + "<button id='btn-delete-item-QTCT' class='btn btn-danger btn-sm m-l-5 m-r-5 m-t-5 m-b-5'><i class='fa fa-trash'></i> Xóa</button>",
        bs_management_action_CapUy: "<button id='btn-update-item-CapUy' class='btn btn-primary btn-sm m-l-5 m-r-5 m-t-5 m-b-5'><i class='fa fa-pencil'></i> Sửa</button>"
            + "<button id='btn-delete-item-CapUy' class='btn btn-danger btn-sm m-l-5 m-r-5 m-t-5 m-b-5'><i class='fa fa-trash'></i> Xóa</button>",
    }

    let keyContentNew = keyContent[key] ? keyContent[key] : key;
    if (!isEmpty(params) && keyContent[key]) {
        for (let k in params) {
            if (params.hasOwnProperty(k)) {
                let find = `{${k}}`;
                var replaceStr = new RegExp(find, 'g');

                keyContentNew = keyContent[key].replace(replaceStr, params[k]);
            }
        }
        return keyContentNew;
    }

    return keyContentNew;
}