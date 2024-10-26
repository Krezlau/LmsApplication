import { UserRole } from './user-role';

export interface UserModel {
  id: string;
  email: string;
  name: string;
  surname: string;
  photo: string | null;
  role: UserRole;
}
