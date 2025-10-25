/**
 * Branch Form - Complete JavaScript with Modern UI/UX
 * Fixed: Working hours submission only sends selected days
 * Enhanced: Premium time picker, smooth animations, improved UX
 */

// Global variables
//let exceptionIndex = 0;

// ============================================================================
// 1. WORKING HOURS - CRITICAL FIX
// ============================================================================

function prepareWorkingHoursForSubmission() {
    const form = document.getElementById('branchForm');
    if (!form) return;

    const workingHourItems = document.querySelectorAll('.working-hour-item');
    const checkedDays = new Set();

    workingHourItems.forEach(item => {
        const checkbox = item.querySelector('.day-enabled:checked');
        if (checkbox) {
            checkedDays.add(parseInt(checkbox.dataset.day));
        }
    });

    form.querySelectorAll('input[name^="WorkingHours["]').forEach(input => {
        input.remove();
    });

    let index = 0;
    checkedDays.forEach(dayOfWeek => {
        const dayItem = document.querySelector(`.working-hour-item input[data-day="${dayOfWeek}"].day-enabled:checked`)
            ?.closest('.working-hour-item');

        if (dayItem) {
            const timeInputs = dayItem.querySelector(`.time-inputs`);
            const openingTime = timeInputs?.querySelector('.opening-time')?.value;
            const closingTime = timeInputs?.querySelector('.closing-time')?.value;

            const dayOfWeekInput = document.createElement('input');
            dayOfWeekInput.type = 'hidden';
            dayOfWeekInput.name = `WorkingHours[${index}].DayOfWeek`;
            dayOfWeekInput.value = dayOfWeek;
            form.appendChild(dayOfWeekInput);

            const openingTimeInput = document.createElement('input');
            openingTimeInput.type = 'hidden';
            openingTimeInput.name = `WorkingHours[${index}].OpeningTime`;
            openingTimeInput.value = openingTime || '09:00';
            form.appendChild(openingTimeInput);

            const closingTimeInput = document.createElement('input');
            closingTimeInput.type = 'hidden';
            closingTimeInput.name = `WorkingHours[${index}].ClosingTime`;
            closingTimeInput.value = closingTime || '22:00';
            form.appendChild(closingTimeInput);

            index++;
        }
    });

    console.log('✓ Working hours prepared. Checked days:', Array.from(checkedDays));
}

// ============================================================================
// 2. PREMIUM TIME PICKER COMPONENT
// ============================================================================

class TimePicker {
    constructor(inputElement) {
        this.input = inputElement;
        this.container = null;
        this.isOpen = false;
        this.currentDisplayHour = 0;
        this.currentMinutes = 0;
        this.currentPeriod = 'ص';
        this.init();
    }

    init() {
        this.input.type = 'text';
        this.input.style.cursor = 'pointer';

        this.input.addEventListener('click', (e) => {
            e.stopPropagation();
            this.toggle();
        });

        document.addEventListener('click', (e) => {
            if (this.isOpen && e.target !== this.input && (!this.container || !this.container.contains(e.target))) {
                this.close();
            }
        });

        this.input.addEventListener('focus', () => {
            if (!this.isOpen) this.open();
        });
    }

    toggle() {
        if (this.isOpen) {
            this.close();
        } else {
            this.open();
        }
    }

    open() {
        if (this.isOpen) return;

        this.isOpen = true;
        this.createPicker();

        // Smooth focus animation
        this.input.style.transition = 'all 0.2s ease';
        this.input.style.borderColor = '#185499';
        this.input.style.boxShadow = '0 0 0 3px rgba(24, 84, 153, 0.15)';
    }

    close() {
        if (!this.isOpen) return;

        this.isOpen = false;
        if (this.container) {
            this.container.style.animation = 'fadeOut 0.2s ease';
            setTimeout(() => {
                this.container.remove();
                this.container = null;
            }, 200);
        }
        this.input.style.borderColor = '';
        this.input.style.boxShadow = '';
    }

