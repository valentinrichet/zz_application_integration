import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { environment } from './../../../../environments/environment';
import { ApiCreateOffer } from './../../../models/api/api-offer';
import { OfferForm } from './../../../models/form/form-offer';
import { Skill } from './../../../models/skill';
import { CompanyService } from './../../../providers/company.service';

@Component({
  selector: 'app-company-page',
  templateUrl: './company-page.component.html',
  styleUrls: ['./company-page.component.scss']
})
export class CompanyPageComponent implements OnInit {
  public readonly levels: ReadonlyArray<string>;
  public readonly types: ReadonlyArray<string>;
  public offerForm: FormGroup;
  private _isLoading: boolean;

  constructor(private _companyService: CompanyService, private _formBuilder: FormBuilder, private _snackBar: MatSnackBar, private _router: Router) {
    this.levels = ["TRAINEE", "JUNIOR", "SENIOR", "OTHER"];
    this.types = ["PART-TIME", "FULL-TIME", "OTHER"];
    this.initForm();
    this._isLoading = false;
  }

  public get isLoading(): boolean {
    return this._isLoading;
  }

  public get skills(): ReadonlyArray<Skill> {
    return this._companyService.skills;
  }

  public get skillsFormArray(): FormArray {
    return this.offerForm.controls.skills as FormArray;
  }

  public ngOnInit(): void { }

  private initForm(): void {
    this.offerForm = this._formBuilder.group({
      title: ["", Validators.required],
      description: ["", Validators.required],
      level: ["TRAINEE", Validators.pattern(`^(${this.levels.join("|").replace("+", "\\+")})$`)],
      type: ["PART-TIME", [Validators.required, Validators.pattern(`^(${this.types.join("|").replace("+", "\\+")})$`)]],
      wage: ["", Validators.pattern("\\d{0,15}")],
      skills: this._formBuilder.array([...this.skills.map(skill => this._formBuilder.control(false))])
    });
  }

  public async onReset(): Promise<void> {
    this.offerForm.controls.title.setValue("");
    this.offerForm.controls.description.setValue("");
    this.offerForm.controls.level.setValue("TRAINEE");
    this.offerForm.controls.type.setValue("PART-TIME");
    this.offerForm.controls.wage.setValue("");
    this.skillsFormArray.controls.forEach(skillControl => skillControl.setValue(false));

  }

  public async onSubmit(): Promise<void> {
    if (this._isLoading) {
      return;
    }

    this._isLoading = true;
    setTimeout(async () => {
      try {
        const { skills, ...offerValues }: OfferForm = this.offerForm.value;

        const offerSkills: bigint[] = this.skills.reduce((acc, curr, index) => {
          if (skills[index]) {
            acc.push(curr.id);
          }
          return acc;
        }, []);

        const apiCreateOffer: ApiCreateOffer = { idCompany: null, ...offerValues, skills: offerSkills };
        await this._companyService.createOffer(apiCreateOffer);
        this.showMessage("Your offer has been created!");
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
