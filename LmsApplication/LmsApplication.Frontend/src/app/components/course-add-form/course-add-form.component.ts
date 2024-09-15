import {Component, ElementRef, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {FormControl, FormsModule, ReactiveFormsModule} from "@angular/forms";
import {CourseService} from "../../services/course.service";
import {CourseDuration} from "../../types/courses/course-duration";
import {CourseCategory} from "../../types/courses/course-category";
import {Subscription} from "rxjs";
import {NgForOf, NgIf} from "@angular/common";

@Component({
  selector: 'app-course-add-form',
  standalone: true,
  imports: [
    FormsModule,
    ReactiveFormsModule,
    NgForOf,
    NgIf
  ],
  templateUrl: './course-add-form.component.html'
})
export class CourseAddFormComponent implements OnInit, OnDestroy {
  titleFormControl = new FormControl('');
  descriptionFormControl = new FormControl('');
  durationFormControl = new FormControl(CourseDuration.OneSemester);

  chosenCategories: CourseCategory[] = [];
  categories: CourseCategory[] = [];

  sub: Subscription = new Subscription();
  createCourseLoading = false;
  categoriesLoading = false;

  @ViewChild('dialogElement') dialogElement: ElementRef | undefined;

  constructor(private courseService: CourseService) {
  }

  ngOnInit(): void {
    this.categoriesLoading = true;
    this.sub.add(this.courseService.getCategories().subscribe(categories => {
      this.categories = categories;
      this.categoriesLoading = false;
    }));
  }

  onSelect(event: any) {
    console.log(event.target.value);
    const category = this.categories.find(c => c.id === event.target.value);
    if (!category) return;

    this.chosenCategories.push(category);
    console.log(this.chosenCategories);
  }

  removeCategory(category: CourseCategory) {
    this.chosenCategories = this.chosenCategories.filter(c => c.id !== category.id);
  }

  onSubmit() {
    this.createCourseLoading = true;

    this.courseService.createCourse({
        title: this.titleFormControl.value!,
        description: this.descriptionFormControl.value!,
        duration: +this.durationFormControl.value!,
        categories: this.chosenCategories.map(c => c.id)
      }).subscribe(() => {
        this.titleFormControl.setValue('');
        this.descriptionFormControl.setValue('');
        this.durationFormControl.setValue(CourseDuration.OneSemester);
        this.chosenCategories = [];
        this.createCourseLoading = false;
        this.dialogElement?.nativeElement.close();
    });
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  protected readonly CourseDuration = CourseDuration;
}