    createPicker() {
        const defaultTime = '09:00 ص';
        const value = this.input.value || defaultTime;

        let displayHour = 9;
        let initialMinutes = 0;
        let period = 'ص';

        const parts = value.match(/(\d{1,2}):(\d{2})\s*(ص|م)/);

        if (parts) {
            displayHour = parseInt(parts[1], 10);
            initialMinutes = parseInt(parts[2], 10);
            period = parts[3];

            if (displayHour < 1 || displayHour > 12) displayHour = 9;

        } else {
            const timeParts = value.match(/(\d{1,2}):(\d{2})/);
            if (timeParts) {
                const h24 = parseInt(timeParts[1], 10);
                initialMinutes = parseInt(timeParts[2], 10);

                period = h24 >= 12 ? 'م' : 'ص';
                displayHour = (h24 % 12) || 12;
            }
        }

        let minutes = Math.round(initialMinutes / 15) * 15;

        this.currentDisplayHour = displayHour;
        this.currentMinutes = minutes;
        this.currentPeriod = period;

        this.container = document.createElement('div');
        this.container.style.cssText = `
            position: absolute;
            top: 100%;
            left: 50%;
            transform: translateX(-50%);
            background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
            border: 1px solid #e2e8f0;
            border-radius: 12px;
            padding: 0;
            margin-top: 12px;
            z-index: 1000;
            box-shadow: 0 10px 30px rgba(0, 0, 0, 0.12), 0 0 0 1px rgba(24, 84, 153, 0.05);
            display: flex;
            justify-content: center;
            align-items: flex-start;
            min-width: 280px;
            max-width: 320px;
            height: 220px;
            overflow: hidden;
            font-family: 'Inter', 'Segoe UI', sans-serif;
            direction: rtl;
            animation: slideUp 0.3s cubic-bezier(0.16, 1, 0.3, 1);
        `;

        const hoursColumn = this.createScrollColumn('ساعة', 1, 12, this.currentDisplayHour, 1, this.handleHourClick.bind(this));
        const minutesColumn = this.createScrollColumn('دقيقة', 0, 45, this.currentMinutes, 15, this.handleMinuteClick.bind(this));
        const periodColumn = this.createPeriodColumn(this.currentPeriod, this.handlePeriodClick.bind(this));

        const separator = document.createElement('div');
        separator.textContent = ':';
        separator.style.cssText = `
            font-size: 28px;
            font-weight: 700;
            color: #185499;
            align-self: center;
            padding: 0 6px;
            pointer-events: none;
        `;

        this.container.appendChild(hoursColumn);
        this.container.appendChild(separator);
        this.container.appendChild(minutesColumn);
        this.container.appendChild(periodColumn);

        this.input.parentElement.style.position = 'relative';
        this.input.parentElement.appendChild(this.container);

        this.scrollToActive(hoursColumn, this.currentDisplayHour);
        this.scrollToActive(minutesColumn, this.currentMinutes);
        this.scrollToActive(periodColumn, this.currentPeriod);

        // Add animation styles if not present
        this.addAnimationStyles();
    }

    createScrollColumn(label, start, end, currentValue, step, clickHandler) {
        const column = document.createElement('div');
        column.className = 'timepicker-column';
        column.style.cssText = `
            flex: 1;
            overflow-y: scroll;
            height: 100%;
            padding: 12px 0;
            text-align: center;
            -ms-overflow-style: none;
            scrollbar-width: none;
            scroll-behavior: smooth;
        `;
        column.style.scrollbarWidth = 'none';

        const labelEl = document.createElement('div');
        labelEl.textContent = label;
        labelEl.style.cssText = `
            font-size: 11px;
            font-weight: 700;
            color: #64748b;
            margin-bottom: 8px;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        `;
        column.appendChild(labelEl);

        const list = document.createElement('ul');
        list.style.cssText = `
            list-style: none;
            padding: 0;
            margin: 0;
            border-radius: 8px;
        `;

        const topBuffer = document.createElement('li');
        topBuffer.style.cssText = 'height: 60px; pointer-events: none;';
        list.appendChild(topBuffer);

        for (let i = start; i <= end; i += step) {
            const itemValue = String(i).padStart(2, '0');
            const listItem = document.createElement('li');
            listItem.textContent = itemValue;
            listItem.dataset.value = i;

            const isActive = i === currentValue;
            listItem.style.cssText = `
                padding: 12px 8px;
                cursor: pointer;
                font-size: 18px;
                font-weight: ${isActive ? '700' : '500'};
                color: ${isActive ? '#185499' : '#64748b'};
                background: ${isActive ? 'rgba(24, 84, 153, 0.1)' : 'transparent'};
                border-radius: 6px;
                transition: all 0.15s cubic-bezier(0.4, 0, 0.2, 1);
                user-select: none;
            `;

            listItem.addEventListener('mouseenter', () => {
                if (parseInt(listItem.dataset.value) !== currentValue) {
                    listItem.style.background = 'rgba(24, 84, 153, 0.05)';
                }
            });

            listItem.addEventListener('mouseleave', () => {
                if (parseInt(listItem.dataset.value) !== currentValue) {
                    listItem.style.background = 'transparent';
                }
            });

            listItem.addEventListener('click', clickHandler);
            list.appendChild(listItem);
        }

        const bottomBuffer = document.createElement('li');
        bottomBuffer.style.cssText = 'height: 60px; pointer-events: none;';
        list.appendChild(bottomBuffer);

        column.appendChild(list);
        return column;
    }

