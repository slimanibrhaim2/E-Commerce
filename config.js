// Load testing configuration
export const config = {
  // Base configuration
  baseUrl: __ENV.BASE_URL || 'http://localhost:44362',
  
  // Test scaling factors
  scale: {
    users: parseInt(__ENV.SCALE_USERS) || 1,
    duration: parseFloat(__ENV.SCALE_DURATION) || 1,
  },
  
  // Virtual Users configuration
  vus: {
    smoke: parseInt(__ENV.SMOKE_VUS) || 1,
    load: {
      small: parseInt(__ENV.LOAD_SMALL_VUS) || 50,
      medium: parseInt(__ENV.LOAD_MEDIUM_VUS) || 150,
      large: parseInt(__ENV.LOAD_LARGE_VUS) || 300,
    },
    stress: {
      // Continue until failure pattern: 5 → 10 → 20 → ... → up to 10000
      step1: parseInt(__ENV.STRESS_VUS_STEP1) || 5,
      step2: parseInt(__ENV.STRESS_VUS_STEP2) || 10,
      step3: parseInt(__ENV.STRESS_VUS_STEP3) || 20,
      step4: parseInt(__ENV.STRESS_VUS_STEP4) || 30,
      step5: parseInt(__ENV.STRESS_VUS_STEP5) || 40,
      step6: parseInt(__ENV.STRESS_VUS_STEP6) || 50,
      step7: parseInt(__ENV.STRESS_VUS_STEP7) || 75,
      step8: parseInt(__ENV.STRESS_VUS_STEP8) || 100,
      step9: parseInt(__ENV.STRESS_VUS_STEP9) || 150,
      step10: parseInt(__ENV.STRESS_VUS_STEP10) || 200,
      step11: parseInt(__ENV.STRESS_VUS_STEP11) || 300,
      step12: parseInt(__ENV.STRESS_VUS_STEP12) || 400,
      step13: parseInt(__ENV.STRESS_VUS_STEP13) || 500,
      step14: parseInt(__ENV.STRESS_VUS_STEP14) || 600,
      step15: parseInt(__ENV.STRESS_VUS_STEP15) || 700,
      step16: parseInt(__ENV.STRESS_VUS_STEP16) || 800,
      step17: parseInt(__ENV.STRESS_VUS_STEP17) || 900,
      step18: parseInt(__ENV.STRESS_VUS_STEP18) || 1000,
      step19: parseInt(__ENV.STRESS_VUS_STEP19) || 1200,
      step20: parseInt(__ENV.STRESS_VUS_STEP20) || 1400,
      step21: parseInt(__ENV.STRESS_VUS_STEP21) || 1600,
      step22: parseInt(__ENV.STRESS_VUS_STEP22) || 1800,
      step23: parseInt(__ENV.STRESS_VUS_STEP23) || 2000,
      step24: parseInt(__ENV.STRESS_VUS_STEP24) || 2500,
      step25: parseInt(__ENV.STRESS_VUS_STEP25) || 3000,
      step26: parseInt(__ENV.STRESS_VUS_STEP26) || 4000,
      step27: parseInt(__ENV.STRESS_VUS_STEP27) || 5000,
      step28: parseInt(__ENV.STRESS_VUS_STEP28) || 6000,
      step29: parseInt(__ENV.STRESS_VUS_STEP29) || 7000,
      step30: parseInt(__ENV.STRESS_VUS_STEP30) || 8000,
      peak: parseInt(__ENV.STRESS_PEAK_VUS) || 10000,
    }
  },
  
  // Duration configuration (in seconds, will be converted to k6 format)
  duration: {
    smoke: parseInt(__ENV.SMOKE_DURATION) || 60,
    load: {
      rampUp: parseInt(__ENV.LOAD_RAMP_UP) || 120,
      hold: parseInt(__ENV.LOAD_HOLD) || 300,
      rampDown: parseInt(__ENV.LOAD_RAMP_DOWN) || 180,
    },
    stress: {
      rampTime: parseInt(__ENV.STRESS_RAMP_TIME) || 30,
      holdTime: parseInt(__ENV.STRESS_HOLD_TIME) || 30,
      rampDown: parseInt(__ENV.STRESS_RAMP_DOWN) || 60,
    }
  },
  
  // Performance thresholds
  thresholds: {
    smoke: {
      responseTime: parseInt(__ENV.SMOKE_RESPONSE_TIME) || 1000,
      errorRate: parseFloat(__ENV.SMOKE_ERROR_RATE) || 0.01,
    },
    load: {
      responseTime: parseInt(__ENV.LOAD_RESPONSE_TIME) || 1000,
      errorRate: parseFloat(__ENV.LOAD_ERROR_RATE) || 0.05,
    },
    stress: {
      responseTime: parseInt(__ENV.STRESS_RESPONSE_TIME) || 1000,
      avgResponseTime: parseInt(__ENV.STRESS_AVG_RESPONSE_TIME) || 500,
      errorRate: parseFloat(__ENV.STRESS_ERROR_RATE) || 0.15,
    }
  }
};

