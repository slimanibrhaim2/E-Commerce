import http from 'k6/http';
import { check, group, sleep } from 'k6';
import { Rate, Trend, Counter } from 'k6/metrics';
import { config, generateLoadStages } from './config.js';

// Custom metrics
const errorRate = new Rate('errors');
const responseTimeTrend = new Trend('response_time');
const requestCounter = new Counter('requests_total');

// Test configuration
export const options = {
  stages: generateLoadStages(),
  thresholds: {
    http_req_duration: [`p(95)<${config.thresholds.load.responseTime}`],
    http_req_failed: [`rate<${config.thresholds.load.errorRate}`],
    errors: [`rate<${config.thresholds.load.errorRate}`],
  },
};

// Configuration
const BASE_URL = config.baseUrl;

// Helper function to make API requests
function makeRequest(method, endpoint, payload = null, params = {}) {
  const url = `${BASE_URL}/${endpoint}`;
  const requestParams = {
    headers: {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
    },
    ...params
  };

  let response;
  switch (method.toLowerCase()) {
    case 'get':
      response = http.get(url, requestParams);
      break;
    case 'post':
      response = http.post(url, JSON.stringify(payload), requestParams);
      break;
    case 'put':
      response = http.put(url, JSON.stringify(payload), requestParams);
      break;
    case 'delete':
      response = http.del(url, null, requestParams);
      break;
    default:
      throw new Error(`Unsupported HTTP method: ${method}`);
  }

  requestCounter.add(1);
  responseTimeTrend.add(response.timings.duration);
  errorRate.add(response.status >= 400);

  return response;
}

// Test scenarios
export default function () {
  group('Catalog Management Tests', () => {
    catalogManagementTests();
  });

  group('Search and Filter Tests', () => {
    searchAndFilterTests();
  });

  sleep(1); // Wait 1 second between iterations
}

function catalogManagementTests() {
  // Get all categories
  group('Get All Categories', () => {
    const response = makeRequest('GET', 'api/Category?pageNumber=1&pageSize=10');
    
    check(response, {
      'Get categories status is 200': (r) => r.status === 200,
      'Get categories response has data': (r) => {
        try {
          if (!r.body || r.body.trim() === '') return false;
          const body = JSON.parse(r.body);
          return body.success && body.data;
        } catch (e) {
          return false;
        }
      }
    });
  });

  // Get all products with pagination
  group('Get All Products', () => {
    const response = makeRequest('GET', 'api/products?pageNumber=1&pageSize=10');
    
    check(response, {
      'Get products status is 200': (r) => r.status === 200,
      'Get products response has data': (r) => {
        try {
          if (!r.body || r.body.trim() === '') return false;
          const body = JSON.parse(r.body);
          return body.success && body.data;
        } catch (e) {
          return false;
        }
      }
    });
  });
}

function searchAndFilterTests() {
  // Search products by name
  group('Search Products by Name', () => {
    const response = makeRequest('GET', 'api/products/search?name=Test&pageNumber=1&pageSize=10');
    
    check(response, {
      'Search products status is 200': (r) => r.status === 200,
      'Search products response has data': (r) => {
        try {
          if (!r.body || r.body.trim() === '') return false;
          const body = JSON.parse(r.body);
          return body.success;
        } catch (e) {
          return false;
        }
      }
    });
  });

  // Get products by price range
  group('Get Products by Price Range', () => {
    const response = makeRequest('GET', 'api/products/price-range?minPrice=0&maxPrice=200&pageNumber=1&pageSize=10');
    
    check(response, {
      'Get products by price range status is 200': (r) => r.status === 200,
      'Get products by price range response has data': (r) => {
        try {
          if (!r.body || r.body.trim() === '') return false;
          const body = JSON.parse(r.body);
          return body.success;
        } catch (e) {
          return false;
        }
      }
    });
  });
}

// Stress test scenario
export function stressTest() {
  const scenarios = [
    () => makeRequest('GET', 'api/products?pageNumber=1&pageSize=20'),
    () => makeRequest('GET', 'api/Category?pageNumber=1&pageSize=20'),
    () => makeRequest('GET', 'api/products/search?name=test&pageNumber=1&pageSize=10'),
  ];

  // Randomly execute different scenarios
  const randomScenario = scenarios[Math.floor(Math.random() * scenarios.length)];
  const response = randomScenario();
  
  check(response, {
    'Stress test response status is 2xx': (r) => r.status >= 200 && r.status < 300,
    'Stress test response time < 1000ms': (r) => r.timings.duration < 1000,
  });
}