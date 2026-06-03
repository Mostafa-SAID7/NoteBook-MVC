/**
 * AJAX Form Submission Handler
 * Handle form submissions via AJAX with validation and feedback
 */

class AJAXForm {
  constructor(formSelector) {
    this.form = typeof formSelector === 'string' ? document.querySelector(formSelector) : formSelector;
    if (!this.form) throw new Error(`Form not found: ${formSelector}`);

    this.method = this.form.getAttribute('method') || 'POST';
    this.action = this.form.getAttribute('action') || '';
    this.submitButton = this.form.querySelector('[type="submit"]');
    this.callbacks = {
      beforeSubmit: null,
      onSuccess: null,
      onError: null,
      onComplete: null
    };

    this.init();
  }

  init() {
    this.form.addEventListener('submit', (e) => {
      e.preventDefault();
      this.submit();
    });
  }

  /**
   * Register callback
   */
  on(event, callback) {
    if (this.callbacks.hasOwnProperty(event)) {
      this.callbacks[event] = callback;
    }
    return this;
  }

  /**
   * Submit form via AJAX
   */
  async submit() {
    try {
      // Call before submit callback
      if (this.callbacks.beforeSubmit) {
        const result = await this.callbacks.beforeSubmit(this.form);
        if (result === false) return;
      }

      // Show loading state
      this.showLoading();

      // Serialize form data
      const formData = UI.serializeForm(this.form);

      // Make AJAX request
      const response = await API.request(this.action, {
        method: this.method,
        body: formData
      });

      // Call success callback
      if (this.callbacks.onSuccess) {
        await this.callbacks.onSuccess(response);
      }

      // Show success message
      const successMessage = this.form.getAttribute('data-success-message') || 'Success!';
      Alert.show(successMessage, 'success');

      // Reset form if configured
      if (this.form.getAttribute('data-reset') === 'true') {
        this.form.reset();
      }

      // Redirect if configured
      const redirect = this.form.getAttribute('data-redirect');
      if (redirect) {
        window.location.href = redirect;
      }
    } catch (error) {
      // Call error callback
      if (this.callbacks.onError) {
        await this.callbacks.onError(error);
      }

      // Handle field errors
      if (error.data && typeof error.data === 'object') {
        this.showFieldErrors(error.data);
      }

      // Show error message
      const errorMessage = error.message || 'An error occurred';
      Alert.show(errorMessage, 'danger');
    } finally {
      // Hide loading state
      this.hideLoading();

      // Call complete callback
      if (this.callbacks.onComplete) {
        await this.callbacks.onComplete();
      }
    }
  }

  /**
   * Show loading state
   */
  showLoading() {
    if (this.submitButton) {
      UI.disableButton(this.submitButton);
      this.submitButton.setAttribute('data-loading', 'true');
      const originalText = this.submitButton.textContent;
      this.submitButton.setAttribute('data-original-text', originalText);
      this.submitButton.textContent = 'Loading...';
    }
  }

  /**
   * Hide loading state
   */
  hideLoading() {
    if (this.submitButton) {
      UI.enableButton(this.submitButton);
      this.submitButton.removeAttribute('data-loading');
      const originalText = this.submitButton.getAttribute('data-original-text') || 'Submit';
      this.submitButton.textContent = originalText;
    }
  }

  /**
   * Show field errors
   */
  showFieldErrors(errors) {
    // Clear previous errors
    this.form.querySelectorAll('.is-invalid').forEach((field) => {
      field.classList.remove('is-invalid');
    });

    // Show new errors
    for (const [fieldName, messages] of Object.entries(errors)) {
      const field = this.form.querySelector(`[name="${fieldName}"]`);
      if (field) {
        field.classList.add('is-invalid');
        const feedback = field.nextElementSibling;
        if (feedback && feedback.classList.contains('invalid-feedback')) {
          feedback.textContent = Array.isArray(messages) ? messages[0] : messages;
        }
      }
    }
  }

  /**
   * Get form data
   */
  getData() {
    return UI.serializeForm(this.form);
  }

  /**
   * Set form data
   */
  setData(data) {
    for (const [key, value] of Object.entries(data)) {
      const field = this.form.querySelector(`[name="${key}"]`);
      if (field) {
        if (field.type === 'checkbox' || field.type === 'radio') {
          field.checked = value;
        } else {
          field.value = value;
        }
      }
    }
  }

  /**
   * Reset form
   */
  reset() {
    this.form.reset();
    this.form.querySelectorAll('.is-invalid, .is-valid').forEach((field) => {
      field.classList.remove('is-invalid', 'is-valid');
    });
  }

  /**
   * Enable form
   */
  enable() {
    this.form.querySelectorAll('input, textarea, select, button').forEach((element) => {
      element.disabled = false;
    });
  }

  /**
   * Disable form
   */
  disable() {
    this.form.querySelectorAll('input, textarea, select, button').forEach((element) => {
      element.disabled = true;
    });
  }
}

// Auto-initialize AJAX forms
document.addEventListener('DOMContentLoaded', () => {
  document.querySelectorAll('form[data-ajax-form]').forEach((form, index) => {
    form.setAttribute('data-id', `ajax-form-${index}`);
    new AJAXForm(`[data-id="ajax-form-${index}"]`);
  });
});

// Export
if (typeof module !== 'undefined' && module.exports) {
  module.exports = { AJAXForm };
}
