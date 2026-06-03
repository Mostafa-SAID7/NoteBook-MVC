/**
 * Utility Functions
 * Common helper functions for form validation, DOM manipulation, AJAX, etc.
 */

const UI = {
  /**
   * Show loading spinner
   */
  showSpinner(target) {
    const spinner = document.createElement('div');
    spinner.className = 'spinner spinner-border';
    spinner.setAttribute('data-spinner', 'true');

    if (typeof target === 'string') {
      const element = document.querySelector(target);
      if (element) element.appendChild(spinner);
    } else if (target instanceof Element) {
      target.appendChild(spinner);
    }

    return spinner;
  },

  /**
   * Hide loading spinner
   */
  hideSpinner(target) {
    if (typeof target === 'string') {
      const element = document.querySelector(target);
      if (element) {
        const spinner = element.querySelector('[data-spinner]');
        if (spinner) spinner.remove();
      }
    } else if (target instanceof Element) {
      const spinner = target.querySelector('[data-spinner]');
      if (spinner) spinner.remove();
    }
  },

  /**
   * Disable button and show loading state
   */
  disableButton(button) {
    button.disabled = true;
    button.setAttribute('data-disabled', 'true');
  },

  /**
   * Enable button and restore state
   */
  enableButton(button) {
    button.disabled = false;
    button.removeAttribute('data-disabled');
  },

  /**
   * Show error message
   */
  showError(element, message) {
    if (typeof element === 'string') {
      element = document.querySelector(element);
    }

    if (element) {
      element.classList.add('is-invalid');
      const feedback = element.nextElementSibling;
      if (feedback && feedback.classList.contains('invalid-feedback')) {
        feedback.textContent = message;
      }
    }
  },

  /**
   * Clear error message
   */
  clearError(element) {
    if (typeof element === 'string') {
      element = document.querySelector(element);
    }

    if (element) {
      element.classList.remove('is-invalid');
      element.classList.remove('is-valid');
    }
  },

  /**
   * Show success state
   */
  showSuccess(element) {
    if (typeof element === 'string') {
      element = document.querySelector(element);
    }

    if (element) {
      element.classList.remove('is-invalid');
      element.classList.add('is-valid');
    }
  },

  /**
   * Serialize form to object
   */
  serializeForm(form) {
    if (typeof form === 'string') {
      form = document.querySelector(form);
    }

    const formData = new FormData(form);
    const object = {};

    for (const [key, value] of formData.entries()) {
      if (object.hasOwnProperty(key)) {
        if (!Array.isArray(object[key])) {
          object[key] = [object[key]];
        }
        object[key].push(value);
      } else {
        object[key] = value;
      }
    }

    return object;
  },

  /**
   * Validate email
   */
  isValidEmail(email) {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email);
  },

  /**
   * Validate required field
   */
  isNotEmpty(value) {
    return value && value.trim() !== '';
  },

  /**
   * Get query parameter
   */
  getQueryParam(param) {
    const params = new URLSearchParams(window.location.search);
    return params.get(param);
  },

  /**
   * Add query parameter to URL
   */
  addQueryParam(url, key, value) {
    const separator = url.includes('?') ? '&' : '?';
    return `${url}${separator}${key}=${encodeURIComponent(value)}`;
  },

  /**
   * Remove element from DOM
   */
  remove(selector) {
    const element = typeof selector === 'string' ? document.querySelector(selector) : selector;
    if (element) element.remove();
  },

  /**
   * Add class to element
   */
  addClass(element, className) {
    if (typeof element === 'string') {
      element = document.querySelector(element);
    }
    if (element) element.classList.add(className);
  },

  /**
   * Remove class from element
   */
  removeClass(element, className) {
    if (typeof element === 'string') {
      element = document.querySelector(element);
    }
    if (element) element.classList.remove(className);
  },

  /**
   * Toggle class on element
   */
  toggleClass(element, className) {
    if (typeof element === 'string') {
      element = document.querySelector(element);
    }
    if (element) element.classList.toggle(className);
  },

  /**
   * Check if element has class
   */
  hasClass(element, className) {
    if (typeof element === 'string') {
      element = document.querySelector(element);
    }
    return element ? element.classList.contains(className) : false;
  },

  /**
   * Set element text content
   */
  setText(element, text) {
    if (typeof element === 'string') {
      element = document.querySelector(element);
    }
    if (element) element.textContent = text;
  },

  /**
   * Get element text content
   */
  getText(element) {
    if (typeof element === 'string') {
      element = document.querySelector(element);
    }
    return element ? element.textContent : '';
  },

  /**
   * Set element HTML content
   */
  setHtml(element, html) {
    if (typeof element === 'string') {
      element = document.querySelector(element);
    }
    if (element) element.innerHTML = html;
  },

  /**
   * Get element HTML content
   */
  getHtml(element) {
    if (typeof element === 'string') {
      element = document.querySelector(element);
    }
    return element ? element.innerHTML : '';
  },

  /**
   * Set element attribute
   */
  setAttribute(element, name, value) {
    if (typeof element === 'string') {
      element = document.querySelector(element);
    }
    if (element) element.setAttribute(name, value);
  },

  /**
   * Get element attribute
   */
  getAttribute(element, name) {
    if (typeof element === 'string') {
      element = document.querySelector(element);
    }
    return element ? element.getAttribute(name) : null;
  },

  /**
   * Show element
   */
  show(element) {
    if (typeof element === 'string') {
      element = document.querySelector(element);
    }
    if (element) element.style.display = '';
  },

  /**
   * Hide element
   */
  hide(element) {
    if (typeof element === 'string') {
      element = document.querySelector(element);
    }
    if (element) element.style.display = 'none';
  },

  /**
   * Toggle element visibility
   */
  toggle(element) {
    if (typeof element === 'string') {
      element = document.querySelector(element);
    }
    if (element) element.style.display = element.style.display === 'none' ? '' : 'none';
  }
};

