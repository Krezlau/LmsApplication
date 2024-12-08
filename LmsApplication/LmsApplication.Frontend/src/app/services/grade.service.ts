import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { env } from '../../env';
import { ApiResponse } from '../types/api-response';
import { GradesTableRowModel } from '../types/course-board/grades-table-row-model';
import {
  GradesTableRowValueModel,
  UserGradesModel,
  UserGradesTableRowValueModel,
} from '../types/course-board/user-grades-model';
import { GradeModel } from '../types/course-board/grade-model';

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

  public editRowDefinition(
    courseEditionId: string,
    rowId: string,
    title: string,
    description: string | null,
    date: Date | null,
    isSummed: boolean,
  ) {
    return this.http.put<ApiResponse<GradesTableRowModel>>(
      `${env.apiUrl}/api/editions/${courseEditionId}/grades/rows/${rowId}`,
      {
        title,
        description,
        date,
        isSummed,
      },
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public getUserGrades(courseEditionId: string, rowId: string) {
    return this.http.get<ApiResponse<UserGradesModel>>(
      `${env.apiUrl}/api/editions/${courseEditionId}/grades/row/${rowId}`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public updateGrade(
    courseEditionId: string,
    rowId: string,
    userId: string,
    value: string,
    teacherComment: string | null,
  ) {
    return this.http.put<ApiResponse<GradesTableRowValueModel>>(
      `${env.apiUrl}/api/editions/${courseEditionId}/grades/row/${rowId}?userId=${userId}`,
      {
        value: value?.toString() ?? null,
        teacherComment,
      },
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public deleteGrade(courseEditionId: string, rowId: string, userId: string) {
    return this.http.delete<ApiResponse<null>>(
      `${env.apiUrl}/api/editions/${courseEditionId}/grades/row/${rowId}?userId=${userId}`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public getUserGradesTable(courseEditionId: string, userId: string | null) {
    const path = userId
      ? `${env.apiUrl}/api/editions/${courseEditionId}/grades/user/${userId}`
      : `${env.apiUrl}/api/editions/${courseEditionId}/grades/current`;

    return this.http.get<ApiResponse<GradeModel[]>>(path, {
      headers: {
        Authorization: `Bearer ${this.authService.authState().accessToken}`,
      },
    });
  }
}
