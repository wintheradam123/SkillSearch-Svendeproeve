import { Component, OnInit } from '@angular/core';
import { AuthService } from '../Services/auth.service';
import { HttpClient } from '@angular/common/http';
import algoliasearch from 'algoliasearch/lite';

@Component({
  selector: 'app-edit-content',
  templateUrl: './edit-content.component.html',
  styleUrls: ['./edit-content.component.css'],
})
export class EditContentComponent implements OnInit {
  role: string = '';
  skills: any[] = [];
  projects: any[] = [];
  selectedSkills: Set<string> = new Set();
  selectedProjects: Set<string> = new Set();
  userId: number | null = null;

  private readonly skillsApiUrl = 'https://localhost:7208/api/Skill';
  private readonly projectsApiUrl = 'https://localhost:7208/api/Project';

  // Algolia credentials
  algoliaAppId: string = 'UW17LRXEG9';
  algoliaApiKey: string = '21a8d95e001480809f816fdebcdeee77';
  algoliaIndexName: string = 'dev_SkillSearch_Users';
  private algoliaClient: any;
  private algoliaIndex: any;

  constructor(private authService: AuthService, private http: HttpClient) {
    this.algoliaClient = algoliasearch(this.algoliaAppId, this.algoliaApiKey);
    this.algoliaIndex = this.algoliaClient.initIndex(this.algoliaIndexName);
  }

  ngOnInit() {
    const currentUser = this.authService.getCurrentUserValue();
    if (currentUser) {
      this.role = currentUser.role;
      this.userId = currentUser.id;
    }

    // Fetch user data from Algolia
    this.fetchUserData();
  }

  fetchUserData() {
    const currentUserEmail =
      this.authService.getCurrentUserValue()?.userPrincipalName;
    if (!currentUserEmail) {
      console.error('User email not found');
      return;
    }

    // Fetch the user's skills and projects from Algolia
    this.algoliaIndex
      .search(currentUserEmail)
      .then((response: any) => {
        const user = response.hits[0]; // Assuming the user is the first result

        this.selectedSkills = new Set(
          user.skills ? user.skills.map((skill: any) => skill.tag) : []
        );

        this.selectedProjects = new Set(
          user.solutions
            ? user.solutions.map((solution: any) => solution.solutionName)
            : []
        );

        // Now fetch skills and projects and compare
        this.loadSkills();
        this.loadProjects();
      })
      .catch((error: any) => {
        console.error('Error fetching user data from Algolia:', error);
      });
  }

  loadSkills() {
    this.http.get<any[]>(this.skillsApiUrl).subscribe(
      (response) => {
        this.skills = response.map((skill) => ({
          ...skill,
          isSelected: this.selectedSkills.has(skill.title),
        }));
        console.log('Skills loaded:', this.skills);
      },
      (error) => {
        console.error('Error fetching skills:', error);
      }
    );
  }

  loadProjects() {
    this.http.get<any[]>(this.projectsApiUrl).subscribe(
      (response) => {
        this.projects = response.map((project) => ({
          ...project,
          isSelected: this.selectedProjects.has(project.title),
        }));
        console.log('Projects loaded:', this.projects);
      },
      (error) => {
        console.error('Error fetching projects:', error);
      }
    );
  }

  toggleSkillSelection(skill: any) {
    if (skill.isSelected) {
      this.selectedSkills.delete(skill.title);
    } else {
      this.selectedSkills.add(skill.title);
    }
    skill.isSelected = !skill.isSelected;
    this.saveSkillSubscription(
      skill.id,
      skill.isSelected ? 'subscribe' : 'unsubscribe'
    );
  }

  toggleProjectSelection(project: any) {
    if (project.isSelected) {
      this.selectedProjects.delete(project.title);
    } else {
      this.selectedProjects.add(project.title);
    }
    project.isSelected = !project.isSelected;
    this.saveProjectSubscription(
      project.id,
      project.isSelected ? 'subscribe' : 'unsubscribe'
    );
  }

  saveSkillSubscription(skillId: number, action: 'subscribe' | 'unsubscribe') {
    const url = `${this.skillsApiUrl}/Subscribe/${skillId}?subscribe=${action}`;
    if (this.userId !== null) {
      this.http.put(url, { Id: skillId, UserId: this.userId }).subscribe(
        (response: any) => {
          console.log(`Skill ${action}d successfully:`, response);
        },
        (error) => {
          console.error(`Error during skill ${action} operation:`, error);
        }
      );
    }
  }

  saveProjectSubscription(
    projectId: number,
    action: 'subscribe' | 'unsubscribe'
  ) {
    const url = `${this.projectsApiUrl}/Subscribe/${projectId}?subscribe=${action}`;
    if (this.userId !== null) {
      this.http.put(url, { Id: projectId, UserId: this.userId }).subscribe(
        (response: any) => {
          console.log(`Project ${action}d successfully:`, response);
        },
        (error) => {
          console.error(`Error during project ${action} operation:`, error);
        }
      );
    }
  }

  addSkill(skillTitle: string) {
    if (this.role === 'Admin') {
      const newSkill = {
        id: 0, // ID will be auto-incremented by the backend
        title: skillTitle.trim(), // Ensure that the title is not empty or just whitespace
        category: 'string', // Placeholder value for category, adjust as needed
        users: [], // Initially an empty array
      };

      this.http.post(this.skillsApiUrl, newSkill).subscribe(
        (response: any) => {
          console.log('Skill added successfully:', response);
          this.skills.push(response);
        },
        (error) => {
          console.error('Error adding skill:', error);
          if (error.status === 400) {
            console.error(
              'Bad Request - Check if all required fields are correct.'
            );
          }
        }
      );
    }
  }

  addProject(projectTitle: string) {
    if (this.role === 'Admin') {
      const newProject = {
        id: 0, // ID will be auto-incremented by the backend
        title: projectTitle.trim(), // Ensure that the title is not empty or just whitespace
        category: 'string', // Placeholder value for category, adjust as needed
        users: [], // Initially an empty array
      };

      this.http.post(this.projectsApiUrl, newProject).subscribe(
        (response: any) => {
          console.log('Project added successfully:', response);
          this.projects.push(response);
        },
        (error) => {
          console.error('Error adding project:', error);
          if (error.status === 400) {
            console.error(
              'Bad Request - Check if all required fields are correct.'
            );
          }
        }
      );
    }
  }
}
