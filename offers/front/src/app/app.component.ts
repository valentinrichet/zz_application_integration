import { CompanyService } from './providers/company.service';
import { Company } from './models/company';
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
  constructor(private _candidateService: CandidateService, private _companyService: CompanyService, private _router: Router) { }

  public get candidate(): Candidate {
    return this._candidateService.candidate;
  }

  public get company(): Company {
    return this._companyService.company;
  }

  public signOut(): void {
    if (this._candidateService.isSignedIn) {
      this._candidateService.signOut();
    } else {
      this._companyService.signOut();
    }

    this._router.navigateByUrl("/login");
  }
}
