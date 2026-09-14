export interface EmployeeBookingData {
  employee: EmployeeDetails;
  expectedAlerts: string[];
}

export interface EmployeeDetails {
  firstName: string;
  surname: string;
  email: string;
}