    createPeriodColumn(currentPeriod, clickHandler) {
        const periodMap = { 'ص': 'صباحًا', 'م': 'مساءً' };

        const column = document.createElement('div');
        column.className = 'timepicker-column period-column';
        column.style.cssText = `
            flex: 0 0 80px;
            overflow-y: scroll;
            height: 100%;
            padding: 12px 0;
            text-align: center;
            -ms-overflow-style: none;
            scrollbar-width: none;
            border-right: 1px solid #e2e8f0;
        `;

        const labelEl = document.createElement('div');
        labelEl.textContent = 'الفترة';
        labelEl.style.cssText = `
            font-size: 11px;
            font-weight: 700;
            color: #64748b;
            margin-bottom: 8px;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        `;
        column.appendChild(labelEl);

        const list = document.createElement('ul');
        list.style.cssText = `
            list-style: none;
            padding: 0;
            margin: 0;
            border-radius: 8px;
        `;

        const topBuffer = document.createElement('li');
        topBuffer.style.cssText = 'height: 60px; pointer-events: none;';
        list.appendChild(topBuffer);

        ['ص', 'م'].forEach(key => {
            const listItem = document.createElement('li');
            listItem.textContent = periodMap[key];
            listItem.dataset.value = key;

            const isActive = key === currentPeriod;
            listItem.style.cssText = `
                padding: 12px 4px;
                cursor: pointer;
                font-size: 14px;
                font-weight: ${isActive ? '700' : '500'};
                color: ${isActive ? '#185499' : '#64748b'};
                background: ${isActive ? 'rgba(24, 84, 153, 0.1)' : 'transparent'};
                border-radius: 6px;
                transition: all 0.15s cubic-bezier(0.4, 0, 0.2, 1);
                user-select: none;
            `;

            listItem.addEventListener('mouseenter', () => {
                if (key !== currentPeriod) {
                    listItem.style.background = 'rgba(24, 84, 153, 0.05)';
                }
            });

            listItem.addEventListener('mouseleave', () => {
                if (key !== currentPeriod) {
                    listItem.style.background = 'transparent';
                }
            });

            listItem.addEventListener('click', clickHandler);
            list.appendChild(listItem);
        });

        const bottomBuffer = document.createElement('li');
        bottomBuffer.style.cssText = 'height: 60px; pointer-events: none;';
        list.appendChild(bottomBuffer);

        column.appendChild(list);
        return column;
    }

    scrollToActive(columnElement, currentValue) {
        const list = columnElement.querySelector('ul');
        const item = list.querySelector(`[data-value="${currentValue}"]`);

        if (item) {
            columnElement.scrollTop = item.offsetTop - (columnElement.offsetHeight / 2) + (item.offsetHeight / 2);
        }
    }

    handleHourClick(e) {
        const hour = parseInt(e.target.dataset.value);
        this.currentDisplayHour = hour;
        this.updateTime();
        this.refreshColumn(e.target.closest('.timepicker-column'), hour);
    }

    handleMinuteClick(e) {
        const minute = parseInt(e.target.dataset.value);
        this.currentMinutes = minute;
        this.updateTime();
        this.refreshColumn(e.target.closest('.timepicker-column'), minute);
        this.close();
    }

    handlePeriodClick(e) {
        const period = e.target.dataset.value;
        this.currentPeriod = period;
        this.updateTime();
        this.refreshPeriodColumn(e.target.closest('.timepicker-column'), period);
    }

    updateTime() {
        const hours = String(this.currentDisplayHour).padStart(2, '0');
        const minutes = String(this.currentMinutes).padStart(2, '0');
        this.input.value = `${hours}:${minutes} ${this.currentPeriod}`;
        this.input.dispatchEvent(new Event('change', { bubbles: true }));
    }

    refreshColumn(column, newValue) {
        column.querySelectorAll('li[data-value]').forEach(li => {
            const value = parseInt(li.dataset.value);
            const isActive = value === newValue;
            li.style.fontWeight = isActive ? '700' : '500';
            li.style.color = isActive ? '#185499' : '#64748b';
            li.style.background = isActive ? 'rgba(24, 84, 153, 0.1)' : 'transparent';
        });
    }

    refreshPeriodColumn(column, newPeriod) {
        column.querySelectorAll('li[data-value]').forEach(li => {
            const period = li.dataset.value;
            const isActive = period === newPeriod;
            li.style.fontWeight = isActive ? '700' : '500';
            li.style.color = isActive ? '#185499' : '#64748b';
            li.style.background = isActive ? 'rgba(24, 84, 153, 0.1)' : 'transparent';
        });
    }

    addAnimationStyles() {
        if (!document.getElementById('timePickerAnimations')) {
            const style = document.createElement('style');
            style.id = 'timePickerAnimations';
            style.textContent = `
                @keyframes slideUp {
                    from {
                        opacity: 0;
                        transform: translateX(-50%) translateY(10px);
                    }
                    to {
                        opacity: 1;
                        transform: translateX(-50%) translateY(0);
                    }
                }

                @keyframes fadeOut {
                    from {
                        opacity: 1;
                        transform: translateX(-50%) translateY(0);
                    }
                    to {
                        opacity: 0;
                        transform: translateX(-50%) translateY(10px);
                    }
                }

                .timepicker-column::-webkit-scrollbar {
                    display: none;
                }
            `;
            document.head.appendChild(style);
        }
    }
}

