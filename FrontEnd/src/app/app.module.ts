import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module'; // Assumes your routing configuration is defined here

// Components
import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { EditContentComponent } from './edit-content/edit-content.component';

// Services
import { AuthService } from './Services/auth.service';

// Guards
import { RoleGuard } from './role-guard.guard';
import { CommonModule } from '@angular/common';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    DashboardComponent,
    EditContentComponent,
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpClientModule,
    AppRoutingModule,
    CommonModule,
  ],
  providers: [AuthService, RoleGuard],
  bootstrap: [AppComponent],
})
export class AppModule {}
