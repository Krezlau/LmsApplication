import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { CourseResourceService } from '../../services/course-resource.service';
import { AlertService } from '../../services/alert.service';
import { AuthService } from '../../services/auth.service';
import { Subscription, tap } from 'rxjs';
import { ApiResponse } from '../../types/api-response';
import { ResourceMetadataModel } from '../../types/resources/resource-metadata-model';
import { NgClass, NgFor, NgIf } from '@angular/common';
import { DateFormatting } from '../../helpers/date-formatter';

@Component({
  selector: 'app-course-resources-list',
  standalone: true,
  imports: [NgClass, NgIf, NgFor],
  templateUrl: './course-resources-list.component.html',
})
export class CourseResourcesListComponent implements OnInit, OnDestroy {
  @Input() id: string = '';
  @Input() type: 'course' | 'edition' = 'course';

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

  deleteResource(resource: ResourceMetadataModel) {
    if (!this.canUserDeleteResource()) {
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

  canUserDeleteResource() {
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
        .getResourceMetadatas(this.type, this.id)
        .pipe(
          tap({
            next: (res: ApiResponse<ResourceMetadataModel[]>) => {
              this.resources = res.data!;
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
