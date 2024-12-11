import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { ResourceMetadataModel } from '../types/resources/resource-metadata-model';
import { ApiResponse } from '../types/api-response';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class CourseResourceService {
  constructor(
    private http: HttpClient,
    private authService: AuthService,
  ) {}

  public getResourceMetadatas(
    resourceType: 'course' | 'edition',
    parentId: string,
  ) {
    return this.http.get<ApiResponse<ResourceMetadataModel[]>>(
      `${environment.apiUrl}/api/resources/metadatas/${resourceType}s/${parentId}`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public downloadResource(resourceId: string): any {
    return this.http.get(`${environment.apiUrl}/api/resources/${resourceId}`, {
      responseType: 'blob' as 'json',
      headers: {
        Authorization: `Bearer ${this.authService.authState().accessToken}`,
      },
    });
  }

  public deleteResource(resourceId: string) {
    return this.http.delete<ApiResponse<null>>(
      `${environment.apiUrl}/api/resources/${resourceId}`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public uploadResource(
    resourceType: 'course' | 'edition',
    parentId: string,
    name: string,
    file: File,
  ) {
    const formData = new FormData();
    formData.append('fileDisplayName', name);
    formData.append('file', file);
    formData.append('parentId', parentId);
    formData.append('type', resourceType);
    return this.http.post<ApiResponse<ResourceMetadataModel>>(
      `${environment.apiUrl}/api/resources/upload`,
      formData,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }
}
