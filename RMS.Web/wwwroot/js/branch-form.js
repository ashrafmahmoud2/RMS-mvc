/**
 * Branch Form - Complete JavaScript
 * Handles: Governorate/Area cascade, Working hours, Exceptions, Images, Validation
 */

// Global variables
let exceptionIndex = 0; // Will be set from Razor

// ============================================================================
// 1. GOVERNORATE / AREA CASCADE
// ============================================================================

/**
 * Load areas based on selected governorate
 */
async function loadAreas(governorateId, selectedAreaId = null) {
    const areaSelect = document.getElementById('areaSelect');

 
    // Clear existing options
    areaSelect.innerHTML = '<option value="">-- اختر المنطقة --</option>';

    if (!governorateId) return;

    try {
        console.log("Arrive3", governorateId)
        const response = await fetch(`/Branch/GetAreas?governorateId=${governorateId}`);
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const areas = await response.json();

        areas.forEach(area => {
            const option = document.createElement('option');
            option.value = area.id;
            option.textContent = area.nameAr;
            areaSelect.appendChild(option);
        });

        // Reselect area if provided (for edit mode)
        if (selectedAreaId) {
            areaSelect.value = selectedAreaId;
        }
    } catch (error) {
        console.error('Error loading areas:', error);
        showNotification('حدث خطأ في تحميل المناطق', 'error');
    }
}

/**
 * Initialize governorate change listener and initial load
 */
function initGovernorateAreaCascade() {
    const governorateSelect = document.getElementById('governorateSelect');
    const areaSelect = document.getElementById('areaSelect');

    if (!governorateSelect || !areaSelect) return;

    // --- FIX START ---
    // 1. Read the initial selected AreaId from the server-rendered HTML.
    // This value is needed to re-select the correct area AFTER JS loads new options.
    const initialAreaId = areaSelect.value;
    // --- FIX END ---

    governorateSelect.addEventListener('change', function () {
        // When the governorate changes, we load new areas, but don't try to re-select
        // an old area ID, so we pass null for the selectedAreaId.
        loadAreas(this.value);
    });

    // Load areas on page load if governorate is already selected (edit mode)
    // We pass the initially selected Area ID to ensure it is re-selected after the loadAreas call.
    if (governorateSelect.value) {
        loadAreas(governorateSelect.value, initialAreaId);
    }
}

// ============================================================================
// 2. WORKING HOURS MANAGEMENT
// ============================================================================

/**
 * Toggle working day enable/disable
 */
function toggleDay(checkbox) {
    const day = checkbox.dataset.day;
    const timeInputs = document.getElementById(`timeInputs_${day}`);

    if (!timeInputs) return;

    const timeInputFields = timeInputs.querySelectorAll('input[type="time"]');

    if (checkbox.checked) {
        // Enable day
        timeInputs.style.display = 'grid';

        timeInputFields.forEach(input => {
            input.disabled = false;

            // Set default times if empty
            if (!input.value) {
                const isOpening = input.classList.contains('opening-time');
                input.value = isOpening ? '09:00' : '22:00';
            }
        });
    } else {
        // Disable day
        timeInputs.style.display = 'none';

        timeInputFields.forEach(input => {
            input.value = '';
            input.disabled = true;
        });
    }
}


// ============================================================================
// 3. EXCEPTION MANAGEMENT
// ============================================================================

/**
 * Set exception type (Closed 24h, Open 24h, Custom)
 */
function setExceptionType(button, type, index) {
    const exceptionItem = button.closest('.exception-item');
    if (!exceptionItem) return;

    const buttons = exceptionItem.querySelectorAll('.type-btn');
    const customTimes = exceptionItem.querySelector(`.custom-times-${index}`);

    // --- UPDATED: Use the single ExceptionType field ---
    const exceptionTypeInput = exceptionItem.querySelector(`input[name="WorkingHourExceptions[${index}].ExceptionType"]`);
    if (!exceptionTypeInput) return;

    // Remove active class from all buttons
    buttons.forEach(btn => {
        btn.classList.remove('active', 'closed', 'open24');
    });

    // Set active button and update hidden inputs/enum value
    if (type === 'closed') {
        button.classList.add('active', 'closed');
        if (customTimes) customTimes.style.display = 'none';
        exceptionTypeInput.value = 1; // 1 corresponds to ClosedAllDay (from BranchExceptionType enum)
    } else if (type === 'open24') {
        button.classList.add('active', 'open24');
        if (customTimes) customTimes.style.display = 'none';
        exceptionTypeInput.value = 2; // 2 corresponds to Open24Hours
    } else { // CustomHours
        button.classList.add('active');
        if (customTimes) customTimes.style.display = 'grid';
        exceptionTypeInput.value = 0; // 0 corresponds to CustomHours
    }
}