function initTimePickers() {
    document.querySelectorAll('input[type="time"].form-control').forEach(input => {
        if (!input.dataset.timepickerInitialized) {
            new TimePicker(input);
            input.dataset.timepickerInitialized = 'true';
        }
    });
}

window.addEventListener('load', initTimePickers);

// ============================================================================
// 3. GOVERNORATE / AREA CASCADE
// ============================================================================

async function loadAreas(governorateId, selectedAreaId = null) {
    const areaSelect = document.getElementById('areaSelect');
    if (!areaSelect) return;

    areaSelect.innerHTML = '<option value="">-- جاري التحميل... --</option>';

    if (!governorateId) {
        areaSelect.innerHTML = '<option value="">-- اختر المنطقة --</option>';
        return;
    }

    try {
        const response = await fetch(`/Branch/GetAreas?governorateId=${governorateId}`);

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const areas = await response.json();

        areaSelect.innerHTML = '<option value="">-- اختر المنطقة --</option>';

        areas.forEach(area => {
            const option = document.createElement('option');
            option.value = area.id;
            option.textContent = area.nameAr;
            areaSelect.appendChild(option);
        });

        if (selectedAreaId) {
            areaSelect.value = selectedAreaId;
        }
    } catch (error) {
        console.error('Error loading areas:', error);
        showNotification('حدث خطأ في تحميل المناطق', 'error');
    }
}

function initGovernorateAreaCascade() {
    const governorateSelect = document.getElementById('governorateSelect');
    const areaSelect = document.getElementById('areaSelect');

    if (!governorateSelect || !areaSelect) return;

    const initialAreaId = areaSelect.value;

    governorateSelect.addEventListener('change', function () {
        loadAreas(this.value);
    });

    if (governorateSelect.value) {
        loadAreas(governorateSelect.value, initialAreaId);
    }
}

// ============================================================================
// 4. WORKING HOURS MANAGEMENT
// ============================================================================

function toggleDay(checkbox) {
    const day = checkbox.dataset.day;
    const timeInputs = document.getElementById(`timeInputs_${day}`);

    if (!timeInputs) return;

    const timeInputFields = timeInputs.querySelectorAll('input[type="time"]');

    if (checkbox.checked) {
        timeInputs.style.animation = 'slideDown 0.3s cubic-bezier(0.4, 0, 0.2, 1)';
        timeInputs.style.display = 'grid';
        timeInputFields.forEach(input => {
            input.disabled = false;

            if (!input.value) {
                const isOpening = input.classList.contains('opening-time');
                input.value = isOpening ? '09:00' : '22:00';
            }
        });
    } else {
        timeInputs.style.animation = 'slideUp 0.2s cubic-bezier(0.4, 0, 0.2, 1)';
        setTimeout(() => {
            timeInputs.style.display = 'none';
        }, 200);
        timeInputFields.forEach(input => {
            input.value = '';
            input.disabled = true;
        });
    }
}

function copyFirstDayToAll() {
    const firstDay = document.querySelector('.day-enabled:checked');
    if (!firstDay) {
        showNotification('يرجى تحديد يوم واحد على الأقل أولاً', 'warning');
        return;
    }

    const dayId = firstDay.dataset.day;
    const sourceInputs = document.getElementById('timeInputs_' + dayId);
    const openingTime = sourceInputs.querySelector('.opening-time').value;
    const closingTime = sourceInputs.querySelector('.closing-time').value;

    document.querySelectorAll('.day-enabled:checked').forEach(checkbox => {
        const currentDayId = checkbox.dataset.day;
        const timeInputsDiv = document.getElementById('timeInputs_' + currentDayId);
        timeInputsDiv.querySelector('.opening-time').value = openingTime;
        timeInputsDiv.querySelector('.closing-time').value = closingTime;
    });

    showNotification('✓ تم نسخ الأوقات بنجاح لجميع الأيام', 'success');
}

// ============================================================================
// 5. EXCEPTION MANAGEMENT
// ============================================================================

function setExceptionType(button, type) {
    const exceptionItem = button.closest('.exception-item');
    if (!exceptionItem) return;

    const index = exceptionItem.dataset.index;
    const buttons = exceptionItem.querySelectorAll('.type-btn');
    const customTimes = exceptionItem.querySelector('.custom-times');
    const exceptionTypeInput = exceptionItem.querySelector(`input[name="WorkingHourExceptions[${index}].ExceptionType"]`);

    if (!exceptionTypeInput) return;

    buttons.forEach(btn => {
        btn.classList.remove('active');
        btn.style.transform = 'scale(1)';
    });

    button.classList.add('active');
    button.style.transform = 'scale(1.05)';
    setTimeout(() => {
        button.style.transform = 'scale(1)';
    }, 100);

    if (type === 'closed') {
        if (customTimes) customTimes.style.display = 'none';
        exceptionTypeInput.value = 1;
    } else if (type === 'open24') {
        if (customTimes) customTimes.style.display = 'none';
        exceptionTypeInput.value = 2;
    } else {
        if (customTimes) customTimes.style.display = 'grid';
        exceptionTypeInput.value = 0;
    }
}

