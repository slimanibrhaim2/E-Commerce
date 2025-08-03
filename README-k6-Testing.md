# k6 Testing Suite for E-Commerce API

This repository contains **fully configurable** k6 testing scripts for the E-Commerce API covering smoke tests, load tests, and **simple stress testing**. **Focuses on Products and Categories APIs only.** Features automatic stress limit discovery with clean, meaningful results!

## ✨ Key Features

- 🎛️ **Fully Configurable** - All parameters via environment variables
- 📈 **Scalable** - Scale users and duration with simple multipliers
- 🌍 **Multi-Environment** - Easy switching between dev/staging/production
- 🎯 **Auto-Stress Discovery** - Automatically finds and reports maximum stress limits
- 🛑 **Smart Auto-Stop** - Stops at 50% failure rate with clear results
- 🚀 **Clean Results** - No spam logs, meaningful stress reports only

## Prerequisites

1. **k6 Installation**
   ```bash
   # Windows (using Chocolatey)
   choco install k6

   # Windows (using winget)
   winget install k6

   # macOS (using Homebrew)
   brew install k6

   # Linux (Debian/Ubuntu)
   sudo gpg -k
   sudo gpg --no-default-keyring --keyring /usr/share/keyrings/k6-archive-keyring.gpg --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys C5AD17C747E3415A3642D57D77C6C491D6AC1D69
   echo "deb [signed-by=/usr/share/keyrings/k6-archive-keyring.gpg] https://dl.k6.io/deb stable main" | sudo tee /etc/apt/sources.list.d/k6.list
   sudo apt-get update
   sudo apt-get install k6
   ```

2. **API Server Running**
   - Default: `http://localhost:44362`
   - Configurable via `BASE_URL` environment variable

## Configuration

### Environment Variables (All Optional)

See `test.env` for all available options:

```bash
# API Configuration
BASE_URL=http://localhost:44362

# Global Scaling (multiply all values)
SCALE_USERS=1        # 0.5 = half users, 2 = double users
SCALE_DURATION=1     # 0.5 = half time, 2 = double time

# Smoke Test (defaults shown)
SMOKE_VUS=1
SMOKE_DURATION=60
SMOKE_RESPONSE_TIME=1000
SMOKE_ERROR_RATE=0.01

# Load Test Virtual Users
LOAD_SMALL_VUS=50    # Stage 1 users
LOAD_MEDIUM_VUS=200  # Stage 2 users  
LOAD_LARGE_VUS=500   # Stage 3 users (peak)

# Load Test Durations (seconds)
LOAD_RAMP_UP=120     # Time to ramp between stages
LOAD_HOLD=300        # Time to hold each stage
LOAD_RAMP_DOWN=180   # Time to ramp down

# Simple Stress Test (Current Focus)
STRESS_VUS_STEP1=50       # Start: 50 users
STRESS_VUS_STEP2=100      # 100 users
STRESS_VUS_STEP3=200      # 200 users
# ... continues incrementally up to...
STRESS_PEAK_VUS=2000      # Maximum: 2000 users

# Stress Test Timing
STRESS_RAMP_TIME=30       # 30s ramp to each level
STRESS_HOLD_TIME=30       # 30s hold at each level (1 min total per step)

# Performance Thresholds
LOAD_RESPONSE_TIME=500    # Max response time (ms)
LOAD_ERROR_RATE=0.05      # Max error rate (5%)
STRESS_RESPONSE_TIME=2000 # Relaxed for stress test
STRESS_ERROR_RATE=0.10    # Relaxed for stress test (10%)
```

## Quick Start Examples

### 1. Simple Stress Test (Recommended)
```bash
# Run the simple stress test to find your maximum stress limits
run-all-tests.bat         # Windows - Runs stress test only
./run-all-tests.sh        # Linux/Mac - Runs stress test only

# Or run directly
k6 run stress-test.js     # 50→100→200→...→2000 users, auto-stops at 50% failure

# Individual tests (currently commented out in run-all-tests)
k6 run smoke-test.js      # 1 user, 1 min
k6 run load-test.js       # up to 500 users
```

### 2. Different Environment
```bash
# Production testing
BASE_URL=https://api.mycompany.com k6 run load-test.js

# Staging with reduced load
BASE_URL=https://staging-api.mycompany.com SCALE_USERS=0.5 k6 run load-test.js
```

