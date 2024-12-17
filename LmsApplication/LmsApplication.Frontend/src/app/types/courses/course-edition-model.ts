import { FinalGradeModel } from '../course-board/grade-model';
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
  settings: CourseEditionPublicSettings;
  finalGrade: FinalGradeModel | null;
}

export enum CourseEditionStatus {
  Planned,
  RegistrationOpen,
  RegistrationClosed,
  InProgress,
  Finished,
}

export interface CourseEditionPublicSettings {
  allowAllToPost: boolean;
}