/**
 * Add new exception
 */
function addException() {
    const container = document.getElementById('exceptionsContainer');
    if (!container) return;

    const newIndex = exceptionIndex;
    const today = new Date().toISOString().split('T')[0];

    // NOTE: The hidden inputs for IsClosedAllDay and IsOpen24Hours are REMOVED
    // and replaced by the single ExceptionType input, reflecting the model optimization.
    const exceptionHtml = `
        <div class="exception-item" data-index="${newIndex}">
            <div class="exception-header">
                <span class="exception-title">استثناء ${newIndex + 1}</span>
                <button type="button" class="btn-delete" onclick="removeException(this)">
                    <i class="fas fa-trash"></i>
                </button>
            </div>
            
            <input type="hidden" name="WorkingHourExceptions[${newIndex}].Id" value="0" />
            <input type="hidden" name="WorkingHourExceptions[${newIndex}].ExceptionType" value="0" /> <!-- Default: CustomHours -->

            <div class="exception-type-buttons">
                <button type="button" class="type-btn" onclick="setExceptionType(this, 'closed', ${newIndex})">
                    <i class="fas fa-times-circle"></i> مغلق 24 ساعة
                </button>
                <button type="button" class="type-btn" onclick="setExceptionType(this, 'open24', ${newIndex})">
                    <i class="fas fa-clock"></i> مفتوح 24 ساعة
                </button>
                <button type="button" class="type-btn active" onclick="setExceptionType(this, 'custom', ${newIndex})">
                    <i class="fas fa-edit"></i> ساعات مخصصة
                </button>
            </div>

            <div class="form-row">
                <div class="form-group">
                    <label class="form-label required">اسم الاستثناء (عربي)</label>
                    <input type="text" name="WorkingHourExceptions[${newIndex}].ExceptionNameAr" 
                           class="form-control" placeholder="مثال: عيد الفطر" required />
                </div>

                <div class="form-group">
                    <label class="form-label required">اسم الاستثناء (إنجليزي)</label>
                    <input type="text" name="WorkingHourExceptions[${newIndex}].ExceptionNameEn" 
                           class="form-control" placeholder="Example: Eid Al-Fitr" required />
                </div>
            </div>

            <div class="form-row">
                <div class="form-group">
                    <label class="form-label required">من تاريخ</label>
                    <input type="date" name="WorkingHourExceptions[${newIndex}].StartDate" 
                           class="form-control" min="${today}" required />
                </div>

                <div class="form-group">
                    <label class="form-label required">إلى تاريخ</label>
                    <input type="date" name="WorkingHourExceptions[${newIndex}].EndDate" 
                           class="form-control" min="${today}" required />
                </div>
            </div>

            <div class="form-row custom-times-${newIndex}">
                <div class="form-group">
                    <label class="form-label">وقت الفتح</label>
                    <input type="time" name="WorkingHourExceptions[${newIndex}].OpeningTime" 
                           class="form-control" value="09:00" />
                </div>

                <div class="form-group">
                    <label class="form-label">وقت الإغلاق</label>
                    <input type="time" name="WorkingHourExceptions[${newIndex}].ClosingTime" 
                           class="form-control" value="22:00" />
                </div>
            </div>
        </div>
    `;

    container.insertAdjacentHTML('beforeend', exceptionHtml);
    exceptionIndex++;
}

/**
 * Remove exception and reindex
 */
function removeException(button) {
    // --- UPDATED: Use custom modal instead of built-in confirm() ---
    showConfirmModal('هل أنت متأكد من حذف هذا الاستثناء؟', () => {
        const item = button.closest('.exception-item');
        if (!item) return;

        item.remove();
        reindexExceptions();
        showNotification('تم حذف الاستثناء بنجاح', 'success');
    });
}

/**
 * Reindex all exceptions after removal
 */
