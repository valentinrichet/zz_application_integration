import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { CandidateService } from 'src/app/providers/candidate.service';
import { environment } from './../../../../environments/environment.prod';
import { CompanyService } from './../../../providers/company.service';

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss']
})
export class LoginPageComponent implements OnInit {
  public loginForm: FormGroup;
  private _isLoading: boolean;
  private _willSignInAsCandidate: boolean;

  constructor(private _candidateService: CandidateService, private _companyService: CompanyService, private _formBuilder: FormBuilder, private _snackBar: MatSnackBar, private _router: Router) {
    this.initForm();
    this._isLoading = false;
  }

  public get isLoading(): boolean {
    return this._isLoading;
  }

  public ngOnInit(): void {
    if (this._candidateService.hasToken || this._companyService.hasToken) {
      this._isLoading = true;
      if (this._candidateService.hasToken) {
        this._candidateService.signInWithToken()
          .then(() => {
            this._router.navigateByUrl("/candidate");
          })
          .catch((exception) => {
            this.showErrorMessage(exception);
          })
          .finally(() => {
            this._isLoading = false;
          });
      } else {
        this._companyService.signInWithToken()
          .then(() => {
            this._router.navigateByUrl("/company");
          })
          .catch((exception) => {
            this.showErrorMessage(exception);
          })
          .finally(() => {
            this._isLoading = false;
          });
      }
    }
  }

  private initForm(): void {
    this.loginForm = this._formBuilder.group({
      mail: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  public onSignInAsCandidate(): void {
    this._willSignInAsCandidate = true;
  }

  public onSignInAsCompany(): void {
    this._willSignInAsCandidate = false;
  }

  public async onSubmit(): Promise<void> {
    if (this._isLoading || this._candidateService.isSignedIn || this._companyService.isSignedIn) {
      return;
    }
    this._isLoading = true;
    setTimeout(async () => {
      try {
        if (this._willSignInAsCandidate === true) {
          await this._candidateService.signInWithPassword(this.loginForm.value.mail, this.loginForm.value.password);
        } else {
          await this._companyService.signInWithPassword(this.loginForm.value.mail, this.loginForm.value.password);
        }
        this.showMessage("You are signed in!");
        if (this._willSignInAsCandidate === true) {
          this._router.navigateByUrl("/candidate");
        } else {
          this._router.navigateByUrl("/company");
        }

      } catch (exception) {
        console.error(exception);
        this.showErrorMessage(exception);
      }
      this._isLoading = false;
    }, 1000);
  }

  private showMessage(message: string) {
    this._snackBar.open(message);
  }

  private showErrorMessage(exception: any) {
    let errorMessage: string = environment.errorMessage;
    if (exception.error != null) {
      errorMessage = typeof exception.error === "string" ? exception.error : exception.error[Object.keys(exception.error)[0]][0];
    }
    this._snackBar.open(errorMessage);
  }
}
