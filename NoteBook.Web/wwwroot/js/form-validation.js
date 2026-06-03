/**
 * Form Validation Component
 * Client-side form validation with real-time feedback
 */

class FormValidator {
  constructor(formSelector) {
    this.form = document.querySelector(formSelector);
    if (!this.form) throw new Error(`Form not found: ${formSelector}`);

    this.fields = {};
    this.errors = {};
    this.init();
  }

  init() {
    // Collect all form fields
    this.form.querySelectorAll('[data-validate]').forEach((field) => {
      const name = field.name || field.id;
      const rules = field.getAttribute('data-validate').split('|');
      this.fields[name] = { element: field, rules };

      // Real-time validation on blur
      field.addEventListener('blur', () => this.validateField(name));

      // Real-time validation on input
      field.addEventListener('input', () => {
        if (this.errors[name]) {
          this.validateField(name);
        }
      });
    });

    // Form submission
    this.form.addEventListener('submit', (e) => {
      e.preventDefault();
      this.validate();
    });
  }

  validateField(name) {
    const field = this.fields[name];
    if (!field) return true;

    const { element, rules } = field;
    const value = element.value.trim();

    for (const rule of rules) {
      const [ruleName, ...params] = rule.split(':');

      if (!this.isRuleValid(ruleName, value, params, element)) {
        const message = this.getErrorMessage(ruleName, element.name, params);
        this.setFieldError(name, message);
        return false;
      }
    }

    this.clearFieldError(name);
    return true;
  }

  validate() {
    this.errors = {};
    let isValid = true;

    for (const name in this.fields) {
      if (!this.validateField(name)) {
        isValid = false;
      }
    }

    if (isValid) {
      this.onSuccess();
    } else {
      this.onError();
    }

    return isValid;
  }

  isRuleValid(rule, value, params, element) {
    switch (rule) {
      case 'required':
        return value !== '';

      case 'email':
        return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value);

      case 'min':
        return value.length >= parseInt(params[0]);

      case 'max':
        return value.length <= parseInt(params[0]);

      case 'pattern':
        const pattern = new RegExp(params[0]);
        return pattern.test(value);

      case 'match':
        const matchElement = document.querySelector(`[name="${params[0]}"]`);
        return value === (matchElement ? matchElement.value : '');

      case 'number':
        return /^\d+$/.test(value);

      case 'url':
        try {
          new URL(value);
          return true;
        } catch {
          return false;
        }

      default:
        return true;
    }
  }

  getErrorMessage(rule, fieldName, params) {
    const messages = {
      required: `${fieldName} is required`,
      email: `${fieldName} must be a valid email`,
      min: `${fieldName} must be at least ${params[0]} characters`,
      max: `${fieldName} must not exceed ${params[0]} characters`,
      pattern: `${fieldName} format is invalid`,
      match: `${fieldName} does not match`,
      number: `${fieldName} must be a number`,
      url: `${fieldName} must be a valid URL`
    };

    return messages[rule] || `${fieldName} is invalid`;
  }

  setFieldError(name, message) {
    this.errors[name] = message;
    const field = this.fields[name];

    if (field) {
      field.element.classList.remove('is-valid');
      field.element.classList.add('is-invalid');

      let feedback = field.element.nextElementSibling;
      if (!feedback || !feedback.classList.contains('invalid-feedback')) {
        feedback = document.createElement('div');
        feedback.className = 'invalid-feedback';
        field.element.parentNode.insertBefore(feedback, field.element.nextSibling);
      }

      feedback.textContent = message;
    }
  }

  clearFieldError(name) {
    delete this.errors[name];
    const field = this.fields[name];

    if (field) {
      field.element.classList.remove('is-invalid');
      field.element.classList.add('is-valid');

      const feedback = field.element.nextElementSibling;
      if (feedback && feedback.classList.contains('invalid-feedback')) {
        feedback.textContent = '';
      }
    }
  }

  onSuccess() {
    console.log('Form is valid');
    // Override this method in subclass or pass callback
  }

  onError() {
    console.log('Form has errors');
    // Override this method in subclass or pass callback
  }

  getErrors() {
    return this.errors;
  }

  isValid() {
    return Object.keys(this.errors).length === 0;
  }
}

// Auto-initialize all forms with data-validate attribute
document.addEventListener('DOMContentLoaded', () => {
  document.querySelectorAll('form[data-validate-form]').forEach((form, index) => {
    form.setAttribute('data-id', `form-validator-${index}`);
    new FormValidator(`[data-id="form-validator-${index}"]`);
  });
});
