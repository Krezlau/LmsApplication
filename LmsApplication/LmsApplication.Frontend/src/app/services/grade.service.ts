import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { env } from '../../env';
import { ApiResponse } from '../types/api-response';
import { GradesTableRowModel } from '../types/course-board/grades-table-row-model';

@Injectable({
  providedIn: 'root',
})
export class GradeService {
  constructor(
    private http: HttpClient,
    private authService: AuthService,
  ) {}

  public getRowDefinitions(courseEditionId: string) {
    return this.http.get<ApiResponse<GradesTableRowModel[]>>(
      `${env.apiUrl}/api/editions/${courseEditionId}/grades/rows`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public createRowDefinition(
    courseEditionId: string,
    title: string,
    description: string | null,
    date: Date | null,
    rowType: number,
    isSummed: boolean,
  ) {
    return this.http.post<ApiResponse<GradesTableRowModel>>(
      `${env.apiUrl}/api/editions/${courseEditionId}/grades/rows`,
      {
        courseEditionId,
        title,
        description,
        date,
        rowType: +rowType,
        isSummed,
      },
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public deleteRowDefinition(courseEditionId: string, rowId: string) {
    return this.http.delete<ApiResponse<null>>(
      `${env.apiUrl}/api/editions/${courseEditionId}/grades/rows/${rowId}`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }
}