function addException() {
    const container = document.getElementById('exceptionsContainer');
    if (!container) return;

    const newIndex = exceptionIndex;
    const today = new Date().toISOString().split('T')[0];

    const exceptionHtml = `
        <div class="exception-item" data-index="${newIndex}" style="animation: slideUp 0.3s cubic-bezier(0.4, 0, 0.2, 1);">
            <div class="exception-header">
                <span class="exception-title">
                    <i class="fas fa-tag"></i>
                    استثناء ${newIndex + 1}
                </span>
                <button type="button" class="btn-delete" onclick="removeException(this)">
                    <i class="fas fa-trash"></i>
                </button>
            </div>

            <input type="hidden" name="WorkingHourExceptions[${newIndex}].Id" value="0" />
            <input type="hidden" name="WorkingHourExceptions[${newIndex}].ExceptionType" value="0" class="exception-type-input" />

            <div class="exception-type-buttons">
                <button type="button" class="type-btn" onclick="setExceptionType(this, 'closed')">
                    <i class="fas fa-times-circle"></i>
                    <span>مغلق 24 ساعة</span>
                </button>
                <button type="button" class="type-btn" onclick="setExceptionType(this, 'open24')">
                    <i class="fas fa-infinity"></i>
                    <span>مفتوح 24 ساعة</span>
                </button>
                <button type="button" class="type-btn active" onclick="setExceptionType(this, 'custom')">
                    <i class="fas fa-sliders-h"></i>
                    <span>ساعات مخصصة</span>
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

            <div class="form-row custom-times" style="display: grid;">
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
    initTimePickers();

    showNotification('✓ تم إضافة استثناء جديد', 'success');
}

function removeException(button) {
    if (typeof showConfirmModal === 'function') {
        showConfirmModal('هل أنت متأكد من حذف هذا الاستثناء؟', () => {
            const item = button.closest('.exception-item');
            if (!item) return;

            item.style.animation = 'slideUp 0.2s cubic-bezier(0.4, 0, 0.2, 1)';
            setTimeout(() => {
                item.remove();
                reindexExceptions();
                showNotification('✓ تم حذف الاستثناء بنجاح', 'success');
            }, 200);
        });
    } else {
        if (confirm('هل أنت متأكد من حذف هذا الاستثناء؟')) {
            const item = button.closest('.exception-item');
            if (!item) return;

            item.remove();
            reindexExceptions();
        }
    }
}

function reindexExceptions() {
    const container = document.getElementById('exceptionsContainer');
    if (!container) return;

    const items = container.querySelectorAll('.exception-item');

    items.forEach((item, index) => {
        item.dataset.index = index;

        const title = item.querySelector('.exception-title');
        if (title) {
            title.innerHTML = `<i class="fas fa-tag"></i>استثناء ${index + 1}`;
        }

        item.querySelectorAll('input, select, textarea').forEach(input => {
            const name = input.getAttribute('name');
            if (name) {
                input.setAttribute('name', name.replace(/\[\d+\]/, `[${index}]`));
            }
        });

        const typeButtons = item.querySelectorAll('.type-btn');
        typeButtons.forEach(btn => {
            const onclick = btn.getAttribute('onclick');
            if (onclick) {
                const updatedOnclick = onclick.replace(/setExceptionType\(this,\s*'[^']+',\s*\d+\)/,
                    (match) => match.replace(/\d+/, index));
                btn.setAttribute('onclick', updatedOnclick);
            }
        });
    });

    exceptionIndex = items.length;
}

// ============================================================================
// 6. IMAGE MANAGEMENT
// ============================================================================

function initImagePreview() {
    const imageUpload = document.getElementById('imageUpload');
    if (!imageUpload) return;

    imageUpload.addEventListener('change', function (e) {
        const preview = document.getElementById('imagePreview');
        if (!preview) return;

        preview.innerHTML = '';

        Array.from(e.target.files).forEach((file, index) => {
            if (file.type.startsWith('image/')) {
                if (file.size > 10 * 1024 * 1024) {
                    showNotification(`الملف ${file.name} كبير جداً (أكثر من 10 ميجا)`, 'error');
                    return;
                }

                const reader = new FileReader();
                reader.onload = function (e) {
                    const div = document.createElement('div');
                    div.className = 'image-item';
                    div.style.animation = `slideUp 0.3s cubic-bezier(0.4, 0, 0.2, 1) ${index * 50}ms`;
                    div.innerHTML = `
                        <img src="${e.target.result}" alt="Preview" />
                        <button type="button" class="btn-delete-image" onclick="this.closest('.image-item').style.animation='slideUp 0.2s'; setTimeout(() => this.closest('.image-item').remove(), 200);">
                            <i class="fas fa-trash"></i>
                        </button>
                    `;
                    preview.appendChild(div);
                };
                reader.readAsDataURL(file);
            }
        });
    });

    // Drag and drop support
    const uploadArea = document.getElementById('uploadArea');
    if (uploadArea) {
        uploadArea.addEventListener('dragover', (e) => {
            e.preventDefault();
            uploadArea.style.borderColor = '#185499';
            uploadArea.style.background = 'rgba(24, 84, 153, 0.05)';
            uploadArea.style.transform = 'scale(1.02)';
        });

        uploadArea.addEventListener('dragleave', () => {
            uploadArea.style.borderColor = 'var(--border)';
            uploadArea.style.background = 'transparent';
            uploadArea.style.transform = 'scale(1)';
        });

        uploadArea.addEventListener('drop', (e) => {
            e.preventDefault();
            uploadArea.style.borderColor = 'var(--border)';
            uploadArea.style.background = 'transparent';
            uploadArea.style.transform = 'scale(1)';
            imageUpload.files = e.dataTransfer.files;
            imageUpload.dispatchEvent(new Event('change', { bubbles: true }));
        });
    }
}

async function deleteImage(imageUrl, event) {
    event.preventDefault();

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
                const imageItem = event.target.closest('.image-item');
                imageItem.style.animation = 'slideUp 0.2s cubic-bezier(0.4, 0, 0.2, 1)';
                setTimeout(() => {
                    imageItem.remove();
                }, 200);
                showNotification('✓ تم حذف الصورة بنجاح', 'success');
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
// 7. PHONE VALIDATION
// ============================================================================

function initPhoneValidation() {
    const phoneInput = document.querySelector('input[name="Phone"]');
    if (!phoneInput) return;

    const mobileOrSaudiPattern = /^(01|05)[0-9]{9}$/;
    const landlinePattern = /^(02|03|04|05|06|08|09)[0-9]{7}$/;
    const MAX_LENGTH = 11;

    phoneInput.addEventListener('input', function (e) {
        let value = e.target.value.replace(/\D/g, '');

        if (value.length > MAX_LENGTH) {
            value = value.slice(0, MAX_LENGTH);
        }

        e.target.value = value;

        if (value.length === 11) {
            if (!mobileOrSaudiPattern.test(value)) {
                e.target.setCustomValidity('رقم الهاتف المحمول يجب أن يبدأ بـ 01 أو 05 ويكون 11 رقم');
            } else {
                e.target.setCustomValidity('');
            }
        } else if (value.length === 8) {
            if (!landlinePattern.test(value)) {
                e.target.setCustomValidity('رقم الهاتف الأرضي يجب أن يكون 8 أرقام ويبدأ بكود المحافظة (02, 03, إلخ).');
            } else {
                e.target.setCustomValidity('');
            }
        } else if (value.length > 0) {
            e.target.setCustomValidity('رقم الهاتف يجب أن يكون 8 أرقام (أرضي) أو 11 رقم (محمول).');
        } else {
            e.target.setCustomValidity('');
        }
    });
}

// ============================================================================
// 8. FORM VALIDATION
// ============================================================================

function validateForm(e) {
    const errors = [];

    const enabledDays = document.querySelectorAll('.day-enabled:checked');
    if (enabledDays.length === 0) {
        errors.push('يجب تحديد يوم عمل واحد على الأقل');
    }

    enabledDays.forEach(checkbox => {
        const day = checkbox.dataset.day;
        const dayItem = checkbox.closest('.working-hour-item');
        const openTime = dayItem?.querySelector('.opening-time')?.value;
        const closeTime = dayItem?.querySelector('.closing-time')?.value;

        if (!openTime || !closeTime) {
            const dayName = checkbox.nextElementSibling?.textContent || 'اليوم المختار';
            errors.push(`${dayName}: يرجى إدخال أوقات العمل`);
        } else if (openTime === closeTime) {
            const dayName = checkbox.nextElementSibling?.textContent || 'اليوم المختار';
            errors.push(`${dayName}: لا يمكن أن يكون وقت الفتح والإغلاق متطابقين تماماً (مدة العمل صفر).`);
        }
    });

    const exceptions = document.querySelectorAll('.exception-item');
    exceptions.forEach((item, index) => {
        const nameAr = item.querySelector('[name*="ExceptionNameAr"]')?.value;
        const nameEn = item.querySelector('[name*="ExceptionNameEn"]')?.value;
        const startDate = item.querySelector('[name*="StartDate"]')?.value;
        const endDate = item.querySelector('[name*="EndDate"]')?.value;
        const exceptionType = item.querySelector('[name*="ExceptionType"]')?.value;

        if (!nameAr || !nameEn || !startDate || !endDate) {
            errors.push(`الاستثناء ${index + 1}: يجب ملء جميع الحقول المطلوبة`);
        }

        if (exceptionType === '0') {
            const openTime = item.querySelector('[name*="OpeningTime"]')?.value;
            const closeTime = item.querySelector('[name*="ClosingTime"]')?.value;
            if (!openTime || !closeTime) {
                errors.push(`الاستثناء ${index + 1}: يجب تحديد وقت الفتح والإغلاق للساعات المخصصة`);
            }
        }

        if (startDate && endDate && new Date(startDate) > new Date(endDate)) {
            errors.push(`الاستثناء ${index + 1}: تاريخ البداية يجب أن يكون قبل تاريخ النهاية`);
        }
    });

    if (errors.length > 0) {
        e.preventDefault();
        showNotification('❌ ' + errors.join('\n'), 'error');
        return false;
    }

    prepareWorkingHoursForSubmission();
    return true;
}

function initFormValidation() {
    const form = document.getElementById('branchForm');
    if (!form) return;

    form.addEventListener('submit', validateForm);
}

// ============================================================================
// 9. NOTIFICATIONS & MODALS
// ============================================================================

function showNotification(message, type = 'info') {
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
            border-radius: 10px;
            box-shadow: 0 10px 30px rgba(0, 0, 0, 0.15);
            max-width: 420px;
            font-weight: 500;
            font-size: 14px;
            transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
            text-align: right;
            backdrop-filter: blur(10px);
            border: 1px solid rgba(255, 255, 255, 0.2);
            animation: slideInRight 0.3s cubic-bezier(0.4, 0, 0.2, 1);
        `;
        document.body.appendChild(notification);
    }

    const colors = {
        success: { bg: 'linear-gradient(135deg, #10b981 0%, #059669 100%)', text: '#ffffff' },
        error: { bg: 'linear-gradient(135deg, #ef4444 0%, #dc2626 100%)', text: '#ffffff' },
        warning: { bg: 'linear-gradient(135deg, #f59e0b 0%, #d97706 100%)', text: '#ffffff' },
        info: { bg: 'linear-gradient(135deg, #3b82f6 0%, #2563eb 100%)', text: '#ffffff' }
    };

    const color = colors[type] || colors.info;
    notification.style.background = color.bg;
    notification.style.color = color.text;
    notification.textContent = message;
    notification.style.opacity = '1';
    notification.style.transform = 'translateX(0)';

    clearTimeout(notification.hideTimeout);
    notification.hideTimeout = setTimeout(() => {
        notification.style.opacity = '0';
        notification.style.transform = 'translateX(400px)';
    }, 5000);
}

