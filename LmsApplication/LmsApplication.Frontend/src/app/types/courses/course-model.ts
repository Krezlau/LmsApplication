import {CourseDuration} from "./course-duration";

export interface CourseModel {
  id: string;
  title: string;
  description: string;
  categories: string[];
  duration: CourseDuration;
}
