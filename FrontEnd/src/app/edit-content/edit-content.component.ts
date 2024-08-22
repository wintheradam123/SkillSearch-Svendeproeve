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
  expertise: string[] = [];
  selectedSkills: string[] = [];
  selectedExpertise: string[] = [];
  userId: number | null = null; // Store the user ID

  private readonly apiUrl = 'https://localhost:7208/api/Skill';

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
      this.selectedExpertise = currentUser.Expertise || [];
    }

    // Fetch skills from the API and populate the skills array
    this.http.get<any[]>(this.apiUrl).subscribe(
      (response) => {
        this.skills = response; // Use the API response to set the skills
        console.log('Skills loaded:', this.skills);
      },
      (error) => {
        console.error('Error fetching skills:', error);
      }
    );

    this.expertise = this.dataService.getExpertise();
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
      this.http.post(this.apiUrl, newSkill).subscribe(
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

  addExpertise(expertiseItem: string) {
    if (this.role === 'Admin') {
      this.dataService.addExpertise(expertiseItem);
      this.expertise = this.dataService.getExpertise(); // Update the list
    }
  }

  toggleSkillSelection(skill: any) {
    const skillId = skill.id; // Access the skill ID from the skill object
    if (this.selectedSkills.includes(skill.title)) {
      this.selectedSkills = this.selectedSkills.filter(
        (s) => s !== skill.title
      );
      this.subscribeOrUnsubscribe(skillId, 'unsubscribe'); // Call API to unsubscribe
    } else {
      this.selectedSkills.push(skill.title);
      this.subscribeOrUnsubscribe(skillId, 'subscribe'); // Call API to subscribe
    }
    this.authService.updateCurrentUserProjects(this.selectedSkills); // Here "Projects" are actually skills
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

  subscribeOrUnsubscribe(skillId: number, action: 'subscribe' | 'unsubscribe') {
    if (this.userId !== null) {
      const url = `${this.apiUrl}/Subscribe/${skillId}?subscribe=${action}`;

      // Send both skillId and userId in the request body
      const skillUpdateDto = {
        Id: skillId,
        UserId: this.userId,
      };

      this.http.put(url, skillUpdateDto).subscribe(
        (response: any) => {
          console.log(`Skill ${action}d successfully:`, response);
        },
        (error) => {
          console.error(`Error during ${action} operation:`, error);
        }
      );
    }
  }
}
