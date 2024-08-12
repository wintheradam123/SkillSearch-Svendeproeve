import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AuthService } from './app.routes';

@Injectable({
  providedIn: 'root'})
export class RoleGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    const currentUser = this.authService.getCurrentUserValue();
    if (!currentUser) {
      console.log("Redirecting to login, user not logged in");
      this.router.navigate(['/login'], { queryParams: { error: 'notLoggedIn' }});
      return false;
    }

    const userRole = currentUser.role.toLowerCase();
    console.log("Checking access with RoleGuard", currentUser);

    if (userRole === 'admin' || userRole === 'user') {
      return true;  // User is either an Admin or a User and can access the route
    } else {
      console.log("Redirecting to login, insufficient permissions");
      this.router.navigate(['/login'], { queryParams: { error: 'unauthorized' }});
      return false;
    }
  }
}