// Helper functions
export function formatDuration(seconds) {
  if (seconds < 60) return `${seconds}s`;
  const minutes = Math.floor(seconds / 60);
  const remainingSeconds = seconds % 60;
  if (remainingSeconds === 0) return `${minutes}m`;
  return `${minutes}m${remainingSeconds}s`;
}

export function scaleVUs(baseVUs) {
  return Math.max(1, Math.floor(baseVUs * config.scale.users));
}

export function scaleDuration(baseDuration) {
  return Math.max(1, Math.floor(baseDuration * config.scale.duration));
}

// Generate load test stages
export function generateLoadStages() {
  const { load } = config.vus;
  const { load: dur } = config.duration;
  
  return [
    { duration: formatDuration(scaleDuration(dur.rampUp)), target: scaleVUs(load.small) },
    { duration: formatDuration(scaleDuration(dur.hold)), target: scaleVUs(load.small) },
    { duration: formatDuration(scaleDuration(dur.rampUp)), target: scaleVUs(load.medium) },
    { duration: formatDuration(scaleDuration(dur.hold)), target: scaleVUs(load.medium) },
    { duration: formatDuration(scaleDuration(dur.rampUp)), target: scaleVUs(load.large) },
    { duration: formatDuration(scaleDuration(dur.hold * 2)), target: scaleVUs(load.large) },
    { duration: formatDuration(scaleDuration(dur.rampDown)), target: 0 },
  ];
}

// Generate simple stress test stages - 1 minute per step until 50% failure
export function generateStressStages() {
  const { stress } = config.vus;
  const { stress: dur } = config.duration;
  
  const stages = [];
  const steps = [
    stress.step1, stress.step2, stress.step3, stress.step4, stress.step5,
    stress.step6, stress.step7, stress.step8, stress.step9, stress.step10,
    stress.step11, stress.step12, stress.step13, stress.step14, stress.step15,
    stress.step16, stress.step17, stress.step18, stress.step19, stress.step20,
    stress.step21, stress.step22, stress.step23, stress.step24, stress.step25,
    stress.step26, stress.step27, stress.step28, stress.step29, stress.step30,
    stress.peak
  ];
  
  // Simple pattern: 30s ramp + 30s hold = 1 minute per step
  for (const stepUsers of steps) {
    stages.push(
      { duration: formatDuration(scaleDuration(dur.rampTime)), target: scaleVUs(stepUsers) },
      { duration: formatDuration(scaleDuration(dur.holdTime)), target: scaleVUs(stepUsers) }
    );
  }
  
  // Graceful ramp down
  stages.push({
    duration: formatDuration(scaleDuration(dur.rampDown)),
    target: 0
  });
  
  return stages;
}

export default config;