import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Candidate } from './models/candidate';
import { CandidateService } from './providers/candidate.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  constructor(private _candidateService: CandidateService, private _router: Router) { }

  public get candidate(): Candidate {
    return this._candidateService.candidate;
  }

  public signOut(): void {
    this._candidateService.signOut();
    this._router.navigateByUrl("/login");
  }
}
