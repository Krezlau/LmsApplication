import { UserModel } from '../users/user-model';
import { GradesTableRowModel } from './grades-table-row-model';
import { GradesTableRowValueModel } from './user-grades-model';

export interface GradeModel {
  row: GradesTableRowModel;
  value: GradesTableRowValueModel | null;
}

export interface UserGradeModel {
  grades: GradeModel[];
  finalGrade: FinalGradeModel;
}

export interface FinalGradeModel {
  id: string;
  value: number;
  teacher: UserModel;
}
