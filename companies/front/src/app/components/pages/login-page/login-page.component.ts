import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
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
  private _willSignIn: boolean;

  constructor(private _companyService: CompanyService, private _formBuilder: FormBuilder, private _snackBar: MatSnackBar, private _router: Router) {
    this.initForm();
    this._isLoading = false;
  }

  public get isLoading(): boolean {
    return this._isLoading;
  }

  public ngOnInit(): void {
    if (this._companyService.hasToken) {
      this._isLoading = true;
      this._companyService.signInWithToken()
        .then(() => {
          this._router.navigateByUrl("/home");
        })
        .catch((exception) => {
          this.showErrorMessage(exception);
        })
        .finally(() => {
          this._isLoading = false;
        });
    }
  }

  private initForm(): void {
    this.loginForm = this._formBuilder.group({
      mail: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  public onSignIn(): void {
    this._willSignIn = true;
  }

  public onSignUp(): void {
    this._willSignIn = false;
  }

  public async onSubmit(): Promise<void> {
    if (this._isLoading) {
      return;
    }
    this._isLoading = true;
    setTimeout(async () => {
      try {
        if (this._willSignIn === true) {
          await this._companyService.signInWithPassword(this.loginForm.value.mail, this.loginForm.value.password);
        } else {
          await this._companyService.signUp(this.loginForm.value.mail, this.loginForm.value.password);
        }
        this.showMessage("You are signed in!");
        this._router.navigateByUrl("/home");
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