function showConfirmModal(message, onConfirm) {
    let overlay = document.getElementById('customConfirmOverlay');

    if (!overlay) {
        overlay = document.createElement('div');
        overlay.id = 'customConfirmOverlay';
        overlay.style.cssText = `
            position: fixed;
            top: 0; left: 0; right: 0; bottom: 0;
            background: rgba(0, 0, 0, 0.5);
            backdrop-filter: blur(4px);
            z-index: 10000;
            display: none;
            justify-content: center;
            align-items: center;
            animation: fadeIn 0.2s ease;
        `;
        document.body.appendChild(overlay);
    }

    let modal = document.getElementById('customConfirmModal');
    if (!modal) {
        modal = document.createElement('div');
        modal.id = 'customConfirmModal';
        overlay.appendChild(modal);
    }

    modal.innerHTML = '';
    modal.style.cssText = `
        background: white;
        padding: 32px;
        border-radius: 16px;
        box-shadow: 0 20px 60px rgba(0, 0, 0, 0.15);
        max-width: 420px;
        width: 90%;
        text-align: center;
        animation: slideUp 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    `;

    const messageP = document.createElement('p');
    messageP.textContent = message;
    messageP.style.cssText = `
        margin-bottom: 24px;
        font-size: 1.05rem;
        color: #1e293b;
        font-weight: 500;
        line-height: 1.5;
    `;
    modal.appendChild(messageP);

    const buttonContainer = document.createElement('div');
    buttonContainer.style.cssText = `
        display: flex;
        gap: 12px;
        justify-content: center;
    `;

    const confirmBtn = document.createElement('button');
    confirmBtn.textContent = 'نعم، احذف';
    confirmBtn.style.cssText = `
        padding: 11px 24px;
        background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%);
        color: white;
        border: none;
        border-radius: 8px;
        cursor: pointer;
        font-weight: 600;
        font-size: 14px;
        transition: all 0.2s;
        box-shadow: 0 4px 12px rgba(239, 68, 68, 0.3);
    `;
    confirmBtn.onmouseover = () => {
        confirmBtn.style.transform = 'translateY(-2px)';
        confirmBtn.style.boxShadow = '0 6px 20px rgba(239, 68, 68, 0.4)';
    };
    confirmBtn.onmouseout = () => {
        confirmBtn.style.transform = 'translateY(0)';
        confirmBtn.style.boxShadow = '0 4px 12px rgba(239, 68, 68, 0.3)';
    };
    confirmBtn.onclick = () => {
        overlay.style.animation = 'fadeOut 0.2s ease';
        setTimeout(() => {
            overlay.style.display = 'none';
            overlay.style.animation = 'fadeIn 0.2s ease';
        }, 200);
        onConfirm();
    };

    const cancelBtn = document.createElement('button');
    cancelBtn.textContent = 'إلغاء';
    cancelBtn.style.cssText = `
        padding: 11px 24px;
        background: #f1f5f9;
        color: #1e293b;
        border: 1px solid #e2e8f0;
        border-radius: 8px;
        cursor: pointer;
        font-weight: 600;
        font-size: 14px;
        transition: all 0.2s;
    `;
    cancelBtn.onmouseover = () => {
        cancelBtn.style.background = '#e2e8f0';
    };
    cancelBtn.onmouseout = () => {
        cancelBtn.style.background = '#f1f5f9';
    };
    cancelBtn.onclick = () => {
        overlay.style.animation = 'fadeOut 0.2s ease';
        setTimeout(() => {
            overlay.style.display = 'none';
            overlay.style.animation = 'fadeIn 0.2s ease';
        }, 200);
    };

    buttonContainer.appendChild(confirmBtn);
    buttonContainer.appendChild(cancelBtn);
    modal.appendChild(buttonContainer);

    overlay.style.display = 'flex';
}