### 3. Custom Scenarios
```bash
# Quick test (half users, half time)
SCALE_USERS=0.5 SCALE_DURATION=0.5 k6 run smoke-test.js

# Massive stress test
STRESS_PEAK_VUS=10000 k6 run stress-test.js

# Endurance test (30 min)
LOAD_LARGE_VUS=200 LOAD_HOLD=1800 k6 run load-test.js

# Performance regression (strict thresholds)
LOAD_RESPONSE_TIME=100 LOAD_ERROR_RATE=0.001 k6 run load-test.js
```

### 4. Environment-Specific Setups
```bash
# Development (minimal load)
export BASE_URL=http://localhost:5000
export SCALE_USERS=0.1
export SCALE_DURATION=0.5
k6 run load-test.js

# CI/CD (quick validation)
export SMOKE_DURATION=30
export SMOKE_RESPONSE_TIME=2000
k6 run smoke-test.js

# Production capacity test
export SCALE_USERS=2
export LOAD_RESPONSE_TIME=200
export BASE_URL=https://api.mycompany.com
k6 run load-test.js
```

## Test Types

### 1. Smoke Test (`smoke-test.js`)
- **Purpose**: Quick health check
- **Default**: 1 user for 1 minute
- **Configurable**: All aspects via environment variables
- **Endpoints**: Basic API endpoints (anonymous access)
- **Use Case**: CI/CD validation, basic health checks

**Configuration Options:**
- `SMOKE_VUS`: Number of virtual users (default: 1)
- `SMOKE_DURATION`: Test duration in seconds (default: 60)
- `SMOKE_RESPONSE_TIME`: Max response time threshold (default: 1000ms)
- `SMOKE_ERROR_RATE`: Max error rate threshold (default: 1%)

### 2. Load Test (`load-test.js`)
- **Purpose**: Normal capacity testing
- **Default**: Ramps from 1 → 50 → 200 → 500 users
- **Configurable**: All user counts, durations, and thresholds
- **Endpoints**: CRUD operations, search, filtering
- **Use Case**: Verify normal operation under expected load

**Configuration Options:**
- `LOAD_SMALL_VUS`, `LOAD_MEDIUM_VUS`, `LOAD_LARGE_VUS`: User counts for each stage
- `LOAD_RAMP_UP`, `LOAD_HOLD`, `LOAD_RAMP_DOWN`: Duration controls
- `LOAD_RESPONSE_TIME`, `LOAD_ERROR_RATE`: Performance thresholds

### 3. Simple Stress Test (`stress-test.js`) - **CURRENT FOCUS**
- **Purpose**: Automatically find your exact maximum stress limits
- **Pattern**: Simple incremental: 50 → 100 → 200 → 300 → 400 → 500 → ... → 2000 users
- **Duration**: 1 minute per step (30s ramp + 30s hold)
- **Auto-Stop**: Stops automatically when 50% of requests fail
- **Output**: Prints exact maximum stress capacity + production recommendations
- **Endpoints**: All available endpoints with weighted realistic usage

**What You Get:**
```
🏆 MAXIMUM SERVING CAPACITY: 650 USERS
💡 Production recommendations:
   • Target capacity: 455 users (70% of max)
   • Auto-scaling trigger: 520 users (80% of max)
   • Emergency capacity: 650 users (100% of max)
```

**Configuration Options:**
- `STRESS_VUS_STEP1` through `STRESS_PEAK_VUS`: User counts for each step (50 to 2000)
- `STRESS_RAMP_TIME`, `STRESS_HOLD_TIME`: 30s each (1 min per step)
- Auto-stops at 50% failure rate - no manual analysis needed!

## Global Scaling

Use scaling factors to adjust all tests proportionally:

```bash
# Half the load (great for development)
export SCALE_USERS=0.5
export SCALE_DURATION=0.5

# Double the load (stress test++)
export SCALE_USERS=2
export SCALE_DURATION=2

# Quick tests (1/4 time)
export SCALE_DURATION=0.25
```

## Files Structure

- `config.js`: **Central configuration system**
- `test.env`: **Environment variable examples**
- `smoke-test.js`: Configurable smoke test
- `load-test.js`: Configurable load test
- `stress-test.js`: Configurable stress test
- `run-all-tests.bat`: Windows batch runner
- `run-all-tests.sh`: Linux/Mac shell runner

