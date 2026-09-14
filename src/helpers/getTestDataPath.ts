import path from 'path';
import { PlaywrightConfig } from '../config/playwrightConfig';

/** Returns the test data root path from the same runtime config source used by tests. */
export function getTestDataPath(): string {
  return path.resolve(PlaywrightConfig.fromEnv().testDataPath);
}
