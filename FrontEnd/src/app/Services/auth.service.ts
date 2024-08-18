import { Injectable } from '@angular/core';
import * as mockUsers from '../../assets/mock-data.json';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private currentUser: any = null;

  constructor() {}

  login(email: string, password: string): boolean {
    const user = (mockUsers as any).default.find(
      (u: any) =>
        u.email.toLowerCase() === email.toLowerCase() && u.password === password
    );

    if (user) {
      this.currentUser = { ...user };
      return true;
    }
    return false;
  }

  logout() {
    this.currentUser = null;
  }

  getCurrentUserValue() {
    return this.currentUser;
  }

  getUserRole(): string | null {
    return this.currentUser ? this.currentUser.role : null;
  }

  updateCurrentUserProjects(projects: string[]) {
    if (this.currentUser) {
      this.currentUser.Projects = projects;
    }
  }

  updateCurrentUserExpertise(expertise: string[]) {
    if (this.currentUser) {
      this.currentUser.Expertise = expertise;
    }
  }
}
