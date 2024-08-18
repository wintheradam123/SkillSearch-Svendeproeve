import { Component, OnInit } from '@angular/core';
import { AuthService } from '../Services/auth.service';
import { Router } from '@angular/router';
import algoliasearch from 'algoliasearch/lite';

// Define the AlgoliaHit interface at the top of the file
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
  data: any[] = []; // Will be populated by Algolia data
  uniqueStudios: string[] = [];
  uniqueJobTitles: string[] = [];
  uniqueProjects: string[] = [];
  uniqueExpertise: string[] = [];
  filteredData: any[] = [];
  selectedFilters: any = {
    Studio: new Set<string>(),
    JobTitle: new Set<string>(),
    Projects: new Set<string>(),
    Expertise: new Set<string>(),
  };

  showFilters: boolean = false;

  // Algolia credentials
  algoliaAppId: string = 'UW17LRXEG9';
  algoliaApiKey: string = '21a8d95e001480809f816fdebcdeee77';
  algoliaIndexName: string = 'dev_SkillSearch_Users';

  private algoliaClient: any;
  private algoliaIndex: any;

  constructor(private authService: AuthService, private router: Router) {
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
  }

  fetchAlgoliaData() {
    this.algoliaIndex.search('').then((response: { hits: AlgoliaHit[] }) => {
      const hits = response.hits;
      console.log('Algolia response hits:', hits);

      // Map the Algolia fields to your expected structure
      this.data = hits.map((hit) => ({
        Name: hit.displayName,
        Studio: hit.officeLocation,
        JobTitle: hit.jobTitle,
        Role: hit.role,
        Projects: [], // Placeholder if Projects data is not in Algolia
        Expertise: [], // Placeholder if Expertise data is not in Algolia
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
      this.uniqueExpertise = [
        ...new Set(this.data.flatMap((person) => person.Expertise)),
      ];
    });
  }

  logout() {
    console.log('Logout clicked');
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  editContent() {
    console.log('Navigating to edit content page');
    this.router.navigate(['/edit-content']);
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
      const expertiseMatch =
        !this.selectedFilters.Expertise.size ||
        person.Expertise.some((expertise: string) =>
          this.selectedFilters.Expertise.has(expertise)
        );
      return studioMatch && jobTitleMatch && projectMatch && expertiseMatch;
    });
  }
}
