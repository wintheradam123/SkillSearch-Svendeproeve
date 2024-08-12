import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, BehaviorSubject } from 'rxjs';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  public currentUserSubject = new BehaviorSubject<any>(null);
  public currentUser = this.currentUserSubject.asObservable();
  private apiUrl = 'https://661e3eb698427bbbef03e8d6.mockapi.io/api/v1/svendeproeve/users'; 

  constructor(private http: HttpClient, private router: Router) {}
 
  login(username: string, password: string): Observable<any> {
    return this.http.get<any[]>(`${this.apiUrl}?search=${username}`)
      .pipe(map(users => {
        const user = users.find(u => u.username === username && u.password === password);
        
        if (user) {
          //console.log("User found and authenticated", user)
          this.currentUserSubject.next(user);
          localStorage.setItem('currentUser', JSON.stringify(user));
          return user;
        } else {
          console.log("No user found or invalid credentials");
          throw new Error('Invalid credentials');
        }
      }));
  }

  logout() {
    localStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  getCurrentUserValue(): any {
    return this.currentUserSubject.value;
  }

}