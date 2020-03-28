import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Company } from './models/company';
import { CompanyService } from './providers/company.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  constructor(private _companyService: CompanyService, private _router: Router) { }

  public get company(): Company {
    return this._companyService.company;
  }

  public signOut(): void {
    this._companyService.signOut();
    this._router.navigateByUrl("/login");
  }
}
