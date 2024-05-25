export enum UserRole {
  Admin,
  Teacher,
  Student
}

export function toHumanReadable(userRole: UserRole) {
  switch (userRole) {
    case UserRole.Admin:
      return 'Admin';
    case UserRole.Teacher:
      return 'Teacher';
    case UserRole.Student:
      return 'Student';
  }
}
