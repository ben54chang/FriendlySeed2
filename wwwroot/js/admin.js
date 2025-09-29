$(document).ready(function () {
    // Initialize sidebar menu using Bootstrap collapse
    $('.nav-sidebar .nav-item.has-treeview > .nav-link').on('click', function(e) {
        e.preventDefault();
        
        var $this = $(this);
        var $parent = $this.parent();
        var $submenu = $parent.find('.nav-treeview');
        
        // Close other open menus
        $('.nav-sidebar .nav-item.has-treeview').not($parent).removeClass('menu-open');
        $('.nav-sidebar .nav-item.has-treeview').not($parent).find('.right.fas').removeClass('fa-angle-down').addClass('fa-angle-left');
        $('.nav-sidebar .nav-treeview').not($submenu).collapse('hide');
        
        // Toggle current submenu
        if ($parent.hasClass('menu-open')) {
            $parent.removeClass('menu-open');
            $submenu.collapse('hide');
            $this.find('.right.fas').removeClass('fa-angle-down').addClass('fa-angle-left');
        } else {
            $parent.addClass('menu-open');
            $submenu.collapse('show');
            $this.find('.right.fas').removeClass('fa-angle-left').addClass('fa-angle-down');
        }
    });
    
    // Initialize LTEAdmin components if available
    if (typeof $.AdminLTE !== 'undefined') {
        $.AdminLTE.layout.activate();
    }

    // Auto-hide alerts after 5 seconds
    setTimeout(function() {
        $('.alert').fadeOut('slow');
    }, 5000);

    // Initialize Toastr
    if (typeof toastr !== 'undefined') {
        toastr.options = {
            "closeButton": true,
            "debug": false,
            "newestOnTop": false,
            "progressBar": true,
            "positionClass": "toast-top-right",
            "preventDuplicates": false,
            "onclick": null,
            "showDuration": "300",
            "hideDuration": "1000",
            "timeOut": "5000",
            "extendedTimeOut": "1000",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        };
    }

    // Confirm delete actions with SweetAlert2
    $('.btn-danger').on('click', function(e) {
        e.preventDefault();
        var url = $(this).attr('href') || $(this).data('url');
        
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                title: '確定要刪除嗎？',
                text: "此操作無法復原！",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: '是的，刪除！',
                cancelButtonText: '取消'
            }).then((result) => {
                if (result.isConfirmed) {
                    if (url) {
                        window.location.href = url;
                    } else {
                        $(this).closest('form').submit();
                    }
                }
            });
        } else {
            if (confirm('確定要刪除這筆資料嗎？此操作無法復原。')) {
                if (url) {
                    window.location.href = url;
                } else {
                    $(this).closest('form').submit();
                }
            }
        }
    });

    // DataTable initialization for all tables
    if ($.fn.DataTable) {
        $('.table').each(function() {
            var $table = $(this);
            var tableId = $table.attr('id');
            
            // 檢查表格是否已經初始化
            if ($.fn.DataTable.isDataTable('#' + tableId)) {
                return; // 跳過已初始化的表格
            }
            
            // 檢查表格是否有資料行
            var rowCount = $table.find('tbody tr').length;
            var headerCount = $table.find('thead th').length;
            
            console.log('初始化表格:', tableId, '行數:', rowCount, '欄位數:', headerCount);
            
            // 檢查第一行資料的欄位數
            var firstRow = $table.find('tbody tr:first');
            var firstRowCells = firstRow.find('td').length;
            console.log('第一行資料欄位數:', firstRowCells);
            
            // 如果沒有資料行，跳過初始化
            if (rowCount === 0) {
                console.log('表格沒有資料行，跳過 DataTable 初始化:', tableId);
                return;
            }
            
            // 檢查欄位數是否一致
            if (headerCount !== firstRowCells) {
                console.error('表格欄位數不一致:', tableId, '標題欄位數:', headerCount, '資料欄位數:', firstRowCells);
                return;
            }
            
            // 基本設定
            var config = {
                "language": {
                    "sProcessing": "處理中...",
                    "sLengthMenu": "顯示 _MENU_ 項結果",
                    "sZeroRecords": "沒有匹配結果",
                    "sInfo": "顯示第 _START_ 至 _END_ 項結果，共 _TOTAL_ 項",
                    "sInfoEmpty": "顯示第 0 至 0 項結果，共 0 項",
                    "sInfoFiltered": "(由 _MAX_ 項結果過濾)",
                    "sInfoPostFix": "",
                    "sSearch": "搜尋:",
                    "sUrl": "",
                    "sEmptyTable": "表中數據為空",
                    "sLoadingRecords": "載入中...",
                    "sInfoThousands": ",",
                    "oPaginate": {
                        "sFirst": "首頁",
                        "sPrevious": "上頁",
                        "sNext": "下頁",
                        "sLast": "末頁"
                    },
                    "oAria": {
                        "sSortAscending": ": 以升序排列此列",
                        "sSortDescending": ": 以降序排列此列"
                    }
                },
                "pageLength": 25,
                "responsive": true,
                "autoWidth": false,
                "columnDefs": [
                    { "orderable": false, "targets": -1 } // Disable ordering on last column (actions)
                ]
            };
            
            // 針對特定表格的設定
            if (tableId === 'menuTable') {
                config.order = [[1, "asc"]]; // 按排序欄位排序
                config.columnDefs = [
                    { "orderable": false, "targets": 4 } // 操作欄位不可排序
                ];
                // Menu 表格也加入匯出功能
                config.dom = 'Bfrtip';
                config.buttons = [
                    {
                        extend: 'excel',
                        text: '匯出 Excel',
                        className: 'buttons-excel btn btn-success btn-sm',
                        title: '選單管理資料',
                        exportOptions: {
                            columns: [0, 1, 2, 3] // 排除操作欄位
                        }
                    }
                ];
                // 禁用分頁，因為是樹狀結構
                config.paging = false;
                // 禁用搜尋，因為是樹狀結構
                config.searching = false;
            } else if (tableId === 'categoryTable') {
                // 文章分類表格：按狀態排序（啟用在前），然後按排序欄位
                config.order = [[4, "desc"], [3, "asc"]]; // 狀態欄位(4)降序，排序欄位(3)升序
                config.columnDefs = [
                    { "orderable": false, "targets": 6 } // 操作欄位不可排序
                ];
                config.dom = 'Bfrtip';
                config.buttons = [
                    {
                        extend: 'excel',
                        text: '匯出 Excel',
                        className: 'btn btn-success btn-sm',
                        title: '文章分類管理資料',
                        exportOptions: {
                            columns: [0, 1, 2, 3, 4, 5] // 排除操作欄位
                        }
                    }
                ];
            } else if (tableId === 'articleTable') {
                // 文章表格：按發布時間降序排列
                config.order = [[3, "desc"]]; // 按發布時間降序排列
                config.columnDefs = [
                    { "orderable": false, "targets": 6 } // 操作欄位不可排序
                ];
                config.dom = 'Bfrtip';
                config.buttons = [
                    {
                        extend: 'excel',
                        text: '匯出 Excel',
                        className: 'btn btn-success btn-sm',
                        title: '文章列表',
                        exportOptions: {
                            columns: [0, 1, 2, 3, 4, 5] // 排除操作欄位
                        }
                    }
                ];
            } else if (tableId === 'teacherTable') {
                // 教師表格：按建立時間降序排列
                config.order = [[0, "desc"]]; // 按ID降序排列
                config.columnDefs = [
                    { "orderable": false, "targets": 10 } // 操作欄位不可排序
                ];
                config.dom = 'Bfrtip';
                config.buttons = [
                    {
                        extend: 'excel',
                        text: '匯出 Excel',
                        className: 'btn btn-success btn-sm',
                        title: '教師列表',
                        exportOptions: {
                            columns: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9] // 排除操作欄位
                        }
                    }
                ];
            } else if (tableId === 'userTable') {
                // 使用者表格：按ID降序排列
                config.order = [[0, "desc"]]; // 按ID降序排列
                config.columnDefs = [
                    { "orderable": false, "targets": 7 } // 操作欄位不可排序
                ];
                config.dom = 'Bfrtip';
                config.buttons = [
                    {
                        extend: 'excel',
                        text: '匯出 Excel',
                        className: 'btn btn-success btn-sm',
                        title: '使用者列表',
                        exportOptions: {
                            columns: [0, 1, 2, 3, 4, 5, 6] // 排除操作欄位
                        }
                    }
                ];
            } else {
                // 其他表格加入匯出功能
                config.dom = 'Bfrtip';
                config.buttons = [
                    {
                        extend: 'excel',
                        text: '匯出 Excel',
                        className: 'btn btn-success btn-sm'
                    }
                ];
            }
            
            try {
                $table.DataTable(config);
                console.log('表格初始化成功:', tableId);
            } catch (error) {
                console.error('表格初始化失敗:', tableId, error);
            }
        });
    }

    // Form validation
    $('form').on('submit', function() {
        var isValid = true;
        $(this).find('[required]').each(function() {
            if (!$(this).val()) {
                $(this).addClass('is-invalid');
                isValid = false;
            } else {
                $(this).removeClass('is-invalid');
            }
        });
        return isValid;
    });

    // Remove validation classes on input
    $('input, select, textarea').on('input change', function() {
        $(this).removeClass('is-invalid');
    });

    // Tooltip initialization (Bootstrap 5)
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Popover initialization (Bootstrap 5)
    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });

    // Modal form handling
    $('.modal').on('hidden.bs.modal', function () {
        $(this).find('form')[0].reset();
        $(this).find('.is-invalid').removeClass('is-invalid');
    });
});

