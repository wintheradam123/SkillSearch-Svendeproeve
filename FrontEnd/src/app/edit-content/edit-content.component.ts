import { Component, OnInit } from '@angular/core';
import { AuthService } from '../Services/auth.service';
import { DataService } from '../Services/data-service.service';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-edit-content',
  templateUrl: './edit-content.component.html',
  styleUrls: ['./edit-content.component.css'],
})
export class EditContentComponent implements OnInit {
  role: string = '';
  skills: any[] = []; // Updated to store objects with skill details
  projects: any[] = []; // Updated to store objects with project details
  selectedSkills: string[] = [];
  selectedProjects: string[] = [];
  userId: number | null = null; // Store the user ID

  private readonly skillsApiUrl = 'https://localhost:7208/api/Skill';
  private readonly projectsApiUrl = 'https://localhost:7208/api/Project';

  constructor(
    private authService: AuthService,
    private dataService: DataService,
    private http: HttpClient // Inject HttpClient to make HTTP requests
  ) {}

  ngOnInit() {
    const currentUser = this.authService.getCurrentUserValue();
    if (currentUser) {
      this.role = currentUser.role;
      this.userId = currentUser.id; // Retrieve the user ID from the logged-in user
      this.selectedSkills = currentUser.Projects || []; // Now "Projects" becomes "Skills"
      this.selectedProjects = currentUser.Expertise || []; // Expertise now refers to Projects
    }

    // Fetch skills from the API and populate the skills array
    this.http.get<any[]>(this.skillsApiUrl).subscribe(
      (response) => {
        this.skills = response; // Use the API response to set the skills
        console.log('Skills loaded:', this.skills);
      },
      (error) => {
        console.error('Error fetching skills:', error);
      }
    );

    // Fetch projects from the API and populate the projects array
    this.http.get<any[]>(this.projectsApiUrl).subscribe(
      (response) => {
        this.projects = response; // Use the API response to set the projects
        console.log('Projects loaded:', this.projects);
      },
      (error) => {
        console.error('Error fetching projects:', error);
      }
    );
  }

  addSkill(skill: string) {
    if (this.role === 'Admin') {
      // Create the skill payload
      const newSkill = {
        id: 0, // ID will be auto-incremented by the backend
        title: skill.trim(), // Ensure that the title is not empty or just whitespace
        category: 'string', // Placeholder value for category, adjust as needed
        users: [], // Initially an empty array
      };

      // Log the payload for debugging
      console.log('Sending skill payload:', newSkill);

      // Send the POST request
      this.http.post(this.skillsApiUrl, newSkill).subscribe(
        (response: any) => {
          console.log('Skill added successfully:', response);
          // Add the new skill to the local list and update the UI
          this.skills.push(response);
        },
        (error) => {
          // Log the error response for debugging
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

  addProject(project: string) {
    if (this.role === 'Admin') {
      // Create the project payload
      const newProject = {
        id: 0, // ID will be auto-incremented by the backend
        title: project.trim(), // Ensure that the title is not empty or just whitespace
        category: 'string', // Placeholder value for category, adjust as needed
        users: [], // Initially an empty array
      };

      // Log the payload for debugging
      console.log('Sending project payload:', newProject);

      // Send the POST request
      this.http.post(this.projectsApiUrl, newProject).subscribe(
        (response: any) => {
          console.log('Project added successfully:', response);
          // Add the new project to the local list and update the UI
          this.projects.push(response);
        },
        (error) => {
          // Log the error response for debugging
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

  toggleSkillSelection(skill: any) {
    const skillId = skill.id; // Access the skill ID from the skill object
    if (this.selectedSkills.includes(skill.title)) {
      this.selectedSkills = this.selectedSkills.filter(
        (s) => s !== skill.title
      );
      this.subscribeOrUnsubscribe(skillId, 'unsubscribe', 'Skill'); // Call API to unsubscribe
    } else {
      this.selectedSkills.push(skill.title);
      this.subscribeOrUnsubscribe(skillId, 'subscribe', 'Skill'); // Call API to subscribe
    }
    this.authService.updateCurrentUserProjects(this.selectedSkills); // Here "Projects" are actually skills
  }

  toggleProjectSelection(project: any) {
    const projectId = project.id; // Access the project ID from the project object
    if (this.selectedProjects.includes(project.title)) {
      this.selectedProjects = this.selectedProjects.filter(
        (p) => p !== project.title
      );
      this.subscribeOrUnsubscribe(projectId, 'unsubscribe', 'Project'); // Call API to unsubscribe
    } else {
      this.selectedProjects.push(project.title);
      this.subscribeOrUnsubscribe(projectId, 'subscribe', 'Project'); // Call API to subscribe
    }
    this.authService.updateCurrentUserExpertise(this.selectedProjects); // Here "Expertise" are actually projects
  }

  subscribeOrUnsubscribe(
    itemId: number,
    action: 'subscribe' | 'unsubscribe',
    type: 'Skill' | 'Project'
  ) {
    const apiUrl = type === 'Skill' ? this.skillsApiUrl : this.projectsApiUrl;
    if (this.userId !== null) {
      const url = `${apiUrl}/Subscribe/${itemId}?subscribe=${action}`;

      // Send both itemId (skillId/projectId) and userId in the request body
      const updateDto = {
        Id: itemId,
        UserId: this.userId,
      };

      this.http.put(url, updateDto).subscribe(
        (response: any) => {
          console.log(`${type} ${action}d successfully:`, response);
        },
        (error) => {
          console.error(`Error during ${action} operation:`, error);
        }
      );
    }
  }
}
