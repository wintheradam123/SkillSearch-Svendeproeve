import { AuthService } from '../Services/auth.service';
import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent implements OnInit {
  username: string = '';
  password: string = '';
  errorMessage: string = '';

  constructor(
    public authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    // Subscribe to query parameters to check for errors passed in the URL
    this.route.queryParams.subscribe((params) => {
      if (params['error'] === 'notLoggedIn') {
        this.errorMessage = 'You need to be logged in to proceed';
      }
    });
  }

  login() {
    const loginSuccessful = this.authService.login(
      this.username,
      this.password
    );

    if (loginSuccessful) {
      const userRole = this.authService.getUserRole();
      console.log(`Login successful as ${userRole}, navigating to dashboard`);
      this.router.navigate(['/dashboard']); // Navigate to dashboard after login
    } else {
      console.error('Login failed');
      this.errorMessage = 'Invalid username or password';
    }
  }
}
