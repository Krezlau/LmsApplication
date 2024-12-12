import {
  Component,
  ElementRef,
  Input,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import { CourseResourceService } from '../../services/course-resource.service';
import { AlertService } from '../../services/alert.service';
import { AuthService } from '../../services/auth.service';
import { Subscription, tap } from 'rxjs';
import { ApiResponse } from '../../types/api-response';
import { ResourceMetadataModel } from '../../types/resources/resource-metadata-model';
import { NgClass, NgFor, NgIf } from '@angular/common';
import { DateFormatting } from '../../helpers/date-formatter';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { CollectionResource } from '../../types/collection-resource';

@Component({
  selector: 'app-course-resources-list',
  standalone: true,
  imports: [NgClass, NgIf, NgFor, ReactiveFormsModule],
  templateUrl: './course-resources-list.component.html',
})
export class CourseResourcesListComponent implements OnInit, OnDestroy {
  @Input() id: string = '';
  @Input() type: 'course' | 'edition' = 'course';

  @ViewChild('modal') modal: ElementRef | undefined;
  @ViewChild('fileInput') fileInput: ElementRef | undefined;

  dataFormatter = DateFormatting;

  constructor(
    private resourceService: CourseResourceService,
    private alertService: AlertService,
    private authService: AuthService,
  ) {}

  sub = new Subscription();
  authState = this.authService.authState;
  resources: ResourceMetadataModel[] = [];
  resourcesLoading = false;
  uploadLoading = false;

  page = 0;
  pageSize = 10;
  nextPage = true;

  nameControl = new FormControl('', [
    Validators.required,
    Validators.pattern(/^[a-zA-Z0-9_-]*$/),
  ]);
  file: File | null = null;

  openModal() {
    this.modal!.nativeElement.showModal();
  }

  onFilePicked(event: Event) {
    const files = (event.target as HTMLInputElement).files;
    if (files && files.length > 0) {
      this.file = files[0];
    }
  }

  downloadResource(resource: ResourceMetadataModel) {
    this.sub.add(
      this.resourceService
        .downloadResource(resource.id)
        .subscribe((res: any) => {
          var downloadURL = window.URL.createObjectURL(res);
          var link = document.createElement('a');
          link.href = downloadURL;
          link.download = resource.fileDisplayName + resource.fileExtension;
          link.click();
        }),
    );
  }

  uploadResource(event: Event) {
    event.preventDefault();

    if (this.nameControl.invalid || !this.nameControl.value) {
      this.alertService.show(
        "Name must be composed of alphanumeric characters and/or '-' '_'. ",
        'error',
      );
      return;
    }

    if (this.file === null) {
      this.alertService.show('Please select a file', 'error');
      return;
    }

    this.uploadLoading = true;
    this.sub.add(
      this.resourceService
        .uploadResource(this.type, this.id, this.nameControl.value, this.file)
        .pipe(
          tap({
            next: (response: ApiResponse<ResourceMetadataModel>) => {
              this.uploadLoading = false;
              this.resources.push(response.data!);
              this.alertService.show(
                'Resource uploaded successfully',
                'success',
              );
              this.nameControl.reset();
              this.file = null;
              this.fileInput!.nativeElement.value = null;
              this.modal!.nativeElement.close();
            },
            error: (err: any) => {
              this.uploadLoading = false;
              if (err.error.message) {
                this.alertService.show(err.error.message, 'error');
              } else {
                this.alertService.show('An error occurred', 'error');
              }
            },
          }),
        )
        .subscribe(),
    );
  }

  deleteResource(resource: ResourceMetadataModel) {
    if (!this.canUserManageResources()) {
      return;
    }

    this.sub.add(
      this.resourceService
        .deleteResource(resource.id)
        .pipe(
          tap({
            next: (_) => {
              this.resources = this.resources.filter(
                (r) => r.id !== resource.id,
              );
              this.alertService.show(
                'Resource deleted successfully',
                'success',
              );
            },
            error: (err) => {
              if (err.error.message) {
                this.alertService.show(err.error.message, 'error');
              } else {
                this.alertService.show('An error occurred', 'error');
              }
            },
          }),
        )
        .subscribe(),
    );
  }

  canUserManageResources() {
    if (this.type === 'course') {
      return this.authState().userData!.role === 2;
    }
    if (this.type === 'edition') {
      return this.authState().userData!.role >= 1;
    }

    return false;
  }

  ngOnInit(): void {
    this.resourcesLoading = true;
    this.sub.add(
      this.resourceService
        .getResourceMetadatas(this.type, this.id, this.page + 1, this.pageSize)
        .pipe(
          tap({
            next: (
              res: ApiResponse<CollectionResource<ResourceMetadataModel>>,
            ) => {
              this.resources = [...this.resources, ...res.data!.items];
              this.page++;
              this.nextPage = res.data?.totalCount! > this.page * this.pageSize;
              this.resourcesLoading = false;
            },
            error: (err) => {
              this.resourcesLoading = false;
              if (err.error.message) {
                this.alertService.show(err.error.message, 'error');
              } else {
                this.alertService.show('An error occurred', 'error');
              }
            },
          }),
        )
        .subscribe(),
    );
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
