import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { RoleGuard } from './role-guard.guard';
import { EditContentComponent } from './edit-content/edit-content.component';
import { CreateUserComponent } from './create-user/create-user.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [RoleGuard],
  }, // RoleGuard ensures only logged in users with the right role access the dashboard
  { path: '', redirectTo: '/login', pathMatch: 'full' }, // Default route
  { path: 'edit-content', component: EditContentComponent },
  { path: 'create-user', component: CreateUserComponent },
  { path: '**', redirectTo: '/login' }, // Catch-all for unmatched routes, redirect to login
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
