import { defineConfig, devices } from '@playwright/test';

/**
 * Read environment variables from file.
 * @see {@link https://github.com/motdotla/dotenv}
 */

// import dotenv from 'dotenv';
// import path from 'path';
// dotenv.config({ path: path.resolve(__dirname, '.env') });

/**
 * @see {@link https://playwright.dev/docs/test-configuration}
 */

export default defineConfig({
  /* Cada teste deve rodar no máximo em 60s e 120s no CI(até melhorar a config da VM que roda na azure). */
  timeout: (process.env.CI ? 120 : 60) * 1000,
  testDir: './src/e2e',
  testMatch: '**/*.spec.e2e.ts',
  /* Run tests in files in parallel */
  fullyParallel: true,
  /* Fail the build on CI if you accidentally left test.only in the source code. */
  forbidOnly: !!process.env.CI,
  /* Retry on CI only */
  retries: process.env.CI ? 2 : 0,
  /* Opt out of parallel tests on CI. */
  workers: process.env.CI ? 3 : 4,
  /* Reporter to use. See https://playwright.dev/docs/test-reporters */
  reporter: 'html', // Com reporter
  // reporter: 'list', // Sem reporter
  /* Shared settings for all the projects below. See https://playwright.dev/docs/api/class-testoptions. */
  use: {
    headless: false, // Sem browser
    // headless: false, // Com browser
    /* Collect trace when retrying the failed test. See https://playwright.dev/docs/trace-viewer */
    trace: 'on-first-retry',
  },
  /* Configure projects for major browsers */
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
    // {
    //   name: 'firefox',
    //   use: { ...devices['Desktop Firefox'] },
    // },
    // {
    //   name: 'webkit',
    //   use: { ...devices['Desktop Safari'] },
    // },
  ],
  /* Run your local dev server before starting the tests */
  webServer: {
    timeout: 120_000,
    command: 'pnpm standalone',
    url: 'http://localhost:9000',
    reuseExistingServer: !process.env.CI,
  },
});
