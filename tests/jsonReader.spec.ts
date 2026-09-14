import { test, expect } from '@playwright/test';
import { fetchData } from '../src/helpers/jsonReader';
import { EmployeeBookingData } from '../src/models/neonatal/employeeBookingData';

test.describe('jsonReader', () => {
  test('loads typed JSON data', () => {
    const data = fetchData<EmployeeBookingData>('Neonatal/T386672_Employee_Books_Neonatal');
    expect(data.employee.firstName).toBe('John');
    expect(data.expectedAlerts.length).toBeGreaterThan(0);
  });

  test('throws for missing json file', () => {
    expect(() => fetchData<EmployeeBookingData>('Neonatal/not-found')).toThrow(/JSON file not found/);
  });

  test('blocks path traversal outside test data root', () => {
    expect(() => fetchData<EmployeeBookingData>('../package')).toThrow(/Invalid JSON path outside test data root/);
  });
});
