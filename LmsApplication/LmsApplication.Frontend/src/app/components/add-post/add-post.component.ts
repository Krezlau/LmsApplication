import {
  Component,
  ElementRef,
  EventEmitter,
  OnDestroy,
  OnInit,
  Output,
  Input,
  signal,
  ViewChild,
} from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Subscription, tap } from 'rxjs';
import { PostModel } from '../../types/course-board/post-model';
import { PostService } from '../../services/post.service';
import { AlertService } from '../../services/alert.service';
import { PostCreateModel } from '../../types/course-board/post-create-model';
import { ApiResponse } from '../../types/api-response';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-add-post',
  standalone: true,
  imports: [NgIf, ReactiveFormsModule],
  templateUrl: './add-post.component.html',
})
export class AddPostComponent implements OnInit, OnDestroy {
  @Input() editionId: string = '';
  @Output() contentAdded = new EventEmitter<PostModel>();
  private addPostSubscription?: Subscription;
  model: PostCreateModel;
  isLoading: boolean = false;

  myForm!: FormGroup;

  //selectedFile: ImageSnippet | null = null;

  @ViewChild('imageInput')
  fileInputElement: ElementRef = new ElementRef(null);

  imageLoading = signal(false);
  deleteImageLoading = signal(false);

  imageSubscription = new Subscription();

  constructor(
    private formBuilder: FormBuilder,
    private postService: PostService,
    private alertService: AlertService,
    //private imageService: ImageService,
  ) {
    this.model = {
      content: '',
      //mediaId: null,
    };
  }

  onFormSubmit(): void {
    if (this.myForm.valid) {
      this.isLoading = true;
      this.model.content = this.myForm.value.content;
      /* this.model.mediaId = this.selectedFile?.guid
        ? this.selectedFile.guid
        : null; */
      this.addPostSubscription = this.postService
        .createPost(this.editionId, this.model)
        .subscribe(
          (newPost: ApiResponse<PostModel>) => {
            this.contentAdded.emit(newPost.data!);
            this.isLoading = false;
            this.myForm.reset();
            //this.selectedFile = null;
            this.fileInputElement.nativeElement.value = '';
          },
          (error) => {
            this.isLoading = false;
          },
        );
    } else {
      this.alertService.show('Can not send empty post', 'error');
    }
  }

  /*
  processImage(imageInput: any) {
    if (this.selectedFile) {
      this.deleteImage();
    }

    this.imageLoading.set(true);
    const file: File = imageInput.files[0];
    const reader = new FileReader();

    reader.addEventListener('load', (event: any) => {
      this.imageSubscription.add(
        this.imageService
          .uploadImage(file, TypeMedia.IMAGE)
          .pipe(
            tap(
              (res: string) => {
                this.imageSubscription.add(
                  this.imageService.downloadImage(res).subscribe((blob) => {
                    const imageUrl = URL.createObjectURL(blob);
                    this.selectedFile = new ImageSnippet(imageUrl, file, res);
                    this.imageLoading.set(false);
                  }),
                );
              },
              (error) => {
                this.imageLoading.set(false);
              },
            ),
          )
          .subscribe(),
      );
    });

    reader.readAsDataURL(file);
  }
  */

  /*
  deleteImage() {
    if (!this.selectedFile) return;

    this.deleteImageLoading.set(true);
    this.imageSubscription.add(
      this.imageService.deleteImage(this.selectedFile.guid).subscribe(() => {
        this.selectedFile = null;
        this.fileInputElement.nativeElement.value = '';
        this.deleteImageLoading.set(false);
      }),
    );
  }
  */

  ngOnInit(): void {
    this.myForm = this.formBuilder.group({
      content: ['', [Validators.required, Validators.minLength(1)]],
    });
  }

  ngOnDestroy(): void {
    this.addPostSubscription?.unsubscribe();
    this.imageSubscription.unsubscribe();
  }
}
