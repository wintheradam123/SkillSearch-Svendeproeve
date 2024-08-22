import { Component, OnInit } from '@angular/core';
import { AuthService } from '../Services/auth.service';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http'; // Import HttpClient
import algoliasearch from 'algoliasearch/lite';

interface AlgoliaHit {
  objectID: string;
  displayName: string;
  userPrincipalName: string;
  officeLocation: string;
  jobTitle: string;
  role: string;
}

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
})
export class DashboardComponent implements OnInit {
  role: string | null = null;
  data: any[] = [];
  uniqueStudios: string[] = [];
  uniqueJobTitles: string[] = [];
  uniqueProjects: string[] = [];
  uniqueSkills: string[] = [];
  filteredData: any[] = [];
  selectedFilters: any = {
    Studio: new Set<string>(),
    JobTitle: new Set<string>(),
    Projects: new Set<string>(),
    Skills: new Set<string>(),
  };

  showFilters: boolean = false;
  searchQuery: string = ''; // New property for the search query

  // Algolia credentials
  algoliaAppId: string = 'UW17LRXEG9';
  algoliaApiKey: string = '21a8d95e001480809f816fdebcdeee77';
  algoliaIndexName: string = 'dev_SkillSearch_Users';

  private algoliaClient: any;
  private algoliaIndex: any;

  private readonly skillApiUrl = 'https://localhost:7208/api/Skill'; // API URL for skills
  private readonly projectApiUrl = 'https://localhost:7208/api/Project'; // API URL for projects

  constructor(
    private authService: AuthService,
    private router: Router,
    private http: HttpClient // Inject HttpClient
  ) {
    this.algoliaClient = algoliasearch(this.algoliaAppId, this.algoliaApiKey);
    this.algoliaIndex = this.algoliaClient.initIndex(this.algoliaIndexName);
  }

  ngOnInit() {
    const currentUser = this.authService.getCurrentUserValue();
    if (currentUser && currentUser.role) {
      this.role = currentUser.role;
      console.log('User role:', this.role);
    }

    this.fetchAlgoliaData(); // Fetch data from Algolia
    this.fetchSkillsAndProjects(); // Fetch skills and projects
  }

  fetchAlgoliaData() {
    this.algoliaIndex.search('').then((response: { hits: any[] }) => {
      const hits = response.hits;
      console.log('Algolia response hits:', hits);

      // Map the Algolia fields to your expected structure
      this.data = hits.map((hit) => ({
        Name: hit.displayName,
        userPrincipalName: hit.userPrincipalName,
        Studio: hit.officeLocation,
        JobTitle: hit.jobTitle,
        Role: hit.role,
        Projects: hit.solutions
          ? hit.solutions.map((solution: any) => solution.solutionName)
          : [], // Map 'solutions' to 'Projects'
        Skills: hit.skills ? hit.skills.map((skill: any) => skill.tag) : [], // Map 'skills' tags to 'Skills'
      }));

      this.filteredData = this.data;

      // Populate filter options based on the fetched data
      this.uniqueStudios = [
        ...new Set(this.data.map((person) => person.Studio)),
      ];
      this.uniqueJobTitles = [
        ...new Set(this.data.map((person) => person.JobTitle)),
      ];
      this.uniqueProjects = [
        ...new Set(this.data.flatMap((person) => person.Projects)),
      ];
      this.uniqueSkills = [
        ...new Set(this.data.flatMap((person) => person.Skills)),
      ];
    });
  }

  fetchSkillsAndProjects() {
    // Fetch skills from the API
    this.http.get<any[]>(this.skillApiUrl).subscribe(
      (skills) => {
        this.uniqueSkills = skills.map((skill) => skill.title);
        console.log('Skills loaded:', this.uniqueSkills);
      },
      (error) => {
        console.error('Error fetching skills:', error);
      }
    );

    // Fetch projects from the API
    this.http.get<any[]>(this.projectApiUrl).subscribe(
      (projects) => {
        this.uniqueProjects = projects.map((project) => project.title);
        console.log('Projects loaded:', this.uniqueProjects);
      },
      (error) => {
        console.error('Error fetching projects:', error);
      }
    );
  }

  performSearch() {
    this.filteredData = this.data.filter(
      (person) =>
        person.Name.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        person.Studio.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        person.JobTitle.toLowerCase().includes(this.searchQuery.toLowerCase())
    );
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  editContent() {
    // Call the API before navigating to the Edit Content page
    this.http.get(this.skillApiUrl).subscribe(
      (response: any) => {
        console.log('Skills fetched successfully:', response);
        this.router.navigate(['/edit-content']); // Navigate after API call
      },
      (error) => {
        console.error('Error fetching skills:', error);
      }
    );
  }

  toggleFilters() {
    this.showFilters = !this.showFilters;
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
      const skillMatch =
        !this.selectedFilters.Skills.size ||
        person.Skills.some((skill: string) =>
          this.selectedFilters.Skills.has(skill)
        );
      return studioMatch && jobTitleMatch && projectMatch && skillMatch;
    });
  }
}
