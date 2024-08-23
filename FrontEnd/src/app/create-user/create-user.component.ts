import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create-user',
  templateUrl: './create-user.component.html',
  styleUrls: ['./create-user.component.css'],
})
export class CreateUserComponent {
  email: string = '';
  password: string = '';
  errorMessage: string = '';
  successMessage: string = '';

  constructor(private http: HttpClient, private router: Router) {}

  goBack() {
    this.router.navigate(['/login']);
  }

  createUser() {
    const payload = {
      email: this.email,
      password: this.password,
    };

    this.http
      .post('https://localhost:7208/api/User/CreatePassword', payload)
      .subscribe(
        (response: any) => {
          console.log('User created successfully');
          this.successMessage = 'User created successfully!';
          this.errorMessage = '';
          // Optionally, navigate to the login page after successful creation
          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 2000); // Redirect after 2 seconds
        },
        (error) => {
          console.error('Error creating user', error);
          this.errorMessage = 'Failed to create user. Please try again.';
          this.successMessage = '';
        }
      );
  }
}
