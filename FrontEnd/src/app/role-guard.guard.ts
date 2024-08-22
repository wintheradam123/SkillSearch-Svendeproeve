import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from './Services/auth.service';

@Injectable({
  providedIn: 'root',
})
export class RoleGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): boolean {
    const currentUser = this.authService.getCurrentUserValue();

    console.log('RoleGuard - Current User:', currentUser); // Debugging log

    if (currentUser) {
      // Normalize the role to lowercase
      const normalizedRole = currentUser.role.toLowerCase();

      // User is logged in
      if (normalizedRole === 'admin' || normalizedRole === 'user') {
        // User has a valid role
        return true;
      } else {
        // User role is invalid, redirect to login
        console.log('Redirecting to login, invalid role:', currentUser.role);
        this.router.navigate(['/login'], {
          queryParams: { error: 'notAuthorized' },
        });
        return false;
      }
    } else {
      // User is not logged in, redirect to login
      console.log('Redirecting to login, user not logged in');
      this.router.navigate(['/login'], {
        queryParams: { error: 'notLoggedIn' },
      });
      return false;
    }
  }
}
