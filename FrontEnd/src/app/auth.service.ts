import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  public currentUserSubject: BehaviorSubject<any>;
  //public currentUser: Observable<any>;
  private apiUrl = 'https://661e3eb698427bbbef03e8d6.mockapi.io/api/v1/svendeproeve/users';

  constructor(private http: HttpClient, private router: Router) {
    const storedUser = localStorage.getItem('currentUser');
    console.log('AuthService Constructor - Stored User:', storedUser);
    this.currentUserSubject = new BehaviorSubject<any>(storedUser ? JSON.parse(storedUser) : null);
  }
  
  login(username: string, password: string): Observable<any> {
    return this.http.get<any[]>(`${this.apiUrl}?search=${username}`)
      .pipe(map(users => {
        const user = users.find(u => u.username === username && u.password === password);
        if (user) {
          localStorage.setItem('currentUser', JSON.stringify(user));
          console.log('LocalStorage after login:', localStorage.getItem('currentUser'));
          this.currentUserSubject.next(user);
          return user;
        } else {
          throw new Error('Invalid credentials');
        }
      }));
  }
  
  logout() {
    console.log('AuthService logout method called');
    localStorage.removeItem('currentUser');
    console.log('LocalStorage after logout:', localStorage.getItem('currentUser'));
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }
  
  
  getCurrentUserValue(): any {
    const currentUser = this.currentUserSubject.value;
    console.log('Current User from BehaviorSubject:', currentUser);
    return currentUser;
  }
}  