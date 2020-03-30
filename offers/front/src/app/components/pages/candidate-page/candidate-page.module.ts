import { ScrollingModule } from '@angular/cdk/scrolling';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { CandidatePageRoutingModule } from './candidate-page-routing.module';
import { CandidatePageComponent } from './candidate-page.component';


@NgModule({
  declarations: [CandidatePageComponent],
  imports: [
    CommonModule,
    CandidatePageRoutingModule,
    ScrollingModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ]
})
export class CandidatePageModule { }