## Monitoring & Metrics

All tests provide comprehensive metrics:

- **Response Time**: p50, p90, p95, p99 percentiles
- **Error Rate**: HTTP failures and custom error tracking
- **Throughput**: Requests per second
- **Custom Metrics**: Business-specific measurements

## Stress Test Results

The simple stress test automatically provides:

### Automatic Stress Discovery
- **Exact Number**: Your maximum stress capacity (e.g., 650 users)
- **Production Target**: 70% of max for normal operations
- **Auto-scaling Trigger**: 80% of max for scaling decisions
- **Emergency Capacity**: 100% of max for peak demand

### Expected Output
```
🎯 SIMPLE STRESS TEST - Find Breaking Point
📊 Pattern: 50 → 100 → 200 → 300 → 400 → 500 → ... → 2000 users
⏱️  Duration: 1 minute per step (auto-stop at 50% failure rate)
✅ API ready - baseline: 45ms
🚀 Starting incremental stress test...

[150 users] 200 requests | 234ms 🟢
[300 users] 200 requests | 890ms 🟡
[450 users] 200 requests | 2100ms 🔴

🎯 STRESS TEST COMPLETED
⏱️  Duration: 8.5 minutes
👥 Maximum Users Reached: 650

🏆 MAXIMUM SERVING CAPACITY: 550 USERS
💡 This is the highest number of users your system can serve successfully
📈 Production recommendations:
   • Target capacity: 385 users (70% of max)
   • Auto-scaling trigger: 440 users (80% of max)
   • Emergency capacity: 550 users (100% of max)
```

## Best Practices

1. **Use Stress Test First**: Run the simple stress test to establish your baseline
2. **Environment-Specific**: Test each environment separately (dev/staging/prod)
3. **Regular Testing**: Re-run stress tests after significant changes
4. **Production Planning**: Use 70% of discovered capacity as your target
5. **Auto-scaling**: Set triggers at 80% of discovered capacity

## VU/Time Data for Plotting

For plotting the stress test progression in Excel or other tools, here are the **Time (minutes) and Virtual Users** pairs:

| Time (دقيقة) | عدد المستخدمين |
|-------------|---------------|
| 0.5         | 50           |
| 1.5         | 100          |
| 2.5         | 200          |
| 3.5         | 300          |
| 4.5         | 400          |
| 5.5         | 500          |
| 6.5         | 600          |
| 7.5         | 700          |
| 8.5         | 800          |
| 9.5         | 900          |
| 10.5        | 1000         |
| 11.5        | 1200         |
| 12.5        | 1400         |
| 13.5        | 1600         |
| 14.5        | 1800         |
| 15.5        | 2000         |

*Note: Test auto-stops when 50% failure rate is reached, so actual plot may be shorter.*

## Troubleshooting

### Common Issues

1. **Test Stops Too Early**: Your capacity is lower than expected - this is the real result!
2. **API Not Responding**: Check if your API server is running at the configured BASE_URL
3. **Very Low Capacity**: Consider optimizing your API before scaling infrastructure
4. **Need Faster Testing**: Reduce `STRESS_RAMP_TIME` and `STRESS_HOLD_TIME` to 15s each

### Environment Variables Not Working?

Make sure to export them:
```bash
# ❌ Wrong
SCALE_USERS=0.5 
k6 run load-test.js

# ✅ Correct
export SCALE_USERS=0.5
k6 run load-test.js

# Or inline
SCALE_USERS=0.5 k6 run load-test.js
```

## Contributing

When adding new tests or modifying existing ones:

1. **Use the config system** - No hardcoded values!
2. **Add environment variables** to `test.env`
3. **Update documentation** with new options
4. **Test with different scales** to ensure flexibility

---

**Automatic stress discovery with clean, actionable results!** 🏆

**Key Benefits:**
- ✅ **Auto-Discovery**: Finds exact maximum stress capacity
- ✅ **Auto-Stop**: Stops at 50% failure rate automatically  
- ✅ **Clean Results**: No manual analysis needed
- ✅ **Production Ready**: Provides scaling recommendations
- ✅ **Fast Testing**: ~5-15 minutes vs hours of manual testing