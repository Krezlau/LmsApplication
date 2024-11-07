import { CourseModel } from './course-model';

export interface CourseEditionModel {
  id: string;
  title: string;
  duration: number;
  startDateUtc: Date;
  endDateUtc: Date;
  studentLimit: number;
  studentCount: number;
  registrationStartDateUtc: Date | null;
  registrationEndDateUtc: Date | null;
  status: CourseEditionStatus;
  isUserRegistered: boolean;
  course: CourseModel;
}

export enum CourseEditionStatus {
  Planned,
  RegistrationOpen,
  RegistrationClosed,
  InProgress,
  Finished,
}
