export interface GradesTableRowModel {
  id: string;
  courseEditionId: string;
  title: string;
  description: string | null;
  date: Date | null;
  rowType: RowType;
  isSummed: boolean;
}

export enum RowType {
  None = 0,
  Text = 1,
  Number = 2,
  Bool = 3,
}
