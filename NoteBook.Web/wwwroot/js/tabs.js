/**
 * Tabs Component
 * Vanilla JavaScript tab switching without jQuery or Bootstrap
 */

class Tabs {
  constructor(selector) {
    this.container = document.querySelector(selector);
    if (!this.container) throw new Error(`Tab container not found: ${selector}`);
    
    this.tabs = this.container.querySelectorAll('.tab-link');
    this.contents = this.container.querySelectorAll('.tab-content');
    
    this.init();
  }

  init() {
    this.tabs.forEach((tab, index) => {
      tab.addEventListener('click', (e) => {
        e.preventDefault();
        this.activate(index);
      });
    });
  }

  activate(index) {
    // Remove active class from all tabs and contents
    this.tabs.forEach((tab) => tab.classList.remove('active'));
    this.contents.forEach((content) => content.classList.remove('active'));

    // Add active class to selected tab and content
    if (this.tabs[index]) {
      this.tabs[index].classList.add('active');
    }
    if (this.contents[index]) {
      this.contents[index].classList.add('active');
    }
  }
}

// Auto-initialize all tab groups on page load
document.addEventListener('DOMContentLoaded', () => {
  document.querySelectorAll('.tab-group').forEach((group) => {
    new Tabs(`.tab-group[data-id="${group.getAttribute('data-id')}"]`);
  });
});
