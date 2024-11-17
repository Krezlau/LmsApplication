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
import { CommentModel } from '../../types/course-board/comment-model';
import { CommentCreateModel } from '../../types/course-board/comment-create-model';
import { AlertService } from '../../services/alert.service';
import { CommentService } from '../../services/comment.service';
import { AuthService } from '../../services/auth.service';
import { CalculateNumberOfRows } from '../../helpers/calculate-number-of-rows';
import { ApiResponse } from '../../types/api-response';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-edit-comment',
  standalone: true,
  imports: [NgIf, ReactiveFormsModule],
  templateUrl: './edit-comment.component.html',
})
export class EditCommentComponent implements OnInit, OnDestroy {
  @Input() currentContent: string = '';
  @Input() commentId: string = '';
  @Input() authorId: string = '';
  @Input() postId: string = '';
  @Input() editionId: string = '';

  @Output() contentUpdated: EventEmitter<CommentModel> =
    new EventEmitter<CommentModel>();
  initContent: string = '';
  model: CommentCreateModel;
  numberOfRows: number = 0;

  visible: boolean = false;
  isLoading: boolean = false;
  myForm!: FormGroup;
  private UpdateCommentSubscription?: Subscription;

  constructor(
    private formBuilder: FormBuilder,
    private alertService: AlertService,
    private commentService: CommentService,
    private authService: AuthService,
  ) {
    this.model = {
      content: '',
    };
  }
  onFormSubmit(): void {
    if (
      this.myForm.valid &&
      this.initContent !== this.myForm.value.commentContent
    ) {
      this.isLoading = true;
      this.currentContent = this.myForm.value.commentContent;
      this.model.content = this.currentContent;
      this.UpdateCommentSubscription = this.commentService
        .updateComment(this.editionId, this.postId, this.commentId, this.model)
        .subscribe(
          (response: ApiResponse<CommentModel>) => {
            this.alertService.show('Comment updated successfully', 'success'),
              this.contentUpdated.emit(response.data!),
              (this.isLoading = false);
          },
          (error) => {
            this.isLoading = false;
          },
        );
    } else if (this.initContent === this.myForm.value.commentContent) {
      this.closeForm();
    } else {
      this.alertService.show('Type content', 'error');
    }
  }

  closeForm() {
    this.contentUpdated.emit();
  }

  ngOnInit(): void {
    this.initContent = this.currentContent;
    if (this.authorId === this.authService.authState().userData?.id)
      this.visible = true;
    this.myForm = this.formBuilder.group({
      commentContent: [
        this.currentContent,
        [Validators.required, Validators.minLength(1)],
      ],
    });
    this.numberOfRows = CalculateNumberOfRows.calculateNumberOfRows(
      this.currentContent,
    );
  }
  ngOnDestroy(): void {
    this.UpdateCommentSubscription?.unsubscribe();
  }
}
