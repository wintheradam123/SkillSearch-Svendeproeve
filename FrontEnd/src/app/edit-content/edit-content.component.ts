import { Component, OnInit } from '@angular/core';
import { AuthService } from '../Services/auth.service';
import { DataService } from '../Services/data-service.service';

@Component({
  selector: 'app-edit-content',
  templateUrl: './edit-content.component.html',
  styleUrls: ['./edit-content.component.css'],
})
export class EditContentComponent implements OnInit {
  role: string = '';
  projects: string[] = [];
  expertise: string[] = [];
  selectedProjects: string[] = [];
  selectedExpertise: string[] = [];

  constructor(
    private authService: AuthService,
    private dataService: DataService
  ) {}

  ngOnInit() {
    const currentUser = this.authService.getCurrentUserValue();
    if (currentUser) {
      this.role = currentUser.role;
      this.selectedProjects = currentUser.Projects || [];
      this.selectedExpertise = currentUser.Expertise || [];
    }

    this.projects = this.dataService.getProjects();
    this.expertise = this.dataService.getExpertise();
  }

  addProject(project: string) {
    if (this.role === 'Admin') {
      this.dataService.addProject(project);
      this.projects = this.dataService.getProjects(); // Update the list
    }
  }

  addExpertise(expertiseItem: string) {
    if (this.role === 'Admin') {
      this.dataService.addExpertise(expertiseItem);
      this.expertise = this.dataService.getExpertise(); // Update the list
    }
  }

  toggleProjectSelection(project: string) {
    if (this.selectedProjects.includes(project)) {
      this.selectedProjects = this.selectedProjects.filter(
        (p) => p !== project
      );
    } else {
      this.selectedProjects.push(project);
    }
    this.authService.updateCurrentUserProjects(this.selectedProjects);
  }

  toggleExpertiseSelection(expertiseItem: string) {
    if (this.selectedExpertise.includes(expertiseItem)) {
      this.selectedExpertise = this.selectedExpertise.filter(
        (e) => e !== expertiseItem
      );
    } else {
      this.selectedExpertise.push(expertiseItem);
    }
    this.authService.updateCurrentUserExpertise(this.selectedExpertise);
  }
}
