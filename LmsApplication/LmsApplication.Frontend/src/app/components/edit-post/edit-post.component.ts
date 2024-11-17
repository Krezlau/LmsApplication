import {
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { PostModel } from '../../types/course-board/post-model';
import { AlertService } from '../../services/alert.service';
import { PostService } from '../../services/post.service';
import { AuthService } from '../../services/auth.service';
import { NgIf } from '@angular/common';
import { PostCreateModel } from '../../types/course-board/post-create-model';
import { CalculateNumberOfRows } from '../../helpers/calculate-number-of-rows';
import { ApiResponse } from '../../types/api-response';

@Component({
  selector: 'app-edit-post',
  standalone: true,
  imports: [NgIf, ReactiveFormsModule],
  templateUrl: './edit-post.component.html',
  styleUrl: './edit-post.component.css',
})
export class EditPostComponent implements OnInit, OnDestroy {
  @Input() currentContent: string = '';
  @Input() imageId: string | null = null;
  @Input() postId: string = '';
  @Input() authorId: string = '';
  @Input() editionId: string = '';

  @Output() contentUpdated: EventEmitter<PostModel> = new EventEmitter<PostModel>();
  @Output() imageDeleted: EventEmitter<void> = new EventEmitter<void>();

  initContent: string = '';
  myForm!: FormGroup;
  visible: boolean = false;
  private UpdatePostSubscription?: Subscription;
  model: PostCreateModel;
  isLoading: boolean = false;
  numberOfRows: number = 0;

  constructor(
    private formBuilder: FormBuilder,
    private alertService: AlertService,
    private postService: PostService,
    private authService: AuthService,
  ) {
    this.model = {
      content: '',
      //mediaId: null,
    };
  }

  onFormSubmit(): void {
    if (this.myForm.valid && this.initContent !== this.myForm.value.content) {
      this.isLoading = true;
      this.currentContent = this.myForm.value.content;
      this.model.content = this.currentContent;
      //this.model.mediaId = this.imageId;
      this.UpdatePostSubscription = this.postService
        .updatePost(this.editionId, this.postId, this.model)
        .subscribe(
          (updatedPost: ApiResponse<PostModel>) => {
            this.alertService.show('Post updated successfully', 'success'),
              this.contentUpdated.emit(updatedPost.data!),
              (this.isLoading = false);
          },
          (error) => {
            this.isLoading = false;
          },
        );
    } else if (this.initContent === this.myForm.value.content) {
      this.closeForm();
    } else {
      this.alertService.show('Type content', 'error');
    }
  }

  closeForm() {
    this.contentUpdated.emit();
  }

  deleteImage() {
    this.imageId = null;
    this.imageDeleted.emit();
  }

  ngOnInit(): void {
    this.initContent = this.currentContent;
    if (this.authorId === this.authService.authState().userData?.id) this.visible = true;
    this.myForm = this.formBuilder.group({
      content: [
        this.currentContent,
        [Validators.required, Validators.minLength(1)],
      ],
    });
    this.numberOfRows = CalculateNumberOfRows.calculateNumberOfRows(
      this.currentContent,
    );
  }

  ngOnDestroy(): void {
    this.UpdatePostSubscription?.unsubscribe();
  }
}
