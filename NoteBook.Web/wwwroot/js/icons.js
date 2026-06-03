/**
 * Icon Management System
 * Provides utilities for working with Feather Icons
 * 
 * Feather Icons is a lightweight SVG icon library (npm: feather-icons)
 * 24x24px, stroke-based, minimalist design, open source
 * 
 * Documentation: https://feathericons.com/
 */

const Icons = {
  /**
   * Icon names available in Feather Icons
   * Common icons used in NoteBook application
   */
  NAMES: {
    // Navigation
    MENU: 'menu',
    X: 'x',
    CHEVRON_RIGHT: 'chevron-right',
    CHEVRON_LEFT: 'chevron-left',
    CHEVRON_DOWN: 'chevron-down',
    CHEVRON_UP: 'chevron-up',
    ARROW_RIGHT: 'arrow-right',
    ARROW_LEFT: 'arrow-left',
    HOME: 'home',
    
    // Notes
    FILE_TEXT: 'file-text',
    EDIT: 'edit',
    EDIT_2: 'edit-2',
    TRASH_2: 'trash-2',
    COPY: 'copy',
    SHARE_2: 'share-2',
    ARCHIVE: 'archive',
    BOOKMARK: 'bookmark',
    
    // Actions
    PLUS: 'plus',
    MINUS: 'minus',
    CHECK: 'check',
    X_CIRCLE: 'x-circle',
    CHECK_CIRCLE: 'check-circle',
    ALERT_CIRCLE: 'alert-circle',
    INFO: 'info',
    HELP_CIRCLE: 'help-circle',
    
    // Search & Filter
    SEARCH: 'search',
    FILTER: 'filter',
    SLIDERS: 'sliders',
    
    // Tags & Categories
    TAG: 'tag',
    FOLDER: 'folder',
    LAYERS: 'layers',
    
    // User & Account
    USER: 'user',
    USERS: 'users',
    LOG_OUT: 'log-out',
    LOG_IN: 'log-in',
    SETTINGS: 'settings',
    LOCK: 'lock',
    UNLOCK: 'unlock',
    
    // Time & Date
    CALENDAR: 'calendar',
    CLOCK: 'clock',
    WATCH: 'watch',
    
    // UI Elements
    BELL: 'bell',
    HEART: 'heart',
    STAR: 'star',
    DOWNLOAD: 'download',
    UPLOAD: 'upload',
    LINK: 'link',
    
    // Status
    CIRCLE: 'circle',
    ACTIVITY: 'activity',
    WIFI: 'wifi',
    WIFI_OFF: 'wifi-off',
    LOADER: 'loader'
  },

  /**
   * Initialize feather icons
   * Call this after DOM is loaded or after adding new SVGs
   */
  init() {
    if (typeof feather !== 'undefined') {
      feather.replace();
    } else {
      console.warn('Feather Icons library not loaded');
    }
  },

  /**
   * Create icon SVG element
   */
  create(iconName, options = {}) {
    const {
      size = 24,
      strokeWidth = 2,
      color = 'currentColor',
      className = '',
      title = ''
    } = options;

    const svg = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
    svg.setAttribute('width', size);
    svg.setAttribute('height', size);
    svg.setAttribute('viewBox', '0 0 24 24');
    svg.setAttribute('fill', 'none');
    svg.setAttribute('stroke', color);
    svg.setAttribute('stroke-width', strokeWidth);
    svg.setAttribute('stroke-linecap', 'round');
    svg.setAttribute('stroke-linejoin', 'round');
    svg.setAttribute('class', `icon icon-${iconName} ${className}`.trim());
    
    if (title) {
      const titleElement = document.createElementNS('http://www.w3.org/2000/svg', 'title');
      titleElement.textContent = title;
      svg.appendChild(titleElement);
    }

    // Use data-feather attribute for feather icons to replace
    svg.setAttribute('data-feather', iconName);

    return svg;
  },

  /**
   * Insert icon into element
   */
  insert(element, iconName, options = {}) {
    if (typeof element === 'string') {
      element = document.querySelector(element);
    }

    if (!element) return null;

    const icon = this.create(iconName, options);
    element.appendChild(icon);
    
    // Initialize if feather is available
    this.init();
    
    return icon;
  },

  /**
   * Replace element with icon
   */
  replace(element, iconName, options = {}) {
    if (typeof element === 'string') {
      element = document.querySelector(element);
    }

    if (!element) return null;

    const icon = this.create(iconName, options);
    element.replaceWith(icon);
    
    // Initialize if feather is available
    this.init();
    
    return icon;
  },

  /**
   * Create icon button
   */
  createButton(iconName, options = {}) {
    const {
      title = '',
      className = 'btn btn-secondary',
      size = 20,
      ariaLabel = title
    } = options;

    const button = document.createElement('button');
    button.className = className;
    button.setAttribute('title', title);
    button.setAttribute('aria-label', ariaLabel);

    const icon = this.create(iconName, { size });
    button.appendChild(icon);

    return button;
  },

  /**
   * Create icon with text
   */
  createWithText(iconName, text, options = {}) {
    const {
      size = 20,
      className = '',
      iconRight = false
    } = options;

    const container = document.createElement('span');
    container.className = `icon-with-text ${className}`.trim();

    const icon = this.create(iconName, { size });

    if (iconRight) {
      container.appendChild(document.createTextNode(text));
      container.appendChild(document.createElement('span')); // spacer
      container.appendChild(icon);
    } else {
      container.appendChild(icon);
      container.appendChild(document.createElement('span')); // spacer
      container.appendChild(document.createTextNode(text));
    }

    return container;
  },

  /**
   * Get all available icon names
   */
  getAvailableIcons() {
    return Object.values(this.NAMES);
  },

  /**
   * Common icon sets for specific use cases
   */
  SETS: {
    /**
     * CRUD operations
     */
    CRUD: {
      CREATE: 'plus',
      READ: 'file-text',
      UPDATE: 'edit',
      DELETE: 'trash-2'
    },

    /**
     * Status indicators
     */
    STATUS: {
      SUCCESS: 'check-circle',
      ERROR: 'x-circle',
      WARNING: 'alert-circle',
      INFO: 'info',
      PENDING: 'loader'
    },

    /**
     * Social actions
     */
    SOCIAL: {
      SHARE: 'share-2',
      LIKE: 'heart',
      BOOKMARK: 'bookmark',
      FOLLOW: 'users'
    },

    /**
     * File operations
     */
    FILES: {
      DOWNLOAD: 'download',
      UPLOAD: 'upload',
      COPY: 'copy',
      DELETE: 'trash-2'
    }
  }
};

// Auto-initialize on page load
document.addEventListener('DOMContentLoaded', () => {
  Icons.init();
});

// Export
if (typeof module !== 'undefined' && module.exports) {
  module.exports = { Icons };
}