/**
 * API Helper with AJAX support
 * Advanced fetch wrapper with error handling, retry logic, and interceptors
 */
const API = {
  baseURL: '/api',
  defaultTimeout: 30000,
  retryCount: 3,
  retryDelay: 1000,
  token: null,

  /**
   * Set authorization token
   */
  setToken(token) {
    this.token = token;
  },

  /**
   * Get authorization headers
   */
  getHeaders(customHeaders = {}) {
    const headers = {
      'Content-Type': 'application/json',
      ...customHeaders
    };

    if (this.token) {
      headers['Authorization'] = `Bearer ${this.token}`;
    }

    return headers;
  },

  /**
   * GET request
   */
  async get(url, options = {}) {
    return this.request(url, {
      method: 'GET',
      ...options
    });
  },

  /**
   * POST request
   */
  async post(url, data, options = {}) {
    return this.request(url, {
      method: 'POST',
      body: JSON.stringify(data),
      ...options
    });
  },

  /**
   * PUT request
   */
  async put(url, data, options = {}) {
    return this.request(url, {
      method: 'PUT',
      body: JSON.stringify(data),
      ...options
    });
  },

  /**
   * PATCH request
   */
  async patch(url, data, options = {}) {
    return this.request(url, {
      method: 'PATCH',
      body: JSON.stringify(data),
      ...options
    });
  },

  /**
   * DELETE request
   */
  async delete(url, options = {}) {
    return this.request(url, {
      method: 'DELETE',
      ...options
    });
  },

  /**
   * Core request method with retry logic
   */
  async request(url, options = {}, attempt = 0) {
    try {
      const fullUrl = url.startsWith('http') ? url : `${this.baseURL}${url}`;
      const timeout = options.timeout || this.defaultTimeout;

      const controller = new AbortController();
      const timeoutId = setTimeout(() => controller.abort(), timeout);

      const response = await fetch(fullUrl, {
        headers: this.getHeaders(),
        ...options,
        signal: controller.signal
      });

      clearTimeout(timeoutId);
      return await this.handleResponse(response);
    } catch (error) {
      clearTimeout(timeoutId);

      // Retry logic for network errors
      if (attempt < this.retryCount && this.isRetryableError(error)) {
        await this.delay(this.retryDelay * (attempt + 1));
        return this.request(url, options, attempt + 1);
      }

      throw this.handleError(error);
    }
  },

  /**
   * Check if error is retryable
   */
  isRetryableError(error) {
    return error.name === 'AbortError' || error instanceof TypeError;
  },

  /**
   * Delay execution
   */
  delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
  },

  /**
   * Handle API response
   */
  async handleResponse(response) {
    const contentType = response.headers.get('content-type');
    const isJson = contentType && contentType.includes('application/json');

    let data = {};
    if (isJson) {
      data = await response.json().catch(() => ({}));
    } else {
      data = await response.text();
    }

    if (!response.ok) {
      const error = new Error(data.message || `HTTP Error: ${response.status}`);
      error.status = response.status;
      error.data = data;
      throw error;
    }

    return data;
  },

  /**
   * Handle error
   */
  handleError(error) {
    console.error('API Error:', error);
    return error;
  }
};

/**
 * AJAX Helper for data-driven interactions
 * Bind data-ajax attributes to elements for automatic AJAX handling
 */
