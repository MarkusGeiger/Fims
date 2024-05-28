
export interface Role {
  id: string;
  name: string;
}

export interface User {
  id: string;
  username: string;
  email: string;
  emailConfirmed: boolean;
  roles: string[];
}

export class EditableUser implements User {
  id: string;
  username: string;
  email: string;
  emailConfirmed: boolean;
  roles: string[];
  private user: User;

  constructor (user:User){
    this.id = user.id;
    this.username = user.username;
    this.email = user.email;
    this.emailConfirmed = user.emailConfirmed;
    this.roles = user.roles;
    this.user = user;
  }

  pendingChanges(): boolean {
    return !(this.user.id === this.id
      && this.user.username === this.username
      && this.user.email === this.email
      && this.user.emailConfirmed === this.emailConfirmed
      && this.user.roles === this.roles);
  }
}