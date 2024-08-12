import { AuthService } from '../app.routes';
import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  username: string = '';
  password: string = '';
  errorMessage: string = '';

  constructor(
    public authService: AuthService,
    private router: Router,
    private route: ActivatedRoute  // Inject ActivatedRoute to access route parameters
  ) {}

  ngOnInit() {
    // Subscribe to query parameters to check for errors passed in the URL
    this.route.queryParams.subscribe(params => {
      if (params['error'] === 'notLoggedIn') {
        this.errorMessage = "You need to be logged in to proceed";
      }
    });
  }

  login() {

    this.authService.login(this.username.toLowerCase(), this.password.toLowerCase()).subscribe({
    
      next: (user) => {
        console.log("Login successful, navigating to dashboard", user);
        this.router.navigate(['/dashboard']);  // Navigate to dashboard after login
      },
      error: (error) => {
        console.error("Login failed", error);
        this.errorMessage = 'Invalid username or password';
      }
    });
  }
}
