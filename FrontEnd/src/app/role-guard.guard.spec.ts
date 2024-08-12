import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router'; // Import ActivatedRouteSnapshot and RouterStateSnapshot
import { RoleGuard } from './role-guard.guard';
import { AuthService } from './app.routes';
import { of } from 'rxjs';

describe('RoleGuard', () => {
  let guard: RoleGuard;
  let authService: jasmine.SpyObj<AuthService>;
  let router: Router;

  beforeEach(() => {
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['getCurrentUserValue']);
    TestBed.configureTestingModule({
      imports: [RouterTestingModule],
      providers: [
        RoleGuard,
        { provide: AuthService, useValue: authServiceSpy }
      ]
    });
    guard = TestBed.inject(RoleGuard);
    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    router = TestBed.inject(Router);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });

  it('should allow access if user is logged in', () => {
    authService.getCurrentUserValue.and.returnValue({ role: 'user' }); // Mock logged-in user
    const canActivate = guard.canActivate({} as ActivatedRouteSnapshot, {} as RouterStateSnapshot); // Provide empty objects as arguments
    expect(canActivate).toBeTrue();
  });

  it('should redirect to login if user is not logged in', () => {
    authService.getCurrentUserValue.and.returnValue(null); // Mock no logged-in user
    spyOn(router, 'navigate');
    guard.canActivate({} as ActivatedRouteSnapshot, {} as RouterStateSnapshot); // Provide empty objects as arguments
    expect(router.navigate).toHaveBeenCalledWith(['/login'], { queryParams: { error: 'notLoggedIn' } });
  });

  // Add more tests for different scenarios, such as testing different roles and unauthorized access
});