function reindexExceptions() {
    const container = document.getElementById('exceptionsContainer');
    if (!container) return;

    const items = container.querySelectorAll('.exception-item');

    items.forEach((item, index) => {
        // Update data-index
        item.dataset.index = index;

        // Update title
        const title = item.querySelector('.exception-title');
        if (title) {
            title.textContent = `استثناء ${index + 1}`;
        }

        // Update all input names
        item.querySelectorAll('input, select, textarea').forEach(input => {
            const name = input.getAttribute('name');
            if (name) {
                input.setAttribute('name', name.replace(/\[\d+\]/, `[${index}]`));
            }
        });

        // Update onclick attributes for type buttons
        const typeButtons = item.querySelectorAll('.type-btn');
        typeButtons.forEach(btn => {
            const onclick = btn.getAttribute('onclick');
            if (onclick) {
                // Ensure the index argument replacement is precise
                const updatedOnclick = onclick.replace(/setExceptionType\(this,\s*'[^']+',\s*\d+\)/,
                    (match) => match.replace(/\d+/, index));
                btn.setAttribute('onclick', updatedOnclick);
            }
        });

        // Update custom-times class
        const customTimes = item.querySelector('[class*="custom-times-"]');
        if (customTimes) {
            customTimes.className = customTimes.className.replace(/custom-times-\d+/, `custom-times-${index}`);
        }
    });

    // Update global index
    exceptionIndex = items.length;
}

// ============================================================================
// 4. IMAGE MANAGEMENT
// ============================================================================

/**
 * Initialize image preview
 */
function initImagePreview() {
    const imageUpload = document.getElementById('imageUpload');
    if (!imageUpload) return;

    imageUpload.addEventListener('change', function (e) {
        const preview = document.getElementById('imagePreview');
        if (!preview) return;

        preview.innerHTML = '';

        Array.from(e.target.files).forEach(file => {
            if (file.type.startsWith('image/')) {
                // Validate file size (10MB)
                if (file.size > 10 * 1024 * 1024) {
                    showNotification(`الملف ${file.name} كبير جداً (أكثر من 10 ميجا)`, 'error');
                    return;
                }

                const reader = new FileReader();
                reader.onload = function (e) {
                    const div = document.createElement('div');
                    div.className = 'image-item';
                    div.innerHTML = `<img src="${e.target.result}" alt="Preview" />`;
                    preview.appendChild(div);
                };
                reader.readAsDataURL(file);
            }
        });
    });
}

/**
 * Delete existing image
 */
async function deleteImage(imageUrl, event) {
    event.preventDefault();

    // --- UPDATED: Use custom modal instead of built-in confirm() ---
    showConfirmModal('هل أنت متأكد من حذف هذه الصورة؟', async () => {
        const branchIdInput = document.querySelector('input[name="Id"]');
        const branchId = branchIdInput ? branchIdInput.value : 0;

        if (!branchId || branchId === '0') {
            showNotification('لا يمكن حذف الصورة، معرف الفرع غير موجود', 'error');
            return;
        }

        try {
            const response = await fetch('/Branches/DeleteImage', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                },
                body: JSON.stringify({ branchId: parseInt(branchId), imageUrl: imageUrl })
            });

            if (response.ok) {
                event.target.closest('.image-item').remove();
                showNotification('تم حذف الصورة بنجاح', 'success');
            } else {
                showNotification('فشل حذف الصورة', 'error');
            }
        } catch (error) {
            console.error('Error deleting image:', error);
            showNotification('حدث خطأ أثناء حذف الصورة', 'error');
        }
    });
}


// ============================================================================
// 5. PHONE VALIDATION
// ============================================================================

/**
 * Initialize phone validation (01 or 05)
 */
function initPhoneValidation() {
    const phoneInput = document.querySelector('input[name="Phone"]');
    if (!phoneInput) return;

    phoneInput.addEventListener('input', function (e) {
        let value = e.target.value.replace(/\D/g, '');

        // Limit to 11 digits
        if (value.length > 11) {
            value = value.slice(0, 11);
        }

        e.target.value = value;

        // Validate pattern
        if (value.length === 11) {
            if (!/^(01|05)[0-9]{9}$/.test(value)) {
                e.target.setCustomValidity('رقم الهاتف يجب أن يبدأ بـ 01 أو 05 ويكون 11 رقم');
            } else {
                e.target.setCustomValidity('');
            }
        } else if (value.length > 0) {
            e.target.setCustomValidity('رقم الهاتف يجب أن يكون 11 رقم');
        } else {
            e.target.setCustomValidity('');
        }
    });
}

// ============================================================================
// 6. FORM VALIDATION
// ============================================================================

/**
 * Validate form before submit
 */
