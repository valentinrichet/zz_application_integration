import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { CandidatePageComponent } from './candidate-page.component';

const routes: Routes = [{ path: '', component: CandidatePageComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CandidatePageRoutingModule { }
