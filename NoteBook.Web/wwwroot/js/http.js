/**
 * HTTP Client
 * Advanced HTTP request handler with interceptors, caching, and middleware support
 */

class HTTPClient {
  constructor(baseURL = '/api') {
    this.baseURL = baseURL;
    this.timeout = 30000;
    this.retries = 3;
    this.retryDelay = 1000;
    this.token = null;
    this.interceptors = {
      request: [],
      response: [],
      error: []
    };
    this.cache = new Map();
    this.enableCache = false;
  }

  /**
   * Add request interceptor
   */
  addRequestInterceptor(callback) {
    this.interceptors.request.push(callback);
  }

  /**
   * Add response interceptor
   */
  addResponseInterceptor(callback) {
    this.interceptors.response.push(callback);
  }

  /**
   * Add error interceptor
   */
  addErrorInterceptor(callback) {
    this.interceptors.error.push(callback);
  }

  /**
   * Set authorization token
   */
  setToken(token) {
    this.token = token;
  }

  /**
   * Build full URL
   */
  buildURL(url, params = {}) {
    if (url.startsWith('http')) return url;

    let fullUrl = `${this.baseURL}${url}`;
    const queryParams = new URLSearchParams(params);
    const queryString = queryParams.toString();

    return queryString ? `${fullUrl}?${queryString}` : fullUrl;
  }

  /**
   * Get headers with auth token
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
  }

  /**
   * Execute request interceptors
   */
  async executeRequestInterceptors(config) {
    for (const interceptor of this.interceptors.request) {
      config = await interceptor(config);
    }
    return config;
  }

  /**
   * Execute response interceptors
   */
  async executeResponseInterceptors(response) {
    for (const interceptor of this.interceptors.response) {
      response = await interceptor(response);
    }
    return response;
  }

  /**
   * Execute error interceptors
   */
  async executeErrorInterceptors(error) {
    for (const interceptor of this.interceptors.error) {
      error = await interceptor(error);
    }
    return error;
  }

  /**
   * Core request method
   */
  async request(url, options = {}, attempt = 0) {
    try {
      // Build config
      let config = {
        method: options.method || 'GET',
        url: this.buildURL(url, options.params),
        headers: this.getHeaders(options.headers),
        body: options.body ? JSON.stringify(options.body) : undefined,
        timeout: options.timeout || this.timeout,
        ...options
      };

      // Execute request interceptors
      config = await this.executeRequestInterceptors(config);

      // Check cache
      const cacheKey = `${config.method}:${config.url}`;
      if (this.enableCache && config.method === 'GET' && this.cache.has(cacheKey)) {
        return this.cache.get(cacheKey);
      }

      // Execute request
      const controller = new AbortController();
      const timeoutId = setTimeout(() => controller.abort(), config.timeout);

      const response = await fetch(config.url, {
        method: config.method,
        headers: config.headers,
        body: config.body,
        signal: controller.signal
      });

      clearTimeout(timeoutId);

      // Handle response
      const data = await this.parseResponse(response);

      if (!response.ok) {
        throw {
          status: response.status,
          message: data.message || `HTTP Error: ${response.status}`,
          data: data
        };
      }

      // Cache successful GET requests
      if (this.enableCache && config.method === 'GET') {
        this.cache.set(cacheKey, data);
      }

      // Execute response interceptors
      return await this.executeResponseInterceptors(data);
    } catch (error) {
      // Execute error interceptors
      error = await this.executeErrorInterceptors(error);

      // Retry logic
      if (attempt < this.retries && this.isRetryable(error)) {
        await this.delay(this.retryDelay * (attempt + 1));
        return this.request(url, options, attempt + 1);
      }

      throw error;
    }
  }

  /**
   * Parse response
   */
  async parseResponse(response) {
    const contentType = response.headers.get('content-type');
    const isJson = contentType && contentType.includes('application/json');

    if (isJson) {
      return await response.json().catch(() => ({}));
    } else {
      return await response.text();
    }
  }

  /**
   * Check if error is retryable
   */
  isRetryable(error) {
    if (error.status) return error.status >= 500 || error.status === 408;
    return error.name === 'AbortError' || error instanceof TypeError;
  }

  /**
   * Delay execution
   */
  delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  /**
   * GET request
   */
  async get(url, options = {}) {
    return this.request(url, { method: 'GET', ...options });
  }

  /**
   * POST request
   */
  async post(url, data, options = {}) {
    return this.request(url, { method: 'POST', body: data, ...options });
  }

  /**
   * PUT request
   */
  async put(url, data, options = {}) {
    return this.request(url, { method: 'PUT', body: data, ...options });
  }

  /**
   * PATCH request
   */
  async patch(url, data, options = {}) {
    return this.request(url, { method: 'PATCH', body: data, ...options });
  }

  /**
   * DELETE request
   */
  async delete(url, options = {}) {
    return this.request(url, { method: 'DELETE', ...options });
  }

  /**
   * Clear cache
   */
  clearCache() {
    this.cache.clear();
  }

  /**
   * Get cache size
   */
  getCacheSize() {
    return this.cache.size;
  }
}

// Create default instance
const http = new HTTPClient('/api');

// Export
if (typeof module !== 'undefined' && module.exports) {
  module.exports = { HTTPClient, http };
}
