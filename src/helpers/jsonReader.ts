import fs from 'fs';
import path from 'path';
import { getTestDataPath } from './getTestDataPath';

/** Reads typed JSON data from TestData while preserving existing flow semantics. */
export function fetchData<T>(jsonName: string): T {
  const filePath = path.join(getTestDataPath(), `${jsonName}.json`);
  if (!fs.existsSync(filePath)) {
    throw new Error(`JSON file not found: ${filePath}`);
  }

  const content = fs.readFileSync(filePath, 'utf-8');
  return JSON.parse(content) as T;
}