// Add animation styles if not present
function addGlobalAnimationStyles() {
    if (!document.getElementById('branchFormAnimations')) {
        const style = document.createElement('style');
        style.id = 'branchFormAnimations';
        style.textContent = `
            @keyframes slideDown {
                from {
                    opacity: 0;
                    transform: translateY(-10px);
                }
                to {
                    opacity: 1;
                    transform: translateY(0);
                }
            }

            @keyframes slideUp {
                from {
                    opacity: 1;
                    transform: translateY(0);
                }
                to {
                    opacity: 0;
                    transform: translateY(-10px);
                }
            }

            @keyframes slideInRight {
                from {
                    opacity: 0;
                    transform: translateX(400px);
                }
                to {
                    opacity: 1;
                    transform: translateX(0);
                }
            }

            @keyframes fadeIn {
                from {
                    opacity: 0;
                }
                to {
                    opacity: 1;
                }
            }

            @keyframes fadeOut {
                from {
                    opacity: 1;
                }
                to {
                    opacity: 0;
                }
            }
        `;
        document.head.appendChild(style);
    }
}

// ============================================================================
// 10. INITIALIZATION
// ============================================================================

function initBranchForm() {
    console.log('🚀 Initializing Branch Form...');

    addGlobalAnimationStyles();

    initGovernorateAreaCascade();
    console.log('✓ Governorate/Area cascade initialized');

    initImagePreview();
    console.log('✓ Image preview initialized');

    initPhoneValidation();
    console.log('✓ Phone validation initialized');

    initFormValidation();
    console.log('✓ Form validation initialized');

    setTimeout(() => {
        initTimePickers();
        console.log('✓ Modern time pickers initialized');
    }, 100);

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
    console.log('✓ Working hours display set');

    document.querySelectorAll('.exception-item').forEach(item => {
        const index = item.dataset.index;
        const exceptionTypeInput = item.querySelector(`input[name="WorkingHourExceptions[${index}].ExceptionType"]`);
        if (!exceptionTypeInput) return;

        const typeValue = exceptionTypeInput.value;
        const customTimes = item.querySelector(`.custom-times`);
        const typeButtons = item.querySelectorAll('.type-btn');

        let targetButton = null;
        if (typeValue === '1') {
            targetButton = typeButtons[0];
            if (customTimes) customTimes.style.display = 'none';
        } else if (typeValue === '2') {
            targetButton = typeButtons[1];
            if (customTimes) customTimes.style.display = 'none';
        } else {
            targetButton = typeButtons[2];
            if (customTimes) customTimes.style.display = 'grid';
        }

        if (targetButton) {
            targetButton.classList.add('active');
        }
    });
    console.log('✓ Exception states initialized');

    console.log('✅ Branch form fully initialized');
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initBranchForm);
} else {
    initBranchForm();
}

// ============================================================================
// 11. EXPORT FUNCTIONS FOR INLINE HANDLERS
// ============================================================================

window.toggleDay = toggleDay;
window.copyFirstDayToAll = copyFirstDayToAll;
window.setExceptionType = setExceptionType;
window.addException = addException;
window.removeException = removeException;
window.deleteImage = deleteImage;
window.showNotification = showNotification;
window.showConfirmModal = showConfirmModal;