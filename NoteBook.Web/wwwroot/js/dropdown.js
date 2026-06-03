/**
 * Dropdown Component
 * Vanilla JavaScript dropdown menu handling without jQuery or Bootstrap
 */

class Dropdown {
  constructor(selector) {
    this.dropdown = document.querySelector(selector);
    if (!this.dropdown) throw new Error(`Dropdown not found: ${selector}`);
    
    this.toggle = this.dropdown.querySelector('.dropdown-toggle');
    this.menu = this.dropdown.querySelector('.dropdown-menu');
    
    this.init();
  }

  init() {
    // Toggle button
    if (this.toggle) {
      this.toggle.addEventListener('click', (e) => {
        e.stopPropagation();
        this.toggle_menu();
      });
    }

    // Menu items
    if (this.menu) {
      this.menu.querySelectorAll('.dropdown-item').forEach((item) => {
        item.addEventListener('click', () => this.close());
      });
    }

    // Close on outside click
    document.addEventListener('click', () => {
      if (this.dropdown.classList.contains('show')) {
        this.close();
      }
    });

    // Close on escape
    document.addEventListener('keydown', (e) => {
      if (e.key === 'Escape' && this.dropdown.classList.contains('show')) {
        this.close();
      }
    });
  }

  toggle_menu() {
    if (this.dropdown.classList.contains('show')) {
      this.close();
    } else {
      this.open();
    }
  }

  open() {
    this.dropdown.classList.add('show');
  }

  close() {
    this.dropdown.classList.remove('show');
  }
}

// Global dropdown opener helper
document.addEventListener('DOMContentLoaded', () => {
  document.querySelectorAll('.dropdown').forEach((dropdown) => {
    new Dropdown(`.dropdown[data-id="${dropdown.getAttribute('data-id')}"]`);
  });
});