const AJAX = {
  /**
   * Auto-bind AJAX attributes to elements
   */
  bindAll() {
    document.querySelectorAll('[data-ajax]').forEach((element) => {
      const method = element.getAttribute('data-ajax-method') || 'GET';
      const url = element.getAttribute('data-ajax');
      const confirm = element.getAttribute('data-ajax-confirm');

      element.addEventListener('click', async (e) => {
        e.preventDefault();

        if (confirm && !window.confirm(confirm)) {
          return;
        }

        await this.execute(element);
      });
    });
  },

  /**
   * Execute AJAX request
   */
  async execute(element) {
    const url = element.getAttribute('data-ajax');
    const method = element.getAttribute('data-ajax-method') || 'GET';
    const confirm = element.getAttribute('data-ajax-confirm');
    const loading = element.getAttribute('data-ajax-loading');
    const success = element.getAttribute('data-ajax-success');
    const error = element.getAttribute('data-ajax-error');
    const target = element.getAttribute('data-ajax-target');
    const mode = element.getAttribute('data-ajax-mode') || 'replace'; // replace, append, prepend

    try {
      // Show loading state
      if (loading) {
        UI.addClass(element, loading);
      }

      let response;
      const body = element.getAttribute('data-ajax-body');

      if (method === 'GET') {
        response = await API.get(url);
      } else if (method === 'POST') {
        response = await API.post(url, body ? JSON.parse(body) : {});
      } else if (method === 'PUT') {
        response = await API.put(url, body ? JSON.parse(body) : {});
      } else if (method === 'DELETE') {
        response = await API.delete(url);
      }

      // Update target element if specified
      if (target) {
        const targetElement = document.querySelector(target);
        if (targetElement) {
          if (mode === 'replace') {
            targetElement.innerHTML = response.html || response;
          } else if (mode === 'append') {
            targetElement.innerHTML += response.html || response;
          } else if (mode === 'prepend') {
            targetElement.innerHTML = (response.html || response) + targetElement.innerHTML;
          }
        }
      }

      // Call success callback
      if (success) {
        this.executeCallback(success, response);
      }

      // Show success alert if specified
      Alert.show(element.getAttribute('data-ajax-success-message') || 'Success!', 'success');
    } catch (err) {
      // Call error callback
      if (error) {
        this.executeCallback(error, err);
      }

      // Show error alert
      Alert.show(err.message || 'An error occurred', 'danger');
    } finally {
      // Hide loading state
      if (loading) {
        UI.removeClass(element, loading);
      }
    }
  },

  /**
   * Execute callback function
   */
  executeCallback(callbackName, data) {
    if (typeof window[callbackName] === 'function') {
      window[callbackName](data);
    }
  }
};

/**
 * Table AJAX Helper
 * Handle pagination, sorting, and filtering with AJAX
 */
const TableAJAX = {
  /**
   * Bind table controls
   */
  bind(tableSelector) {
    const table = document.querySelector(tableSelector);
    if (!table) return;

    // Bind pagination links
    table.querySelectorAll('[data-page]').forEach((link) => {
      link.addEventListener('click', (e) => {
        e.preventDefault();
        const url = link.getAttribute('href');
        this.loadTable(tableSelector, url);
      });
    });

    // Bind sort links
    table.querySelectorAll('[data-sort]').forEach((link) => {
      link.addEventListener('click', (e) => {
        e.preventDefault();
        const url = link.getAttribute('href');
        this.loadTable(tableSelector, url);
      });
    });

    // Bind filter form
    const filterForm = table.querySelector('[data-table-filter]');
    if (filterForm) {
      filterForm.addEventListener('submit', (e) => {
        e.preventDefault();
        const formData = UI.serializeForm(filterForm);
        const url = this.buildFilterUrl(filterForm.action, formData);
        this.loadTable(tableSelector, url);
      });
    }
  },

  /**
   * Load table data via AJAX
   */
  async loadTable(tableSelector, url) {
    const table = document.querySelector(tableSelector);
    if (!table) return;

    try {
      UI.showSpinner(table);
      const response = await API.get(url);
      UI.setHtml(table, response);
      this.bind(tableSelector);
    } catch (error) {
      Alert.show(error.message || 'Failed to load table', 'danger');
    } finally {
      UI.hideSpinner(table);
    }
  },

  /**
   * Build filter URL
   */
  buildFilterUrl(baseUrl, filters) {
    let url = baseUrl;
    for (const [key, value] of Object.entries(filters)) {
      if (value) {
        url = UI.addQueryParam(url, key, value);
      }
    }
    return url;
  }
};

// Auto-initialize AJAX bindings on page load
document.addEventListener('DOMContentLoaded', () => {
  AJAX.bindAll();
});

// Export for use
if (typeof module !== 'undefined' && module.exports) {
  module.exports = { UI, API, AJAX, TableAJAX };
}
