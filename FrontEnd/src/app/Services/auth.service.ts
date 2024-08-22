import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private currentUser: any = null;
  private readonly apiUrl = 'https://localhost:7208/api/User/LoginUser';

  constructor(private http: HttpClient) {}

  /**
   * Login method that sends a POST request to the API.
   * @param email User email
   * @param password User password
   * @returns Observable<boolean> indicating success or failure
   */
  login(email: string, password: string): Observable<boolean> {
    return this.http.post<any>(this.apiUrl, { email, password }).pipe(
      tap((response) => {
        if (response && response.role) {
          this.currentUser = { ...response }; // Store user info including role
          localStorage.setItem('userRole', this.currentUser.role);
          localStorage.setItem('isLoggedIn', 'true');
        }
      }),
      map(() => !!this.currentUser) // Return true if login was successful
    );
  }

  /**
   * Logout the current user.
   */
  logout() {
    this.currentUser = null;
    localStorage.removeItem('userRole');
    localStorage.removeItem('isLoggedIn');
  }

  /**
   * Get the current logged-in user.
   * @returns The current user object or null
   */
  getCurrentUserValue() {
    return this.currentUser;
  }

  /**
   * Get the role of the current user.
   * @returns The role of the user or null
   */
  getUserRole(): string | null {
    return this.currentUser
      ? this.currentUser.role
      : localStorage.getItem('userRole');
  }
  updateCurrentUserProjects(projects: string[]) {
    if (this.currentUser) {
      this.currentUser.Projects = projects;
    }
  }

  /**
   * Update the user's expertise.
   * @param expertise Updated list of expertise
   */
  updateCurrentUserExpertise(expertise: string[]) {
    if (this.currentUser) {
      this.currentUser.Expertise = expertise;
    }
  }
}
