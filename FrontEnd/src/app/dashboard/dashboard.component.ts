import { Component, OnInit } from '@angular/core';
import { AuthService } from '../app.routes';
import { Router } from '@angular/router';
import * as mockData from '../../assets/mock-data.json';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
})
export class DashboardComponent implements OnInit {
  role: string | null = null;
  data: any[] = (mockData as any).default;
  uniqueStudios: string[] = [];
  uniqueJobTitles: string[] = [];
  uniqueProjects: string[] = [];
  uniqueExpertise: string[] = [];
  filteredData: any[] = this.data;
  selectedFilters: any = {
    Studio: new Set<string>(),
    JobTitle: new Set<string>(),
    Projects: new Set<string>(),
    Expertise: new Set<string>(),
  };

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit() {
    const currentUser = this.authService.getCurrentUserValue();
    if (currentUser && currentUser.role) {
      this.role = currentUser.role;
    }

    // Remove duplicates
    this.uniqueStudios = [...new Set(this.data.map((person) => person.Studio))];
    this.uniqueJobTitles = [
      ...new Set(this.data.map((person) => person.JobTitle)),
    ];
    this.uniqueProjects = [
      ...new Set(this.data.flatMap((person) => person.Projects)),
    ];
    this.uniqueExpertise = [
      ...new Set(this.data.map((person) => person.Expertise)),
    ];
  }

  logout() {
    console.log('Logout clicked');
    this.authService.logout();
  }

  editContent() {
    console.log('Navigating to edit content page');
    this.router.navigate(['/edit-content']); // Use Router to navigate
  }

  filterBy(property: string, value: string) {
    if (this.selectedFilters[property].has(value)) {
      this.selectedFilters[property].delete(value);
    } else {
      this.selectedFilters[property].add(value);
    }
    this.applyFilters();
  }

  applyFilters() {
    this.filteredData = this.data.filter((person) => {
      const studioMatch =
        !this.selectedFilters.Studio.size ||
        this.selectedFilters.Studio.has(person.Studio);
      const jobTitleMatch =
        !this.selectedFilters.JobTitle.size ||
        this.selectedFilters.JobTitle.has(person.JobTitle);
      const projectMatch =
        !this.selectedFilters.Projects.size ||
        person.Projects.some((project: string) =>
          this.selectedFilters.Projects.has(project)
        );
      const expertiseMatch =
        !this.selectedFilters.Expertise.size ||
        this.selectedFilters.Expertise.has(person.Expertise);
      return studioMatch && jobTitleMatch && projectMatch && expertiseMatch;
    });
  }
}