function validateForm(e) {
    const errors = [];

    // 1. Validate at least one working day
    const enabledDays = document.querySelectorAll('.day-enabled:checked');
    if (enabledDays.length === 0) {
        errors.push('يجب تحديد يوم عمل واحد على الأقل');
    }

    // 2. Validate each enabled day has times
    enabledDays.forEach(checkbox => {
        const day = checkbox.dataset.day;
        const openTime = document.querySelector(`.opening-time[data-day="${day}"]`)?.value;
        const closeTime = document.querySelector(`.closing-time[data-day="${day}"]`)?.value;

        if (!openTime || !closeTime) {
            errors.push('يرجى إدخال أوقات العمل لجميع الأيام المحددة');
        }
    });

    // 3. Validate exceptions
    const exceptions = document.querySelectorAll('.exception-item');
    exceptions.forEach((item, index) => {
        const nameAr = item.querySelector('[name*="ExceptionNameAr"]')?.value;
        const nameEn = item.querySelector('[name*="ExceptionNameEn"]')?.value;
        const startDate = item.querySelector('[name*="StartDate"]')?.value;
        const endDate = item.querySelector('[name*="EndDate"]')?.value;
        const exceptionType = item.querySelector('[name*="ExceptionType"]')?.value; // Get the type

        // Check required fields
        if (!nameAr || !nameEn || !startDate || !endDate) {
            errors.push(`الاستثناء ${index + 1}: يجب ملء جميع الحقول المطلوبة`);
        }

        // Only validate times if the type is CustomHours (value 0)
        if (exceptionType === '0') {
            const openTime = item.querySelector('[name*="OpeningTime"]')?.value;
            const closeTime = item.querySelector('[name*="ClosingTime"]')?.value;
            if (!openTime || !closeTime) {
                errors.push(`الاستثناء ${index + 1}: يجب تحديد وقت الفتح والإغلاق للساعات المخصصة`);
            }
        }

        // Check date range
        if (startDate && endDate && new Date(startDate) > new Date(endDate)) {
            errors.push(`الاستثناء ${index + 1}: تاريخ البداية يجب أن يكون قبل تاريخ النهاية`);
        }
    });

    // 4. Show errors if any
    if (errors.length > 0) {
        e.preventDefault();
        showNotification(errors.join('\n'), 'error');
        return false;
    }

    return true;
}

/**
 * Initialize form validation
 */
function initFormValidation() {
    const form = document.getElementById('branchForm');
    if (!form) return;

    form.addEventListener('submit', validateForm);
}

// ============================================================================
// 7. NOTIFICATIONS & CUSTOM MODALS
// ============================================================================

/**
 * Show notification message
 */
