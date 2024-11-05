export interface CourseEditionModel {
  id: string;
  title: string;
  courseId: string;
  duration: number;
  startDateUtc: Date;
  endDateUtc: Date;
  studentLimit: number;
  studentIds: string[];
  teacherIds: string[];
  registrationStartDateUtc: Date | null;
  registrationEndDateUtc: Date | null;
  status: CourseEditionStatus;
}

export enum CourseEditionStatus {
  Planned,
  RegistrationOpen,
  RegistrationClosed,
  InProgress,
  Finished,
}
