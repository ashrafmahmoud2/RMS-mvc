// Fixed Login Handler with embedded CSS
window.LoginHandler = (function () {
    'use strict';

    let currentInstance = null;
    let modalContainer = null;

    class LoginModalHandler {
        constructor(options = {}) {
            this.options = {
                onSuccess: options.onSuccess || function () { window.location.reload(); },
                onCancel: options.onCancel || function () { },
                onError: options.onError || function (error) { console.error('Login Error:', error); },
                redirectUrl: options.redirectUrl || null,
                showCancelButton: options.showCancelButton !== false,
                ...options
            };

            this.isLoading = false;
            this.currentPhone = '';
            this.elements = {};
        }

        init(modalElement) {
            if (!modalElement) {
                console.error('Login modal element not found');
                return false;
            }

            this.elements = {
                modal: modalElement,
                phoneInput: modalElement.querySelector('#phoneInput'),
                phoneInputWrapper: modalElement.querySelector('#phoneInputWrapper'),
                phoneError: modalElement.querySelector('#phoneError'),
                continueBtn: modalElement.querySelector('.phone-btn-continue'),
                cancelBtn: modalElement.querySelector('.phone-btn-cancel'),
                continueText: modalElement.querySelector('.continue-text'),
                form: modalElement.querySelector('form'),
                antiForgeryToken: modalElement.querySelector('input[name="__RequestVerificationToken"]')
            };

            if (!this.elements.phoneInput || !this.elements.continueBtn || !this.elements.form) {
                console.error('Required login elements not found');
                return false;
            }

            this.setupEventListeners();
            this.initializeButtonState();

            // Show modal
            this.elements.modal.style.display = 'flex';
            this.elements.modal.classList.add('show');

            // Focus phone input after animation
            setTimeout(() => {
                if (this.elements.phoneInput) {
                    this.elements.phoneInput.focus();
                }
            }, 300);

            return true;
        }

        setupEventListeners() {
            if (this.elements.phoneInput) {
                this.elements.phoneInput.addEventListener('input', (e) => this.handlePhoneInput(e));
                this.elements.phoneInput.addEventListener('focus', () => this.updateButtonState());
                this.elements.phoneInput.addEventListener('blur', () => this.updateButtonState());
            }

            if (this.elements.form) {
                this.elements.form.addEventListener('submit', (e) => this.handleFormSubmit(e));
            }

            if (this.elements.cancelBtn && this.options.showCancelButton) {
                this.elements.cancelBtn.addEventListener('click', () => this.handleCancel());
            } else if (this.elements.cancelBtn) {
                this.elements.cancelBtn.style.display = 'none';
            }

            // Close on escape key
            document.addEventListener('keydown', (e) => {
                if (e.key === 'Escape' && this.elements.modal?.classList.contains('show')) {
                    this.handleCancel();
                }
            });

            // Close on backdrop click
            if (this.elements.modal) {
                this.elements.modal.addEventListener('click', (e) => {
                    if (e.target === this.elements.modal) {
                        this.handleCancel();
                    }
                });
            }
        }

        handlePhoneInput(e) {
            let value = e.target.value.replace(/\D/g, '');
            if (value.length > 11) value = value.slice(0, 11);
            e.target.value = value;

            this.clearError();
            this.updateButtonState();
            this.validatePhone(value);
        }

        async handleFormSubmit(e) {
            e.preventDefault();

            const phone = this.elements.phoneInput.value.trim();
            if (!this.validatePhone(phone)) return;

            this.currentPhone = phone;

            const existingToken = localStorage.getItem('otp_token');
            const existingPhone = localStorage.getItem('otp_phone');

            if (existingToken && existingPhone === phone && this.isTokenValid(existingToken)) {
                await this.autoSignIn(phone, existingToken);
            } else {
                if (existingPhone !== phone) {
                    localStorage.removeItem('otp_token');
                    localStorage.removeItem('otp_phone');
                }
                await this.sendOTP(phone);
            }
        }

        handleCancel() {
            this.hide();
            this.options.onCancel();
        }

        updateButtonState() {
            const phone = this.elements.phoneInput?.value || '';
            const existingToken = localStorage.getItem('otp_token');
            const existingPhone = localStorage.getItem('otp_phone');

            if (existingToken &&
                existingPhone === phone &&
                phone.length === 11 &&
                phone.startsWith('01') &&
                this.isTokenValid(existingToken)) {

                if (this.elements.continueText) {
                    this.elements.continueText.textContent = 'تسجيل الدخول التلقائي';
                }
                if (this.elements.phoneInputWrapper) {
                    this.elements.phoneInputWrapper.classList.add('success');
                }
            } else {
                if (this.elements.continueText) {
                    this.elements.continueText.textContent = 'استمر';
                }
                if (this.elements.phoneInputWrapper) {
                    this.elements.phoneInputWrapper.classList.remove('success');
                }

                if (existingPhone && existingPhone !== phone) {
                    localStorage.removeItem('otp_token');
                    localStorage.removeItem('otp_phone');
                }
            }
        }

        initializeButtonState() {
            const initialPhone = this.elements.phoneInput?.value.trim() || '';
            if (initialPhone) {
                this.updateButtonState();
            }
        }

        isTokenValid(token) {
            try {
                const data = atob(token);
                const parts = data.split(':');
                if (parts.length !== 2) return false;

                const timestamp = parseInt(parts[1]);
                const tokenTime = new Date(timestamp * 1000);
                const now = new Date();

                return (now - tokenTime) < (24 * 60 * 60 * 1000);
            } catch {
                return false;
            }
        }

        validatePhone(phone) {
            if (!phone) return false;
            if (phone.length < 11) return true;

            if (!/^01[0125][0-9]{8}$/.test(phone)) {
                this.showError('رقم الهاتف يجب أن يبدأ بـ 010 أو 011 أو 012 أو 015 ويكون 11 رقم');
                return false;
            }

            return true;
        }

        // Simplified sendOTP method for LoginModalHandler
        async sendOTP(phone) {
            this.setLoading(true, 'جاري الإرسال...');

            try {
                const formData = new FormData();
                formData.append('PhoneNumber', phone);
                if (this.elements.antiForgeryToken) {
                    formData.append('__RequestVerificationToken', this.elements.antiForgeryToken.value);
                }

                const response = await fetch('/Account/SendOtp', {
                    method: 'POST',
                    body: formData
                });

                if (response.ok) {
                    // Hide login modal
                    this.hide();

                    // Get the OTP HTML content
                    const otpHtml = await response.text();

                    // Find OTP modal container
                    let otpModal = document.getElementById("otpModal");
                    if (!otpModal) {
                        console.error('OTP modal container not found in DOM');
                        this.showError('خطأ في عرض نافذة التحقق');
                        return;
                    }

                    // Insert the OTP content and show modal
                    const otpContent = otpModal.querySelector("#otpContent");
                    if (otpContent) {
                        otpContent.innerHTML = otpHtml;
                        otpModal.style.display = "flex";
                        otpModal.classList.add('show');

                        // Create and initialize OTP handler
                        const otpHandler = new window.OtpHandler.OtpModalHandler({
                            onSuccess: (result, formData) => {
                                // Generate and store token for future use
                                const token = this.generateOtpToken(phone);
                                localStorage.setItem('otp_token', token);
                                localStorage.setItem('otp_phone', phone);

                                // Hide OTP modal
                                window.OtpHandler.hide();

                                // Call login success callback
                                this.options.onSuccess(result, formData);
                            },
                            onCancel: () => {
                                // Hide OTP modal and show login modal again
                                window.OtpHandler.hide();
                                this.show();
                            },
                            onError: (error) => {
                                console.error('OTP Error:', error);
                                this.options.onError(error);
                            }
                        });

                        // Initialize the OTP modal
                        setTimeout(() => {
                            otpHandler.init(otpContent);
                        }, 100);

                    } else {
                        console.error('OTP content container not found');
                        this.showError('خطأ في عرض نافذة التحقق');
                    }

                } else {
                    const errorResult = await response.json().catch(() => ({}));
                    this.showError(errorResult.message || 'فشل إرسال رمز التحقق');
                }
            } catch (error) {
                console.error('Error sending OTP:', error);
                this.showError('حدث خطأ في إرسال رمز التحقق');
                this.options.onError(error);
            } finally {
                this.setLoading(false, 'استمر');
            }
        }

        generateOtpToken(phone) {
            const data = `${phone}:${Math.floor(Date.now() / 1000)}`;
            return btoa(data);
        }

        async autoSignIn(phone, token) {
            this.setLoading(true, 'جاري تسجيل الدخول...');

            try {
                const response = await fetch('/Account/AutoSignIn', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': this.elements.antiForgeryToken?.value
                    },
                    body: JSON.stringify({ phoneNumber: phone, otpToken: token })
                });

                const result = await response.json();

                if (result.success) {
                    this.hide();
                    this.options.onSuccess(result);
                } else {
                    localStorage.removeItem('otp_token');
                    localStorage.removeItem('otp_phone');
                    this.showError('انتهت صلاحية الجلسة، يرجى المحاولة مرة أخرى');
                    if (this.elements.continueText) {
                        this.elements.continueText.textContent = 'استمر';
                    }
                }
            } catch (error) {
                console.error('Error:', error);
                localStorage.removeItem('otp_token');
                localStorage.removeItem('otp_phone');
                this.showError('حدث خطأ، يرجى المحاولة مرة أخرى');
                if (this.elements.continueText) {
                    this.elements.continueText.textContent = 'استمر';
                }
                this.options.onError(error);
            } finally {
                this.setLoading(false, 'استمر');
            }
        }

        setLoading(loading, text) {
            this.isLoading = loading;
            if (this.elements.continueBtn) {
                this.elements.continueBtn.disabled = loading;
            }

            if (this.elements.continueText) {
                if (loading) {
                    this.elements.continueText.innerHTML = `<div class="loading-spinner" style="width:18px;height:18px;border:2px solid transparent;border-top:2px solid currentColor;border-radius:50%;animation:spin 1s linear infinite;display:inline-block;margin-left:8px;"></div> ${text}`;
                } else {
                    this.elements.continueText.textContent = text;
                }
            }
        }

        showError(message) {
            if (this.elements.phoneError) {
                this.elements.phoneError.textContent = message;
                this.elements.phoneError.style.display = 'block';
            }

            if (this.elements.phoneInputWrapper) {
                this.elements.phoneInputWrapper.classList.add('error');
                this.elements.phoneInputWrapper.classList.remove('success');
            }
        }

        clearError() {
            if (this.elements.phoneError) {
                this.elements.phoneError.style.display = 'none';
            }

            if (this.elements.phoneInputWrapper) {
                this.elements.phoneInputWrapper.classList.remove('error');
            }
        }

        show() {
            if (this.elements.modal) {
                this.elements.modal.style.display = 'flex';
                this.elements.modal.classList.add('show');
                setTimeout(() => {
                    if (this.elements.phoneInput) {
                        this.elements.phoneInput.focus();
                    }
                }, 300);
            }
        }

        hide() {
            if (this.elements.modal) {
                this.elements.modal.style.display = 'none';
                this.elements.modal.classList.remove('show');
            }
        }

        destroy() {
            this.hide();
            this.elements = {};
        }
    }

    // Get anti-forgery token from existing form or page
    function getAntiForgeryToken() {
        const existingToken = document.querySelector('input[name="__RequestVerificationToken"]');
        if (existingToken) {
            return existingToken.value;
        }
        return '';
    }

    // Static methods for global access
    return {
        // Create and show login modal
        async show(options = {}) {
            try {
                // Create modal container if it doesn't exist
                if (!modalContainer) {
                    // Add styles first
                    this.addStyles();

                    // Get the modal HTML content from server
                    const response = await fetch('/Account/GetLoginModal', {
                        method: 'GET',
                        headers: {
                            'X-Requested-With': 'XMLHttpRequest'
                        }
                    });

                    if (!response.ok) {
                        throw new Error('Failed to load login modal');
                    }

                    const html = await response.text();

                    // Create modal container
                    modalContainer = document.createElement('div');
                    modalContainer.className = 'modal-overlay';
                    modalContainer.id = 'loginModal';
                    modalContainer.style.display = 'none';
                    modalContainer.innerHTML = `<div class="phone-modal">${html}</div>`;

                    document.body.appendChild(modalContainer);
                }

                // Create new handler instance
                currentInstance = new LoginModalHandler(options);

                // Initialize and show modal
                if (currentInstance.init(modalContainer)) {
                    return currentInstance;
                } else {
                    currentInstance = null;
                    return null;
                }

            } catch (error) {
                console.error('Error showing login modal:', error);
                if (options.onError) {
                    options.onError(error);
                }
                return null;
            }
        },

        // Hide modal
        hide() {
            if (currentInstance) {
                currentInstance.destroy();
                currentInstance = null;
            }

            if (modalContainer) {
                modalContainer.classList.remove('show');
                modalContainer.style.display = 'none';
            }
        },

        // Get current instance
        getCurrentInstance() {
            return currentInstance;
        },

        // Add required styles to document
        addStyles() {
            const styleId = 'login-modal-styles';
            if (document.querySelector('#' + styleId)) return;

            const styles = document.createElement('style');
            styles.id = styleId;
            styles.textContent = `
                :root {
                    --primary-color: #185499;
                    --primary-dark: #0f3d7a;
                    --primary-light: #3e7bb5;
                    --success-color: #10b981;
                    --danger-color: #ef4444;
                    --warning-color: #f59e0b;
                    --info-color: #3b82f6;
                    --background: #f8fafc;
                    --surface: #ffffff;
                    --surface-hover: #f1f5f9;
                    --border: #e2e8f0;
                    --border-focus: #3b82f6;
                    --text-primary: #1e293b;
                    --text-secondary: #64748b;
                    --text-muted: #94a3b8;
                    --shadow-sm: 0 1px 3px rgba(0, 0, 0, 0.1);
                    --shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
                    --shadow-lg: 0 8px 25px rgba(0, 0, 0, 0.15);
                    --border-radius: 16px;
                    --border-radius-lg: 24px;
                    --transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
                    --transition-spring: all 0.4s cubic-bezier(0.34, 1.56, 0.64, 1);
                    --transition-bounce: all 0.5s cubic-bezier(0.68, -0.55, 0.265, 1.55);
                }

                .modal-overlay {
                    position: fixed;
                    top: 0;
                    left: 0;
                    right: 0;
                    bottom: 0;
                    background: rgba(0, 0, 0, 0.6);
                    backdrop-filter: blur(12px);
                    z-index: 1000;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    opacity: 0;
                    visibility: hidden;
                    transition: all 0.4s ease-out;
                    direction: rtl;
                }

                .modal-overlay.show {
                    opacity: 1;
                    visibility: visible;
                }

                .phone-modal {
                    background: var(--surface);
                    border-radius: var(--border-radius-lg);
                    padding: 32px;
                    max-width: 440px;
                    width: 90%;
                    box-shadow: var(--shadow-lg);
                    border: 1px solid var(--border);
                    position: relative;
                    transform: translateY(50px) scale(0.9);
                    transition: all 0.6s cubic-bezier(0.34, 1.56, 0.64, 1);
                }

                .modal-overlay.show .phone-modal {
                    transform: translateY(0) scale(1);
                }

                .phone-header {
                    text-align: center;
                    margin-bottom: 32px;
                }

                .phone-icon {
                    width: 72px;
                    height: 72px;
                    background: linear-gradient(135deg, var(--primary-color) 0%, var(--primary-dark) 100%);
                    border-radius: 50%;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    margin: 0 auto 20px;
                    font-size: 28px;
                    color: white;
                    box-shadow: 0 8px 32px rgba(24, 84, 153, 0.3);
                    position: relative;
                    overflow: hidden;
                    animation: iconPulse 3s infinite;
                }

                .phone-title {
                    font-size: 24px;
                    font-weight: 700;
                    color: var(--text-primary);
                    margin-bottom: 12px;
                    letter-spacing: -0.5px;
                }

                .phone-description {
                    color: var(--text-secondary);
                    font-size: 15px;
                    line-height: 1.6;
                    max-width: 320px;
                    margin: 0 auto;
                }

                .phone-input-container {
                    margin: 32px 0;
                    position: relative;
                }

                .input-label {
                    position: absolute;
                    top: -12px;
                    right: 20px;
                    background: var(--surface);
                    padding: 0 12px;
                    font-size: 13px;
                    font-weight: 600;
                    color: var(--text-secondary);
                    transition: var(--transition);
                    z-index: 2;
                    border-radius: 4px;
                }

                .phone-input-wrapper {
                    position: relative;
                    display: flex;
                    align-items: center;
                    background: var(--surface);
                    border: 3px solid var(--border);
                    border-radius: 16px;
                    transition: var(--transition-spring);
                    overflow: hidden;
                    backdrop-filter: blur(4px);
                    box-shadow: var(--shadow-sm);
                }

                .phone-input-wrapper:focus-within {
                    border-color: var(--primary-color);
                    box-shadow: 0 0 0 6px rgba(24, 84, 153, 0.12), var(--shadow);
                    transform: scale(1.02) translateY(-2px);
                    background: rgba(255, 255, 255, 0.9);
                }

                .phone-input-wrapper.error {
                    border-color: var(--danger-color);
                    box-shadow: 0 0 0 6px rgba(239, 68, 68, 0.15);
                    animation: errorShake 0.6s ease-in-out;
                }

                .phone-input-wrapper.success {
                    border-color: var(--success-color);
                    background: rgba(16, 185, 129, 0.08);
                    box-shadow: 0 0 0 6px rgba(16, 185, 129, 0.15);
                    animation: successBounce 0.5s ease-out;
                }

                .phone-input {
                    flex: 1;
                    border: none;
                    outline: none;
                    padding: 18px 20px;
                    font-size: 17px;
                    font-weight: 600;
                    background: transparent;
                    color: var(--text-primary);
                    direction: ltr;
                    text-align: left;
                    transition: var(--transition);
                    letter-spacing: 0.5px;
                }

                .phone-input::placeholder {
                    color: var(--text-muted);
                    font-weight: 400;
                    transition: var(--transition);
                }

                .phone-error {
                    background: linear-gradient(135deg, rgba(239, 68, 68, 0.1) 0%, rgba(239, 68, 68, 0.15) 100%);
                    color: var(--danger-color);
                    padding: 14px 18px;
                    border-radius: 12px;
                    margin: 16px 0;
                    font-size: 14px;
                    font-weight: 500;
                    text-align: center;
                    border: 2px solid rgba(239, 68, 68, 0.2);
                    display: none;
                    animation: errorSlideIn 0.4s ease-out;
                    box-shadow: var(--shadow-sm);
                    backdrop-filter: blur(4px);
                }

                .phone-actions {
                    display: flex;
                    gap: 16px;
                    margin-top: 32px;
                }

                .phone-btn {
                    flex: 1;
                    padding: 16px 24px;
                    border: none;
                    border-radius: 14px;
                    font-weight: 600;
                    cursor: pointer;
                    transition: var(--transition-spring);
                    font-family: inherit;
                    font-size: 16px;
                    position: relative;
                    overflow: hidden;
                    letter-spacing: 0.3px;
                }

                .phone-btn-cancel {
                    background: var(--surface-hover);
                    color: var(--text-secondary);
                    border: 2px solid var(--border);
                    box-shadow: var(--shadow-sm);
                }

                .phone-btn-cancel:hover {
                    background: var(--border);
                    transform: translateY(-3px);
                    box-shadow: var(--shadow);
                    border-color: var(--text-muted);
                }

                .phone-btn-continue {
                    background: linear-gradient(135deg, var(--primary-color) 0%, var(--primary-dark) 100%);
                    color: white;
                    position: relative;
                    overflow: hidden;
                    box-shadow: 0 6px 20px rgba(24, 84, 153, 0.3);
                }

                .phone-btn-continue:hover:not(:disabled) {
                    transform: translateY(-4px);
                    box-shadow: 0 12px 35px rgba(24, 84, 153, 0.4);
                    filter: brightness(1.1);
                }

                .phone-btn-continue:disabled {
                    opacity: 0.6;
                    cursor: not-allowed;
                    transform: none;
                    filter: grayscale(0.3);
                }

                @keyframes iconPulse {
                    0%, 100% {
                        box-shadow: 0 8px 32px rgba(24, 84, 153, 0.3);
                    }
                    50% {
                        box-shadow: 0 8px 32px rgba(24, 84, 153, 0.5), 0 0 0 8px rgba(24, 84, 153, 0.1);
                    }
                }

                @keyframes errorShake {
                    0%, 100% {
                        transform: translateX(0);
                    }
                    10%, 30%, 50%, 70%, 90% {
                        transform: translateX(-8px);
                    }
                    20%, 40%, 60%, 80% {
                        transform: translateX(8px);
                    }
                }

                @keyframes successBounce {
                    0% {
                        transform: scale(1);
                    }
                    50% {
                        transform: scale(1.05);
                    }
                    100% {
                        transform: scale(1);
                    }
                }

                @keyframes errorSlideIn {
                    from {
                        opacity: 0;
                        transform: translateY(-20px);
                        max-height: 0;
                    }
                    to {
                        opacity: 1;
                        transform: translateY(0);
                        max-height: 100px;
                    }
                }

                @keyframes spin {
                    0% {
                        transform: rotate(0deg);
                    }
                    100% {
                        transform: rotate(360deg);
                    }
                }

                @media (max-width: 480px) {
                    .phone-modal {
                        padding: 28px 24px;
                        max-width: 100%;
                        margin: 16px;
                    }

                    .phone-icon {
                        width: 64px;
                        height: 64px;
                        font-size: 24px;
                    }

                    .phone-title {
                        font-size: 22px;
                    }

                    .phone-btn {
                        padding: 18px 24px;
                    }
                }
            `;
            document.head.appendChild(styles);
        },

        // Utility method to check if user should be prompted to login
        requireLogin(options = {}) {
            const isLoggedIn = document.body.hasAttribute('data-user-authenticated') ||
                window.userAuthenticated ||
                localStorage.getItem('otp_token');

            if (!isLoggedIn) {
                return this.show(options);
            }

            return null;
        }
    };
})();