import { defineConfig, mergeConfig, type UserConfig } from "vite";
import react from "@vitejs/plugin-react";

interface ViteConfigOptions {
  /** Nome da organização */
  orgName: string;
  /** Nome do projeto */
  projectName: string;
  /** Modo de build (development/production) */
  mode?: string;
  /**
   * Formato de saída do bundle
   * @default 'system'
   */
  format?: "system" | "es" | "cjs" | "iife" | "umd" | "amd";
  /**
   * Caminho para o arquivo tsconfig.json
   * @default './tsconfig.json'
   */
  tsconfigPath?: string;
}
/**
 * Configuração do Vite com suporte a aliases automáticos
 *
 * @example
 * ```ts
 * import { viteConfig } from '@bem-lib/bem-configs-vite';
 *
 * export default viteConfig({
 *   orgName: 'bem',
 *   projectName: 'my-project',
 *   mode: 'development',
 *   tsconfigPath: './tsconfig.json' // opcional
 * });
 * ```
 */
declare const viteConfig: (opts: ViteConfigOptions) => UserConfig;

export default defineConfig(({ mode }) => {
  const orgName = "bem";
  const projectName = "custodia";

  const config = viteConfig({
    mode,
    orgName,
    projectName,
    format: "system",
  });

  return mergeConfig(config, {
    plugins: [react()],
    build: {
      rollupOptions: {
        external: ["axios", "@bem-lib/pubsub"],
        output: {
          format: "system",
          globals: {
            react: "React",
            "react-dom": "ReactDOM",
          },
          // Força o nome exato do arquivo de saída
          entryFileNames: "bem-custodia.js",
          assetFileNames: "bem-custodia.[ext]",
        },
      },
    },
    server: {
      open: "/custodia",
      cors: true,
    },
    optimizeDeps: {
      include: ["@bem-lib/vite-configs"],
    },
  });
});
