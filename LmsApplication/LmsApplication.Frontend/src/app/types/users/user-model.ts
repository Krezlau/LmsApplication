import {UserRole} from "./user-role";

export interface UserModel {
  email: string;
  name: string;
  surname: string;
  photo: string | null;
  userId: string;
  role: UserRole;
}

