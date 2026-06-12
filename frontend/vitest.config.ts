import path from 'node:path';
import url from 'url';
import { defineConfig } from 'vitest/config';

const __filename = url.fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

export default defineConfig({
  resolve: {
    alias: {
      '@': `${path.resolve(__dirname, './src')}`,
      assets: `${path.resolve(__dirname, 'public/assets')}`,
      core: path.resolve(__dirname, './src/core'),
      e2e: path.resolve(__dirname, './src/e2e'),
      modules: path.resolve(__dirname, './src/modules'),
      public: path.resolve(__dirname, './public'),
      shared: path.resolve(__dirname, './src/shared'),
    },
  },
  test: {
    globals: true,
    environment: 'jsdom',
    include: ['src/**/*.{test,spec}.{ts,tsx,js,jsx}'],
    coverage: {
      enabled: false,
    },
  },
});
