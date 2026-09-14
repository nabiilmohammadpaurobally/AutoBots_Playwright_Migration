import fs from 'fs';
import path from 'path';
import { getTestDataPath } from './getTestDataPath';

/** Reads typed JSON data from TestData while preserving existing flow semantics. */
export function fetchData<T>(jsonName: string): T {
  const rootPath = path.resolve(getTestDataPath());
  const filePath = path.resolve(rootPath, `${jsonName}.json`);
  const relativePath = path.relative(rootPath, filePath);
  if (relativePath.startsWith('..') || path.isAbsolute(relativePath)) {
    throw new Error(`Invalid JSON path outside test data root: ${jsonName}`);
  }

  if (!fs.existsSync(filePath)) {
    throw new Error(`JSON file not found: ${filePath}`);
  }

  const content = fs.readFileSync(filePath, 'utf-8');
  return JSON.parse(content) as T;
}
