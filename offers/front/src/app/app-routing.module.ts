import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CandidateAuthGuard } from './guards/candidate-auth.guard';
import { CompanyAuthGuard } from './guards/company-auth.guard';

const routes: Routes = [
  {
    path: 'login',
    loadChildren: () => import('./components/pages/login-page/login-page.module').then(m => m.LoginPageModule)
  },
  {
    path: 'candidate',
    loadChildren: () => import('./components/pages/candidate-page/candidate-page.module').then(m => m.CandidatePageModule),
    canActivate: [CandidateAuthGuard]
  },
  {
    path: 'company',
    loadChildren: () => import('./components/pages/company-page/company-page.module').then(m => m.CompanyPageModule),
    canActivate: [CompanyAuthGuard]
  },
  {
    path: '**',
    redirectTo: 'login'
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
