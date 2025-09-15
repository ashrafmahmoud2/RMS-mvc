// 🔹 Keep track of active messages
let activeToasts = new Set();

function showSuccessMessage(message) {
    Swal.fire({
        icon: 'success',
        title: 'Success',
        text: message,
        buttonsStyling: false,
        customClass: {
            confirmButton: "btn btn-outline btn-outline-dashed btn-outline-primary btn-active-light-primary"
        }
    });
}


function showErrorMessage(message) {
    Swal.fire({
        icon: 'error',
        title: 'خطأ...',
        text: message,
        customClass:
            { confirmButton: "btn btn-outline btn-outline-dashed btn-outline-primary btn-active-light-primary" }
    });
}

function showMessageInToast(message, type = "info") {
    // Prevent duplicate messages
    if (activeToasts.has(message)) return;

    activeToasts.add(message);

    const styles = {
        success: {
            background: "linear-gradient(135deg, #4caf50, #43a047)",
            icon: '<i class="fa-solid fa-circle-check"></i>'
        },
        error: {
            background: "linear-gradient(135deg, #f44336, #e53935)",
            icon: '<i class="fa-solid fa-circle-xmark"></i>'
        },
        warning: {
            background: "linear-gradient(135deg, #ff9800, #fb8c00)",
            icon: '<i class="fa-solid fa-triangle-exclamation"></i>'
        },
        info: {
            background: "linear-gradient(135deg, #2196f3, #1e88e5)",
            icon: '<i class="fa-solid fa-circle-info"></i>'
        }
    };

    let { background, icon } = styles[type] || styles.success;

    Toastify({
        text: `
            <span style="display:flex;align-items:center;gap:10px;font-size:15px;">
                ${icon}
                <span style="line-height:1.4;">${message}</span>
            </span>`,
        duration: 5000,
        gravity: "top",
        position: "center",
        stopOnFocus: true,
        close: true,
        escapeMarkup: false,
        style: {
            background: "linear-gradient(to right, #00b09b, #96c93d)",
            borderRadius: "12px",
            padding: "12px 18px",
            boxShadow: "0 4px 10px rgba(0,0,0,0.15)",
            color: "#fff",
            fontFamily: "system-ui, sans-serif"
        },
        callback: () => {
            // 🔹 Remove from active messages when closed
            activeToasts.delete(message);
        }
    }).showToast();
}
