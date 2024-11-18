import { UserModel } from '../users/user-model';

export interface ResourceMetadataModel {
  id: string;
  fileDisplayName: string;
  fileSize: number;
  fileExtension: string;
  type: 'course' | 'edition';
  parentId: string;
  createdAtUtc: Date;
  user: UserModel;
}
