/*
 * 🎯 SIMPLE STRESS TEST - FIND BREAKING POINT QUICKLY
 * ===================================================
 * 
 * PURPOSE: Keep increasing users until system breaks (50% failure rate)
 * 
 * 📊 TEST PATTERN:
 * • Start small and keep increasing: 5 → 10 → 20 → 30 → ... → up to 10,000 users
 * • Duration: 1 minute per step (30s ramp + 30s hold)
 * • Auto-stop: When 50% of requests fail
 * • Clean logs: No verbose warnings, meaningful summaries only
 * 
 * ⏱️ ESTIMATED DURATION: ~30 minutes (or until 50% failure rate)
 */

import http from 'k6/http';
import { check, group, sleep } from 'k6';
import { Rate, Trend, Counter, Gauge } from 'k6/metrics';
import { config, generateStressStages } from './config.js';

// Custom metrics following best practices
const errorRate = new Rate('errors');
const responseTimeTrend = new Trend('response_time');
const requestCounter = new Counter('requests_total');
const activeUsersGauge = new Gauge('active_users');

// Best practice: comprehensive thresholds with exit criteria
export const options = {
  stages: generateStressStages(),
  thresholds: {
    // Auto-stop at 50% failure rate
    http_req_failed: [
      { threshold: 'rate<0.50', abortOnFail: true }, // Stop test when rate reaches 50% or higher
    ],
    
    // Performance monitoring (no abort - just track)
    http_req_duration: [
      'avg<5000', // Average should stay reasonable (5s max)
      'p(95)<10000', // P95 should stay under 10s
    ],
  },
  
  // Test settings
  discardResponseBodies: true,
  noConnectionReuse: false,
  userAgent: 'k6-simple-capacity-test/1.0',
};

const BASE_URL = config.baseUrl;

// Track maximum serving capacity globally
let maxServingCapacity = 0;
let currentUserLevel = 0;

// Simple test setup with clean messaging
export function setup() {
  console.log('🎯 SIMPLE STRESS TEST - Find Breaking Point');
  console.log(`📊 Pattern: 5 → 10 → 20 → 30 → 40 → 50 → 75 → 100 → 150 → 200 → ... → up to 10,000 users`);
  console.log(`⏱️  Duration: 1 minute per step (auto-stop at 50% failure rate)`);
  console.log(`🎯 Base URL: ${BASE_URL}`);
  console.log('');
  
  const healthCheck = http.get(`${BASE_URL}/api/products?pageNumber=1&pageSize=1`);
  if (healthCheck.status !== 200) {
    throw new Error(`❌ API not available. Status: ${healthCheck.status}, Response: ${healthCheck.body}`);
  }
  
  const baselineTime = healthCheck.timings.duration;
  if (baselineTime > 3000) {
    console.log(`⚠️  API response is slow: ${baselineTime.toFixed(0)}ms - API might be starting up`);
    console.log(`⚠️  Consider waiting a moment for API to warm up before testing`);
  } else {
    console.log(`✅ API ready - baseline: ${baselineTime.toFixed(0)}ms`);
  }
  console.log('🚀 Starting incremental stress test...');
  
  return { startTime: Date.now() };
}

// Simple test completion reporting with maximum serving capacity
export function teardown(data) {
  const testDuration = (Date.now() - data.startTime) / 1000 / 60;
  console.log(`\n🎯 STRESS TEST COMPLETED`);
  console.log(`⏱️  Duration: ${testDuration.toFixed(1)} minutes`);
  console.log(`👥 Maximum Users Reached: ${currentUserLevel}`);
  console.log(`\n🏆 MAXIMUM SERVING CAPACITY: ${maxServingCapacity} USERS`);
  console.log(`💡 This is the highest number of users your system can serve successfully`);
  console.log(`📈 Production recommendations:`);
  console.log(`   • Target capacity: ${Math.floor(maxServingCapacity * 0.7)} users (70% of max)`);
  console.log(`   • Auto-scaling trigger: ${Math.floor(maxServingCapacity * 0.8)} users (80% of max)`);
  console.log(`   • Emergency capacity: ${maxServingCapacity} users (100% of max)`);
}

// Best practice: Weighted scenario selection for realistic load
const scenarios = [
  // 40% - Product browsing (most common user action)
  { weight: 4, request: () => http.get(`${BASE_URL}/api/products?pageNumber=${Math.floor(Math.random() * 5) + 1}&pageSize=20`) },
  
  // 25% - Category browsing  
  { weight: 2.5, request: () => http.get(`${BASE_URL}/api/Category?pageNumber=${Math.floor(Math.random() * 3) + 1}&pageSize=20`) },
  
  // 25% - Product search
  { weight: 2.5, request: () => http.get(`${BASE_URL}/api/products/search?name=test&pageNumber=1&pageSize=10`) },
  
  // 10% - Price filtering (less common)
  { weight: 1, request: () => http.get(`${BASE_URL}/api/products/price-range?minPrice=0&maxPrice=500&pageNumber=1&pageSize=10`) },
];

// Helper function for weighted random selection
function selectWeightedScenario() {
  const totalWeight = scenarios.reduce((sum, scenario) => sum + scenario.weight, 0);
  let random = Math.random() * totalWeight;
  
  for (const scenario of scenarios) {
    random -= scenario.weight;
    if (random <= 0) {
      return scenario.request;
    }
  }
  
  return scenarios[0].request; // Fallback
}

export default function (data) {
  // Track current virtual user for monitoring
  activeUsersGauge.add(1);
  
  // Update current user level and track maximum serving capacity
  const currentVUs = __VU;
  if (currentVUs > currentUserLevel) {
    currentUserLevel = currentVUs;
  }
  
  // Select and execute scenario
  const selectedScenario = selectWeightedScenario();
  let response;
  
  try {
    response = selectedScenario();
  } catch (error) {
    errorRate.add(1);
    requestCounter.add(1);
    sleep(1); // Brief back off on errors
    return;
  }
  
  // Record metrics
  const duration = response.timings.duration;
  responseTimeTrend.add(duration);
  requestCounter.add(1);
  const isError = response.status >= 400;
  errorRate.add(isError);
  
  // Track maximum serving capacity (if this request was successful)
  if (!isError && currentVUs > maxServingCapacity) {
    maxServingCapacity = currentVUs;
  }
  
  // Simple performance checks
  check(response, {
    'status is 2xx': (r) => r.status >= 200 && r.status < 300,
    'response time < 10s': (r) => r.timings.duration < 10000,
    'has content or valid empty': (r) => r.status === 204 || (r.body && r.body.length > 0),
  });
  
  // Adaptive sleep based on performance
  let sleepTime = duration < 500 ? 0.1 : 
                  duration < 2000 ? 0.5 : 
                  duration < 5000 ? 1.0 : 2.0;
  
  sleep(sleepTime + Math.random() * 0.2);
  
  // Summary logging every 200 iterations (reduced frequency)
  if (__ITER % 200 === 0 && __ITER > 0) {
    let zone = duration < 500 ? "🟢" : duration < 1000 ? "🟡" : duration < 3000 ? "🟠" : "🔴";
    console.log(`[${currentVUs} users] ${__ITER} requests | ${duration.toFixed(0)}ms ${zone}`);
  }
}