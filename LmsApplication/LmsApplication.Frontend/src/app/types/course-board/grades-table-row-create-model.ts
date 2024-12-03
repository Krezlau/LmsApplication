import { RowType } from './grades-table-row-model';

export interface GradesTableRowCreateModel {
  courseEditionId: string;
  title: string;
  description: string | null;
  date: Date | null;
  rowType: RowType;
  isSummed: boolean;
}
