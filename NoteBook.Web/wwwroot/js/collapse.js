/**
 * Collapse Component
 * Vanilla JavaScript collapse/accordion without jQuery or Bootstrap
 */

class Collapse {
  constructor(selector) {
    this.element = document.querySelector(selector);
    if (!this.element) throw new Error(`Collapse element not found: ${selector}`);
    
    this.header = this.element.querySelector('.collapse-header');
    this.body = this.element.querySelector('.collapse-body');
    
    this.init();
  }

  init() {
    if (this.header) {
      this.header.addEventListener('click', () => this.toggle());
    }
  }

  toggle() {
    if (this.element.classList.contains('show')) {
      this.hide();
    } else {
      this.show();
    }
  }

  show() {
    this.element.classList.add('show');
  }

  hide() {
    this.element.classList.remove('show');
  }
}

// Auto-initialize all collapse elements on page load
document.addEventListener('DOMContentLoaded', () => {
  document.querySelectorAll('.collapse').forEach((collapse, index) => {
    collapse.setAttribute('data-id', `collapse-${index}`);
    new Collapse(`[data-id="collapse-${index}"]`);
  });
});

// Accordion functionality
class Accordion {
  constructor(selector) {
    this.container = document.querySelector(selector);
    if (!this.container) throw new Error(`Accordion not found: ${selector}`);
    
    this.items = this.container.querySelectorAll('.collapse');
    this.init();
  }

  init() {
    this.items.forEach((item) => {
      const header = item.querySelector('.collapse-header');
      if (header) {
        header.addEventListener('click', () => this.handleClick(item));
      }
    });
  }

  handleClick(clickedItem) {
    this.items.forEach((item) => {
      if (item !== clickedItem) {
        item.classList.remove('show');
      }
    });
    clickedItem.classList.toggle('show');
  }
}

// Auto-initialize accordion on page load
document.addEventListener('DOMContentLoaded', () => {
  document.querySelectorAll('[data-accordion]').forEach((accordion, index) => {
    accordion.setAttribute('data-id', `accordion-${index}`);
    new Accordion(`[data-id="accordion-${index}"]`);
  });
});
