@echo off
setlocal EnableDelayedExpansion

REM Load Testing Configuration
REM Set these environment variables to customize tests

REM Check for custom configuration
if not defined BASE_URL set BASE_URL=http://localhost:44362
if not defined SCALE_USERS set SCALE_USERS=1
if not defined SCALE_DURATION set SCALE_DURATION=1

echo ================================
echo E-Commerce API Load Testing Suite
echo ================================
echo Configuration:
echo   BASE_URL: %BASE_URL%
echo   SCALE_USERS: %SCALE_USERS%
echo   SCALE_DURATION: %SCALE_DURATION%
echo ================================
echo.

REM COMMENTED OUT - Running only stress test for stress testing
REM echo Starting Smoke Test...
REM echo ================================
REM k6 run smoke-test.js
REM if %ERRORLEVEL% neq 0 (
REM     echo ❌ Smoke test failed! Stopping execution.
REM     echo.
REM     echo 💡 Tips:
REM     echo   - Check if API is running at %BASE_URL%
REM     echo   - Try: set BASE_URL=http://localhost:5000
REM     echo   - Try: set SCALE_USERS=0.1 for minimal load
REM     pause
REM     exit /b 1
REM )
REM echo ✅ Smoke test passed!
REM echo.

REM echo Waiting 30 seconds before load test...
REM echo   (Set SKIP_DELAYS=1 to skip waits)
REM if not defined SKIP_DELAYS timeout /t 30 /nobreak > nul

REM echo Starting Load Test...
REM echo ================================
REM k6 run load-test.js
REM if %ERRORLEVEL% neq 0 (
REM     echo ❌ Load test failed!
REM     echo.
REM     echo 💡 Tips:
REM     echo   - Try: set LOAD_LARGE_VUS=100 for lower load
REM     echo   - Try: set LOAD_RESPONSE_TIME=1000 for relaxed thresholds
REM     echo.
REM ) else (
REM     echo ✅ Load test completed!
REM     echo.
REM )

echo Starting Stress Test - Continue Until Failure...
echo ================================
echo   Testing 5 -^> 10 -^> 20 -^> 30 -^> 40 -^> 50 -^> 75 -^> 100 -^> ... -^> up to 10,000 users
echo   Auto-stops at 50%% failure rate - 1 minute per step
echo   Press Ctrl+C to cancel or wait 5 seconds...
if not defined SKIP_DELAYS (
    timeout /t 5 /nobreak > nul
)
echo.

k6 run stress-test.js
if %ERRORLEVEL% neq 0 (
    echo [X] Stress test stopped (hit 50%% failure rate)
    echo.
    echo [RESULT] Check the MAXIMUM SERVING CAPACITY number printed above!
    echo.
) else (
    echo [OK] Stress test completed all 2000 users!
    echo.
)

echo ================================
echo Stress test completed!
echo Look for "MAXIMUM SERVING CAPACITY" in the results above.
echo ================================
echo.
echo [INFO] The test automatically calculated:
echo   - Your maximum serving capacity
echo   - Production target (70%% of max)
echo   - Auto-scaling trigger (80%% of max)
echo   - Emergency capacity (100%% of max)
echo.
pause