function showNotification(message, type = 'info') {
    // Create notification element if doesn't exist
    let notification = document.getElementById('notification');

    if (!notification) {
        notification = document.createElement('div');
        notification.id = 'notification';
        notification.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 9999;
            padding: 16px 24px;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
            max-width: 400px;
            font-weight: 500;
            transition: opacity 0.3s, transform 0.3s;
            text-align: right; /* Ensure RTL text is aligned correctly */
        `;
        document.body.appendChild(notification);
    }

    // Set colors based on type
    const colors = {
        success: { bg: '#10b981', text: '#ffffff' },
        error: { bg: '#ef4444', text: '#ffffff' },
        warning: { bg: '#f59e0b', text: '#ffffff' },
        info: { bg: '#3b82f6', text: '#ffffff' }
    };

    const color = colors[type] || colors.info;
    notification.style.backgroundColor = color.bg;
    notification.style.color = color.text;
    notification.textContent = message;
    notification.style.opacity = '1';
    notification.style.transform = 'translateY(0)';

    // Auto hide after 5 seconds
    setTimeout(() => {
        notification.style.opacity = '0';
        notification.style.transform = 'translateY(-20px)';
    }, 5000);
}

/**
 * Show a custom confirmation modal (replaces window.confirm)
 */
function showConfirmModal(message, onConfirm) {
    let modal = document.getElementById('customConfirmModal');
    let overlay = document.getElementById('customConfirmOverlay');

    if (!modal) {
        overlay = document.createElement('div');
        overlay.id = 'customConfirmOverlay';
        overlay.style.cssText = `
            position: fixed;
            top: 0; left: 0; right: 0; bottom: 0;
            background: rgba(0, 0, 0, 0.5);
            z-index: 10000;
            display: none;
            justify-content: center;
            align-items: center;
        `;
        document.body.appendChild(overlay);

        modal = document.createElement('div');
        modal.id = 'customConfirmModal';
        modal.style.cssText = `
            background: #ffffff;
            padding: 30px;
            border-radius: 12px;
            box-shadow: 0 8px 30px rgba(0, 0, 0, 0.2);
            max-width: 400px;
            width: 90%;
            text-align: center;
        `;
        overlay.appendChild(modal);
    }

    // Clear previous content
    modal.innerHTML = '';

    const messageP = document.createElement('p');
    messageP.textContent = message;
    messageP.style.marginBottom = '20px';
    messageP.style.fontSize = '1.1rem';
    modal.appendChild(messageP);

    const buttonContainer = document.createElement('div');
    buttonContainer.style.display = 'flex';
    buttonContainer.style.gap = '10px';
    buttonContainer.style.justifyContent = 'center';

    const confirmBtn = document.createElement('button');
    confirmBtn.textContent = 'نعم';
    confirmBtn.style.cssText = `
        padding: 10px 20px;
        background-color: #ef4444;
        color: white;
        border: none;
        border-radius: 6px;
        cursor: pointer;
        font-weight: 600;
    `;
    confirmBtn.onclick = () => {
        overlay.style.display = 'none';
        onConfirm();
    };

    const cancelBtn = document.createElement('button');
    cancelBtn.textContent = 'إلغاء';
    cancelBtn.style.cssText = `
        padding: 10px 20px;
        background-color: #ccc;
        color: #333;
        border: none;
        border-radius: 6px;
        cursor: pointer;
        font-weight: 600;
    `;
    cancelBtn.onclick = () => {
        overlay.style.display = 'none';
    };

    buttonContainer.appendChild(confirmBtn);
    buttonContainer.appendChild(cancelBtn);
    modal.appendChild(buttonContainer);

    overlay.style.display = 'flex';
}

// ============================================================================
// 8. INITIALIZATION
// ============================================================================

/**
 * Initialize all form functionality
 */
function initBranchForm() {
    // Initialize governorate/area cascade
    initGovernorateAreaCascade();

    // Initialize image preview
    initImagePreview();

    // Initialize phone validation
    initPhoneValidation();

    // Initialize form validation
    initFormValidation();

    // Ensure working hours display correctly on page load
    document.querySelectorAll('.day-enabled').forEach(checkbox => {
        const day = checkbox.dataset.day;
        const timeInputs = document.getElementById(`timeInputs_${day}`);

        if (timeInputs && checkbox.checked) {
            timeInputs.style.display = 'grid';
            timeInputs.querySelectorAll('input').forEach(input => {
                input.disabled = false;
            });
        }
    });

    // --- UPDATED: Initialize Exception State on Load ---
    document.querySelectorAll('.exception-item').forEach(item => {
        const index = item.dataset.index;
        const exceptionTypeInput = item.querySelector(`input[name="WorkingHourExceptions[${index}].ExceptionType"]`);
        if (!exceptionTypeInput) return;

        const typeValue = exceptionTypeInput.value;
        const customTimes = item.querySelector(`.custom-times-${index}`);
        const typeButtons = item.querySelectorAll('.type-btn');

        // Find the correct button and set its state
        let targetButton = null;
        if (typeValue === '1') { // ClosedAllDay
            targetButton = typeButtons[0];
            if (customTimes) customTimes.style.display = 'none';
        } else if (typeValue === '2') { // Open24Hours
            targetButton = typeButtons[1];
            if (customTimes) customTimes.style.display = 'none';
        } else { // CustomHours (0)
            targetButton = typeButtons[2];
            if (customTimes) customTimes.style.display = 'grid';
        }

        if (targetButton) {
            targetButton.classList.add('active');
            if (typeValue === '1') targetButton.classList.add('closed');
            if (typeValue === '2') targetButton.classList.add('open24');
        }
    });

    console.log('Branch form initialized successfully');
}

// Initialize when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initBranchForm);
} else {
    initBranchForm();
}

// ============================================================================
// 9. EXPORT FUNCTIONS (for inline onclick handlers)
// ============================================================================

// Make functions globally accessible
window.toggleDay = toggleDay;
window.fillAllDays = fillAllDays;
window.setExceptionType = setExceptionType;
window.addException = addException;
window.removeException = removeException;
window.deleteImage = deleteImage;
window.showNotification = showNotification; // Exporting for general use
