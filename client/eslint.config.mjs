import eslint from '@eslint/js'
import { defineConfig } from 'eslint/config'
import tseslint from 'typescript-eslint'
import reactPlugin from 'eslint-plugin-react'
import eslintConfigPrettier from 'eslint-config-prettier/flat'

export default defineConfig(
    eslint.configs.recommended,
    ...tseslint.configs.recommended,
    reactPlugin.configs.flat.recommended,
    reactPlugin.configs.flat['jsx-runtime'],

    {
        settings: {
            react: {
                version: 'detect'
            },
        },
        rules: {
            'react/react-in-jsx-scope' : 'off',
        },
    },
    eslintConfigPrettier,
)