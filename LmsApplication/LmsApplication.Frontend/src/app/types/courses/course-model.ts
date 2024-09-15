import {CourseDuration} from "./course-duration";
import {CourseCategory} from "./course-category";

export interface CourseModel {
  id: string;
  title: string;
  description: string;
  categories: CourseCategory[];
  duration: CourseDuration;
}
