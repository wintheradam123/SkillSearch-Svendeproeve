import { Component, OnInit } from '@angular/core';
import { AuthService } from './app.routes';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  constructor(private authService: AuthService) {}
  title = 'Svendeprøve';

  ngOnInit() {
    this.authService.currentUser.subscribe(user => {
      if (!user) {
        // Optionally handle a case where no user is found on app init
        console.log('No user found on app init, user might need to login');
      } else {
        console.log('User found on app init, user is logged in', user);
      }
    });
  }
}
