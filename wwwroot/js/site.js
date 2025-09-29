// Site JavaScript
// Wait for jQuery to be loaded
if (typeof $ === 'undefined') {
    console.error('jQuery is not loaded. Please check the jQuery script tag.');
    // Cannot return here as we're not in a function
}

$(document).ready(function() {
    // Smooth scrolling for anchor links
    $('a[href^="#"]').on('click', function(event) {
        var target = $(this.getAttribute('href'));
        if (target.length) {
            event.preventDefault();
            $('html, body').stop().animate({
                scrollTop: target.offset().top - 70
            }, 1000);
        }
    });

    // Add fade-in animation to cards
    $('.card').addClass('fade-in-up');

    // Auto-hide alerts
    setTimeout(function() {
        $('.alert').fadeOut('slow');
    }, 5000);

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

    // Tooltip initialization
    if ($.fn.tooltip) {
        $('[data-bs-toggle="tooltip"]').tooltip();
    }

    // Popover initialization
    if ($.fn.popover) {
        $('[data-bs-toggle="popover"]').popover();
    }

    // Lazy loading for images
    if ('IntersectionObserver' in window) {
        const imageObserver = new IntersectionObserver((entries, observer) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    const img = entry.target;
                    img.src = img.dataset.src;
                    img.classList.remove('lazy');
                    imageObserver.unobserve(img);
                }
            });
        });

        document.querySelectorAll('img[data-src]').forEach(img => {
            imageObserver.observe(img);
        });
    }
});

// Global functions
function showAlert(message, type = 'success') {
    if (typeof $ === 'undefined') {
        console.error('jQuery is not loaded. Cannot show alert.');
        return;
    }
    
    var alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
    var icon = type === 'success' ? 'fas fa-check-circle' : 'fas fa-exclamation-circle';
    
    var alertHtml = `
        <div class="alert ${alertClass} alert-dismissible fade show" role="alert">
            <i class="${icon}"></i> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `;
    
    $('.container').prepend(alertHtml);
    
    // Auto-hide after 5 seconds
    setTimeout(function() {
        $('.alert').fadeOut('slow');
    }, 5000);
}

// AJAX helper functions
function ajaxPost(url, data, successCallback, errorCallback) {
    if (typeof $ === 'undefined') {
        console.error('jQuery is not loaded. Cannot make AJAX request.');
        return;
    }
    
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
    if (typeof $ === 'undefined') {
        console.error('jQuery is not loaded. Cannot make AJAX request.');
        return;
    }
    
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
