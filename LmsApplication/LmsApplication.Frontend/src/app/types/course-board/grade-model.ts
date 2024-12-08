import { GradesTableRowModel } from "./grades-table-row-model";
import { GradesTableRowValueModel } from "./user-grades-model";

export interface GradeModel {
  row: GradesTableRowModel;
  value: GradesTableRowValueModel | null;
}
