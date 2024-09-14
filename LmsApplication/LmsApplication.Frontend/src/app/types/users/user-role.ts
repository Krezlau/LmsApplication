export enum UserRole {
  Student,
  Teacher,
  Admin,
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
