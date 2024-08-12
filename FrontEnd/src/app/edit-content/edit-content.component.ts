import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth.service'; // Make sure this is correctly imported

@Component({
  selector: 'app-edit-content',
  templateUrl: './edit-content.component.html',
  styleUrls: ['./edit-content.component.css'],
})
export class EditContentComponent implements OnInit {
  role: string = '';
  projects: string[] = [];
  expertise: string[] = [];

  constructor(private authService: AuthService) {}

  ngOnInit() {
    const currentUser = this.authService.getCurrentUserValue();
    if (currentUser && currentUser.role) {
      this.role = currentUser.role;
      console.log('Current user role:', this.role); // Verify role output
    } else {
      console.log('No current user or role not set:', currentUser);
    }
  }

  addProject(project: string) {
    if (project && !this.projects.includes(project)) {
      this.projects.push(project);
    }
  }

  addExpertise(expertiseItem: string) {
    if (expertiseItem && !this.expertise.includes(expertiseItem)) {
      this.expertise.push(expertiseItem);
    }
  }
}
