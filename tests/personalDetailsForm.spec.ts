import { test, expect } from '@playwright/test';
import { matchesExpectedAlert } from '../src/pageObjects/neonatal/personalDetailsFormPlaywright';

test.describe('PersonalDetailsForm alert matching', () => {
  test('matches expected alert text case-insensitively', () => {
    expect(matchesExpectedAlert('Booking Created Successfully', ['success', 'warning'])).toBeTruthy();
  });

  test('returns false when no expected message matches', () => {
    expect(matchesExpectedAlert('Unknown response', ['success', 'warning'])).toBeFalsy();
  });
});
