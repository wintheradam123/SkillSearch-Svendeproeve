import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class DataService {
  private projects: string[] = [];
  private expertise: string[] = [];

  constructor() {}

  getProjects(): string[] {
    return this.projects;
  }

  getExpertise(): string[] {
    return this.expertise;
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
