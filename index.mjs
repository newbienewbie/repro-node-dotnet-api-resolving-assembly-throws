import { Main } from './bin/Release/net8.0/publish/NodeApiRepro.mjs';

const mode = process.argv[2];

// Run each mode in its own process: the first touch can only happen once per process.
try {
  if (mode === 'js') {
    Main.touchOnJsThread();
  } else {
    Main.touchOnPoolThread();
  }
  console.log(`[${mode}] RESULT: no exception`);
} catch (e) {
  console.log(`[${mode}] RESULT: ${(e?.message ?? e).split('\n')[0]}`);
}
