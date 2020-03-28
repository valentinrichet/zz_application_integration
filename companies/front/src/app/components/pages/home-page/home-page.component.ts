import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { environment } from './../../../../environments/environment';
import { ApiUpdateCompany } from './../../../models/api-company';
import { CompanyService } from './../../../providers/company.service';

@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.scss']
})
export class HomePageComponent implements OnInit {
  public updateForm: FormGroup;
  private _isLoading: boolean;

  constructor(private _companyService: CompanyService, private _formBuilder: FormBuilder, private _snackBar: MatSnackBar, private _router: Router) {
    this.initForm();
    this._isLoading = false;
  }

  public get isLoading(): boolean {
    return this._isLoading;
  }

  public ngOnInit(): void {
  }

  private initForm(): void {
    this.updateForm = this._formBuilder.group({
      mail: [this._companyService.company.mail, [Validators.required, Validators.email]],
      password: ['', Validators.minLength(6)],
      name: [this._companyService.company.name, Validators.required],
      address: [this._companyService.company.address, Validators.required],
      description: [this._companyService.company.description, Validators.required]
    });
  }

  public async onReset(): Promise<void> {
    this.updateForm.controls.mail.setValue(this._companyService.company.mail);
    this.updateForm.controls.name.setValue(this._companyService.company.name);
    this.updateForm.controls.address.setValue(this._companyService.company.address);
    this.updateForm.controls.description.setValue(this._companyService.company.description);
  }

  public async onSubmit(): Promise<void> {
    if (this._isLoading) {
      return;
    }
    this._isLoading = true;
    setTimeout(async () => {
      try {
        const newCompany: ApiUpdateCompany = { ...this.updateForm.value };
        if (newCompany.password === "") {
          delete newCompany.password;
        }
        await this._companyService.updateCompany(newCompany);
        this.showMessage("Your company has been updated!");
      } catch (exception) {
        console.error(exception);
        this.showErrorMessage(exception);
        if (!this._companyService.isSignedIn) {
          this._router.navigateByUrl("/login");
        }
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
