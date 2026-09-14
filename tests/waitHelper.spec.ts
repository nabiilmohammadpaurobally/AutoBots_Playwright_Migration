import { test, expect } from '@playwright/test';
import { staticWait } from '../src/extensions/playwrightExtensions';

test.describe('wait helpers', () => {
  test('staticWait handles zero', async () => {
    await staticWait(0);
    expect(true).toBeTruthy();
  });

  test('staticWait rejects negative values', async () => {
    await expect(staticWait(-1)).rejects.toThrow(/non-negative integer/);
  });

  test('staticWait rejects non-integer values', async () => {
    await expect(staticWait(1.5)).rejects.toThrow(/non-negative integer/);
    await expect(staticWait(Number.NaN)).rejects.toThrow(/non-negative integer/);
    await expect(staticWait(Number.POSITIVE_INFINITY)).rejects.toThrow(/non-negative integer/);
  });
});
