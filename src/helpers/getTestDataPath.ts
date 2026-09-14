import path from 'path';

/** Returns the test data root path. */
export function getTestDataPath(): string {
  return process.env.TEST_DATA_PATH?.trim() || path.resolve(process.cwd(), 'TestData');
}
