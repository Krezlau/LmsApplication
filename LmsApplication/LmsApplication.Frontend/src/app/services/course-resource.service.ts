import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { ResourceMetadataModel } from '../types/resources/resource-metadata-model';
import { env } from '../../env';
import { ApiResponse } from '../types/api-response';

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
      `${env.apiUrl}/api/resources/metadatas/${resourceType}s/${parentId}`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public downloadResource(resourceId: string): any {
    return this.http.get(`${env.apiUrl}/api/resources/${resourceId}`, {
      responseType: 'blob' as 'json',
      headers: {
        Authorization: `Bearer ${this.authService.authState().accessToken}`,
      },
    });
  }

  public deleteResource(resourceId: string) {
    return this.http.delete<ApiResponse<null>>(
      `${env.apiUrl}/api/resources/${resourceId}`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }
}
