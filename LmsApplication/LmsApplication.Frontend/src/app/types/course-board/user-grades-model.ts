import { UserModel } from "../users/user-model";
import { GradesTableRowModel } from "./grades-table-row-model";

export interface UserGradesModel {
  row: GradesTableRowModel;
  values: UserGradesTableRowValueModel[];
}

export interface UserGradesTableRowValueModel {
  id: string | null;
  teacherComment: string | null;
  teacher: UserModel | null;
  student: UserModel;
  value: any | null;
}
