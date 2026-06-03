/**
 * Site-Specific JavaScript
 * Common initialization and setup for the application
 */

document.addEventListener('DOMContentLoaded', () => {
  // Initialize HTTP client
  initializeHTTPClient();

  // Initialize mobile menu
  initializeMobileMenu();
});

/**
 * Initialize HTTP client with interceptors
 */
function initializeHTTPClient() {
  // Get auth token from storage or meta tag
  const token = localStorage.getItem('authToken') || 
                document.querySelector('meta[name="auth-token"]')?.content;

  if (token) {
    API.setToken(token);
    http.setToken(token);
  }

  // Add request logging in development
  if (isDevelopment()) {
    http.addRequestInterceptor(async (config) => {
      console.log('📤 Request:', config.method, config.url);
      return config;
    });

    http.addResponseInterceptor(async (response) => {
      console.log('📥 Response:', response);
      return response;
    });

    http.addErrorInterceptor(async (error) => {
      console.error('❌ Error:', error.status, error.message);
      return error;
    });
  }

  // Handle 401 errors (unauthorized)
  http.addErrorInterceptor(async (error) => {
    if (error.status === 401) {
      // Clear token and redirect to login
      localStorage.removeItem('authToken');
      window.location.href = '/auth/login';
    }
    return error;
  });

  // Handle 403 errors (forbidden)
  http.addErrorInterceptor(async (error) => {
    if (error.status === 403) {
      Alert.show('You do not have permission to perform this action', 'warning');
    }
    return error;
  });
}

/**
 * Initialize mobile menu toggle
 */
function initializeMobileMenu() {
  const mobileMenuBtn = document.getElementById('mobile-menu-btn');
  const mobileMenu = document.getElementById('mobile-menu');

  if (mobileMenuBtn && mobileMenu) {
    mobileMenuBtn.addEventListener('click', () => {
      const isOpen = mobileMenu.style.maxHeight && mobileMenu.style.maxHeight !== '0px';
      mobileMenu.style.maxHeight = isOpen ? '0px' : `${mobileMenu.scrollHeight}px`;
    });

    // Close menu when a link is clicked
    mobileMenu.querySelectorAll('a').forEach((link) => {
      link.addEventListener('click', () => {
        mobileMenu.style.maxHeight = '0px';
      });
    });
  }
}

/**
 * Check if in development mode
 */
function isDevelopment() {
  return document.body.getAttribute('data-environment') === 'Development';
}

/**
 * Global error handler
 */
window.addEventListener('error', (event) => {
  console.error('Global error:', event.error);
});

/**
 * Global unhandled promise rejection handler
 */
window.addEventListener('unhandledrejection', (event) => {
  console.error('Unhandled promise rejection:', event.reason);
});
