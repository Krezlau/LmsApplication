import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { ApiResponse } from '../types/api-response';
import { GradesTableRowModel } from '../types/course-board/grades-table-row-model';
import {
  GradesTableRowValueModel,
  UserGradesModel,
} from '../types/course-board/user-grades-model';
import {
  FinalGradeModel,
  UserGradeModel,
} from '../types/course-board/grade-model';
import { environment } from '../../environments/environment';

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
      `${environment.apiUrl}/api/editions/${courseEditionId}/grades/rows`,
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
      `${environment.apiUrl}/api/editions/${courseEditionId}/grades/rows`,
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
      `${environment.apiUrl}/api/editions/${courseEditionId}/grades/rows/${rowId}`,
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
      `${environment.apiUrl}/api/editions/${courseEditionId}/grades/rows/${rowId}`,
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
      `${environment.apiUrl}/api/editions/${courseEditionId}/grades/row/${rowId}`,
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
      `${environment.apiUrl}/api/editions/${courseEditionId}/grades/row/${rowId}?userId=${userId}`,
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
      `${environment.apiUrl}/api/editions/${courseEditionId}/grades/row/${rowId}?userId=${userId}`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public getUserGradesTable(courseEditionId: string, userId: string | null) {
    const path = userId
      ? `${environment.apiUrl}/api/editions/${courseEditionId}/grades/user/${userId}`
      : `${environment.apiUrl}/api/editions/${courseEditionId}/grades/current`;

    return this.http.get<ApiResponse<UserGradeModel>>(path, {
      headers: {
        Authorization: `Bearer ${this.authService.authState().accessToken}`,
      },
    });
  }

  public addFinalGrade(courseEditionId: string, userId: string, value: number) {
    return this.http.post<ApiResponse<FinalGradeModel>>(
      `${environment.apiUrl}/api/editions/${courseEditionId}/grades/final`,
      { userId, value },
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public deleteFinalGrade(courseEditionId: string, userId: string) {
    return this.http.delete<ApiResponse<null>>(
      `${environment.apiUrl}/api/editions/${courseEditionId}/grades/final?userId=${userId}`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }
}
