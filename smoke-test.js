import http from 'k6/http';
import { check, group } from 'k6';
import { config, formatDuration, scaleVUs, scaleDuration } from './config.js';

// Smoke test configuration - minimal load to verify API is working
export const options = {
  vus: config.vus.smoke,
  duration: formatDuration(config.duration.smoke),
  thresholds: {
    http_req_duration: [`p(95)<${config.thresholds.smoke.responseTime}`],
    http_req_failed: [`rate<${config.thresholds.smoke.errorRate}`],
  },
};

const BASE_URL = config.baseUrl;

export default function () {
  // Basic health checks
  group('API Health Checks', () => {
    
    // Test basic endpoints (using AllowAnonymous endpoints)
      const endpoints = [
    'api/Category?pageNumber=1&pageSize=5',
    'api/products?pageNumber=1&pageSize=5'
  ];

    endpoints.forEach(endpoint => {
      const response = http.get(`${BASE_URL}/${endpoint}`, {
        headers: {
          'Accept': 'application/json',
          'Content-Type': 'application/json'
        }
      });

      check(response, {
        [`${endpoint} - status is 200`]: (r) => r.status === 200,
        [`${endpoint} - response time < 500ms`]: (r) => r.timings.duration < 500,
        [`${endpoint} - has valid response`]: (r) => {
          try {
            if (!r.body || r.body.trim() === '') return false;
            const body = JSON.parse(r.body);
            return body.hasOwnProperty('success');
          } catch (e) {
            return false;
          }
        }
      });
    });
  });
}