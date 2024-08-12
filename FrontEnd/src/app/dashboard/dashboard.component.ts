import { Component, OnInit } from '@angular/core';
import { AuthService } from '../app.routes';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  role: string | null = null;

  constructor(private authService: AuthService) {}

  ngOnInit() {
    const currentUser = this.authService.getCurrentUserValue();
    if (currentUser && currentUser.role) {
      this.role = currentUser.role;
    }
  }

  editContent() {
    console.log('Editing content...');
    // Implement the actual content editing logic here
  }

  logout() {
    console.log('Logout clicked');
    this.authService.logout();

  
  }
}
