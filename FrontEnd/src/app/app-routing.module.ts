import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { RoleGuard } from './role-guard.guard';


const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'dashboard', component: DashboardComponent, canActivate: [RoleGuard] },  // RoleGuard ensures only logged in users with the right role access the dashboard
  { path: '', redirectTo: '/login', pathMatch: 'full' },  // Default route
  { path: '**', redirectTo: '/login' }  // Catch-all for unmatched routes, redirect to login
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
