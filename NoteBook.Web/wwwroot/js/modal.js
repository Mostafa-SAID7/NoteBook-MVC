/**
 * Modal Component
 * Vanilla JavaScript modal handling without jQuery or Bootstrap
 */

class Modal {
  constructor(selector) {
    this.modal = document.querySelector(selector);
    if (!this.modal) throw new Error(`Modal not found: ${selector}`);
    this.backdrop = this.modal.querySelector('.modal');
    this.dialog = this.modal.querySelector('.modal-dialog');
    this.closeBtn = this.modal.querySelector('.modal-close');
    
    this.init();
  }

  init() {
    // Close button
    if (this.closeBtn) {
      this.closeBtn.addEventListener('click', () => this.hide());
    }

    // Backdrop click
    if (this.backdrop) {
      this.backdrop.addEventListener('click', (e) => {
        if (e.target === this.backdrop) {
          this.hide();
        }
      });
    }

    // Escape key
    document.addEventListener('keydown', (e) => {
      if (e.key === 'Escape' && this.backdrop?.classList.contains('show')) {
        this.hide();
      }
    });
  }

  show() {
    if (this.backdrop) {
      this.backdrop.classList.add('show');
      document.body.style.overflow = 'hidden';
    }
  }

  hide() {
    if (this.backdrop) {
      this.backdrop.classList.remove('show');
      document.body.style.overflow = '';
    }
  }

  toggle() {
    if (this.backdrop?.classList.contains('show')) {
      this.hide();
    } else {
      this.show();
    }
  }
}

// Global modal opener helper
document.addEventListener('DOMContentLoaded', () => {
  document.querySelectorAll('[data-modal-target]').forEach((trigger) => {
    const modalId = trigger.getAttribute('data-modal-target');
    trigger.addEventListener('click', () => {
      const modal = new Modal(modalId);
      modal.show();
    });
  });
});