// Global functions
function showAlert(message, type = 'success') {
    var alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
    var icon = type === 'success' ? 'fas fa-check-circle' : 'fas fa-exclamation-circle';
    
    var alertHtml = `
        <div class="alert ${alertClass} alert-dismissible fade show" role="alert">
            <i class="${icon}"></i> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `;
    
    $('.container-fluid').prepend(alertHtml);
    
    // Auto-hide after 5 seconds
    setTimeout(function() {
        $('.alert').fadeOut('slow');
    }, 5000);
}

function confirmDelete(message = '確定要刪除這筆資料嗎？此操作無法復原。') {
    return confirm(message);
}

// AJAX helper functions
function ajaxPost(url, data, successCallback, errorCallback) {
    $.ajax({
        url: url,
        type: 'POST',
        data: data,
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        success: function(response) {
            if (successCallback) successCallback(response);
        },
        error: function(xhr, status, error) {
            if (errorCallback) errorCallback(xhr, status, error);
            else showAlert('操作失敗：' + error, 'error');
        }
    });
}

function ajaxGet(url, successCallback, errorCallback) {
    $.ajax({
        url: url,
        type: 'GET',
        success: function(response) {
            if (successCallback) successCallback(response);
        },
        error: function(xhr, status, error) {
            if (errorCallback) errorCallback(xhr, status, error);
            else showAlert('載入失敗：' + error, 'error');
        }
    });
}
