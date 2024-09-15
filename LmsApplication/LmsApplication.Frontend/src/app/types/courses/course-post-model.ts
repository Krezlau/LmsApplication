import {CourseDuration} from "./course-duration";

export interface CoursePostModel {
  title: string;
  description: string;
  duration: CourseDuration;
  categories: string[];
}
