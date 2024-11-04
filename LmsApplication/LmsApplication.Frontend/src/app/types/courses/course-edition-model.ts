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
}
