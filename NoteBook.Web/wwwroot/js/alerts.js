/**
 * Alert Component
 * Vanilla JavaScript alert dismissal without jQuery or Bootstrap
 */

class Alert {
  constructor(selector) {
    this.alert = document.querySelector(selector);
    if (!this.alert) throw new Error(`Alert not found: ${selector}`);
    
    this.closeBtn = this.alert.querySelector('.alert-close');
    this.init();
  }

  init() {
    if (this.closeBtn) {
      this.closeBtn.addEventListener('click', () => this.dismiss());
    }
  }

  dismiss() {
    this.alert.remove();
  }

  static dismissAll() {
    document.querySelectorAll('.alert').forEach((alert) => alert.remove());
  }

  static show(message, type = 'info', duration = 5000) {
    const alert = document.createElement('div');
    alert.className = `alert alert-${type}`;
    alert.innerHTML = `
      <span>${message}</span>
      <button class="alert-close" aria-label="Close">×</button>
    `;

    const container = document.querySelector('.alerts-container') || document.body;
    container.insertBefore(alert, container.firstChild);

    const alertInstance = new Alert(alert);

    if (duration > 0) {
      setTimeout(() => alert.remove(), duration);
    }

    return alertInstance;
  }
}

// Auto-initialize all alerts on page load
document.addEventListener('DOMContentLoaded', () => {
  document.querySelectorAll('.alert').forEach((alert, index) => {
    alert.setAttribute('data-id', `alert-${index}`);
    new Alert(`[data-id="alert-${index}"]`);
  });
});
