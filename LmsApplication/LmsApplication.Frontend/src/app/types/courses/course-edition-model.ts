export interface CourseEditionModel {
  id: string,
  courseId: string,
  duration: number,
  startDateUtc: Date,
  endDateUtc: Date,
  studentLimit: number,
  studentIds: string[],
  teacherIds: string[],
}
