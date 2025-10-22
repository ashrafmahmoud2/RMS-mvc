window.OtpHandler = (function () {
    'use strict';

    let currentInstance = null;

    class OtpModalHandler {
        constructor(options = {}) {
            this.options = {
                onSuccess: options.onSuccess || function () { },
                onCancel: options.onCancel || function () { },
                onError: options.onError || function (error) { console.error('OTP Error:', error); },
                maxAttempts: options.maxAttempts || 5,
                timerDuration: options.timerDuration || 60,
                ...options
            };

            this.attempts = this.options.maxAttempts;
            this.timeLeft = this.options.timerDuration;
            this.timerInterval = null;
            this.isSubmitting = false;
            this.isInitialized = false;
            this.elements = {};
        }

        init(otpContentElement) {
            if (!otpContentElement) {
                console.error('OTP content element not found');
                return false;
            }

            // Cache DOM elements
            this.elements = {
                otpInputs: otpContentElement.querySelectorAll('.otp-input'),
                otpHiddenInput: otpContentElement.querySelector('#Otp'),
                phoneHiddenInput: otpContentElement.querySelector('#PhoneNumber'),
                verifyBtn: otpContentElement.querySelector('#verifyOtp'),
                otpForm: otpContentElement.querySelector('#otpForm'),
                cancelBtn: otpContentElement.querySelector('#cancelOtp'),
                resendBtn: otpContentElement.querySelector('#resendBtn'),
                otpError: otpContentElement.querySelector('#otpError'),
                timerElement: otpContentElement.querySelector('#timerCount'),
                attemptsElement: otpContentElement.querySelector('#attemptsCount'),
                otpContent: otpContentElement.querySelector('#otpContent'),
                blockedContent: otpContentElement.querySelector('#blockedContent')
            };

            if (!this.elements.otpInputs.length || !this.elements.otpHiddenInput ||
                !this.elements.verifyBtn || !this.elements.otpForm) {
                console.error('Required OTP elements not found');
                return false;
            }

            this.setupEventListeners();
            this.startTimer();
            this.focusFirstInput();
            this.updateOTPValue();
            this.isInitialized = true;

            console.log('✅ OTP modal initialized successfully');
            return true;
        }

        setupEventListeners() {
            // OTP input handlers
            Array.from(this.elements.otpInputs).forEach((input, index) => {
                input.addEventListener('input', (e) => this.handleOtpInput(e, index));
                input.addEventListener('keydown', (e) => this.handleKeydown(e, index));
                input.addEventListener('paste', (e) => this.handlePaste(e, index));
            });

            // Form submission
            if (this.elements.otpForm) {
                this.elements.otpForm.addEventListener('submit', (e) => this.handleFormSubmit(e));
            }

            // Cancel button
            if (this.elements.cancelBtn) {
                this.elements.cancelBtn.addEventListener('click', () => this.handleCancel());
            }

            // Resend button
            if (this.elements.resendBtn) {
                this.elements.resendBtn.addEventListener('click', () => this.handleResend());
            }
        }

        handleOtpInput(e, index) {
            const value = e.target.value.replace(/[^0-9]/g, '');
            e.target.value = value.length > 1 ? value.charAt(0) : value;

            if (value && index < this.elements.otpInputs.length - 1) {
                this.elements.otpInputs[index + 1].focus();
            }

            this.updateOTPValue();
        }

        handleKeydown(e, index) {
            if (e.key === 'Backspace' && !e.target.value && index > 0) {
                this.elements.otpInputs[index - 1].focus();
            }
        }

        handlePaste(e, index) {
            e.preventDefault();
            const pastedData = e.clipboardData.getData('text').replace(/[^0-9]/g, '');

            for (let i = 0; i < pastedData.length && (index + i) < this.elements.otpInputs.length; i++) {
                this.elements.otpInputs[index + i].value = pastedData[i];
            }

            this.updateOTPValue();
        }

        async handleFormSubmit(e) {
            e.preventDefault();

            if (this.isSubmitting) return;

            this.updateOTPValue();
            const otpValue = this.elements.otpHiddenInput.value;

            if (otpValue.length !== 4) {
                this.showError('يرجى إدخال رمز التحقق كاملاً');
                return;
            }

            this.showLoading();

            try {
                const formData = new FormData(this.elements.otpForm);
                const response = await fetch('/Account/VerifyOtp', {
                    method: 'POST',
                    body: formData
                });

                const result = await response.json();

                if (result.success) {
                    // ✅ Handle redirect URL from server
                    if (result.redirectUrl) {
                      

                        // Redirect after a short delay to show the message
                        setTimeout(() => {
                            window.location.href = result.redirectUrl;
                        }, 500);
                    } else {
                        // Fallback: call onSuccess callback
                        this.options.onSuccess(result, formData);
                    }
                }
                else {
                    this.attempts--;

                    if (this.elements.attemptsElement) {
                        this.elements.attemptsElement.textContent = this.attempts;
                    }

                    if (this.attempts <= 0) {
                        this.showBlockedState();
                    } else {
                        this.showError(result.message || 'رمز التحقق غير صحيح');
                        this.clearInputs();
                    }
                }
            } catch (error) {
                this.showError('حدث خطأ أثناء التحقق من الرمز');
                this.options.onError(error);
            } finally {
                this.hideLoading();
            }
        }

        handleCancel() {
            this.cleanup();
            this.options.onCancel();
        }

        async handleResend() {
            if (!this.elements.phoneHiddenInput?.value) return;

            try {
                const formData = new FormData();
                formData.append("PhoneNumber", this.elements.phoneHiddenInput.value);

                const response = await fetch("/Account/SendOtp", {
                    method: "POST",
                    body: formData
                });

                if (response.ok) {
                    this.timeLeft = this.options.timerDuration;
                    this.attempts = this.options.maxAttempts;

                    if (this.elements.attemptsElement) {
                        this.elements.attemptsElement.textContent = this.attempts;
                    }

                    this.clearInputs();
                    this.hideError();
                    this.startTimer();

                    this.showSuccess('تم إرسال رمز جديد');
                } else {
                    this.showError('فشل إرسال رمز جديد');
                }
            } catch (error) {
                this.showError('حدث خطأ أثناء إعادة الإرسال');
            }
        }

        updateOTPValue() {
            if (!this.elements.otpInputs.length || !this.elements.otpHiddenInput) return;

            const otpValue = Array.from(this.elements.otpInputs).map(input => input.value).join('');
            this.elements.otpHiddenInput.value = otpValue;

            if (this.elements.verifyBtn) {
                this.elements.verifyBtn.disabled = otpValue.length !== 4 || this.isSubmitting;
            }

            if (otpValue.length === 4) {
                this.hideError();
            }
        }

        showLoading() {
            this.isSubmitting = true;

            if (this.elements.verifyBtn) {
                const verifyText = this.elements.verifyBtn.querySelector('.verify-text');
                const loadingSpinner = this.elements.verifyBtn.querySelector('.loading-spinner');

                if (verifyText) verifyText.style.display = 'none';
                if (loadingSpinner) loadingSpinner.style.display = 'inline-block';

                this.elements.verifyBtn.disabled = true;
            }
        }

        hideLoading() {
            this.isSubmitting = false;

            if (this.elements.verifyBtn) {
                const verifyText = this.elements.verifyBtn.querySelector('.verify-text');
                const loadingSpinner = this.elements.verifyBtn.querySelector('.loading-spinner');

                if (verifyText) verifyText.style.display = 'inline';
                if (loadingSpinner) loadingSpinner.style.display = 'none';
            }

            this.updateOTPValue();
        }

        showError(message) {
            if (this.elements.otpError) {
                this.elements.otpError.textContent = message;
                this.elements.otpError.style.display = 'block';
            }

            Array.from(this.elements.otpInputs || []).forEach(input => {
                input.classList.add('error');
            });
        }

        hideError() {
            if (this.elements.otpError) {
                this.elements.otpError.style.display = 'none';
            }

            Array.from(this.elements.otpInputs || []).forEach(input => {
                input.classList.remove('error');
            });
        }

        showSuccess(message) {
            console.log('Success:', message);
        }

        clearInputs() {
            Array.from(this.elements.otpInputs || []).forEach(input => {
                input.value = '';
                input.classList.remove('error');
            });

            this.updateOTPValue();
            this.focusFirstInput();
        }

        showBlockedState() {
            if (this.elements.otpContent) {
                this.elements.otpContent.style.display = 'none';
            }

            if (this.elements.blockedContent) {
                this.elements.blockedContent.style.display = 'block';
            }
        }

        startTimer() {
            this.clearTimer();

            this.timerInterval = setInterval(() => {
                this.timeLeft--;

                if (this.elements.timerElement) {
                    this.elements.timerElement.textContent = this.timeLeft;
                }

                if (this.timeLeft <= 0) {
                    this.enableResend();
                }
            }, 1000);
        }

        enableResend() {
            this.clearTimer();

            if (this.elements.resendBtn) {
                this.elements.resendBtn.disabled = false;
                this.elements.resendBtn.textContent = 'إعادة الإرسال';
            }
        }

        clearTimer() {
            if (this.timerInterval) {
                clearInterval(this.timerInterval);
                this.timerInterval = null;
            }
        }

        focusFirstInput() {
            if (this.elements.otpInputs?.length > 0) {
                this.elements.otpInputs[0].focus();
            }
        }

        cleanup() {
            this.clearTimer();
            this.isInitialized = false;
        }

        destroy() {
            this.cleanup();
            this.elements = {};
        }
    }

    // Return public interface
    return {
        // Expose the class for direct instantiation
        OtpModalHandler: OtpModalHandler,

        // Create and show OTP modal - simplified version
        async show(phoneNumber, options = {}) {
            try {
                const formData = new FormData();
                formData.append("PhoneNumber", phoneNumber);

                const response = await fetch("/Account/SendOtp", {
                    method: "POST",
                    body: formData
                });

                if (!response.ok) {
                    throw new Error('Failed to send OTP');
                }

                const html = await response.text();
                let otpModal = document.getElementById("otpModal");

                if (!otpModal) {
                    console.error('OTP modal container not found');
                    return null;
                }

                const otpContent = otpModal.querySelector("#otpContent");
                if (otpContent) {
                    otpContent.innerHTML = html;
                    otpModal.style.display = "flex";
                    otpModal.classList.add('show');

                    currentInstance = new OtpModalHandler(options);

                    setTimeout(() => {
                        if (currentInstance) {
                            currentInstance.init(otpContent);
                        }
                    }, 100);

                    return currentInstance;
                } else {
                    console.error('OTP content container not found');
                    return null;
                }

            } catch (error) {
                console.error('Error showing OTP modal:', error);
                if (options.onError) {
                    options.onError(error);
                }
                return null;
            }
        },

        hide() {
            const otpModal = document.getElementById("otpModal");
            if (otpModal) {
                otpModal.style.display = "none";
                otpModal.classList.remove('show');
            }

            if (currentInstance) {
                currentInstance.destroy();
                currentInstance = null;
            }
        },

        getCurrentInstance() {
            return currentInstance;
        }
    };
})();