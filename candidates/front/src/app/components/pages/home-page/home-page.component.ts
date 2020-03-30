import { EducationForm, ExperienceForm, UpdateForm } from './../../../models/form/form-update';
import { Experience } from './../../../models/experience';
import { Education } from './../../../models/education';
import { Skill } from './../../../models/skill';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl, FormArray } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { CandidateService } from 'src/app/providers/candidate.service';
import { environment } from './../../../../environments/environment';

@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.scss']
})
export class HomePageComponent implements OnInit {
  public levels: ReadonlyArray<string>;
  public updateForm: FormGroup;
  private _isLoading: boolean;

  constructor(private _candidateService: CandidateService, private _formBuilder: FormBuilder, private _snackBar: MatSnackBar, private _router: Router) {
    this.levels = ["L1", "L2", "L3", "M1", "M2", "D1", "D2", "+"];
    this.initForm();
    this._isLoading = false;
  }

  public get isLoading(): boolean {
    return this._isLoading;
  }

  public get skills(): ReadonlyArray<Skill> {
    return this._candidateService.skills;
  }

  public get skillsFormArray(): FormArray {
    return this.updateForm.controls.skills as FormArray;
  }

  public get educationsFormArray(): FormArray {
    return this.updateForm.controls.educations as FormArray;
  }

  public get experiencesFormArray(): FormArray {
    return this.updateForm.controls.experiences as FormArray;
  }

  public educationsFormGroup(index: number): FormGroup {
    return this.educationsFormArray.controls[index] as FormGroup;
  }

  public experiencesFormGroup(index: number): FormGroup {
    return this.experiencesFormArray.controls[index] as FormGroup;
  }

  public ngOnInit(): void { }

  private initForm(): void {
    this.updateForm = this._formBuilder.group({
      mail: [this._candidateService.candidate.mail, [Validators.required, Validators.email]],
      password: ["", Validators.minLength(6)],
      firstName: [this._candidateService.candidate.firstName, Validators.required],
      lastName: [this._candidateService.candidate.lastName, Validators.required],
      address: [this._candidateService.candidate.address, Validators.required],
      description: [this._candidateService.candidate.description, Validators.required],
      skills: this._formBuilder.array([...this.skills.map(skill => this._formBuilder.control(this._candidateService.candidate.skills.has(skill.id)))]),
      educations: this._formBuilder.array([...this._candidateService.candidate.educations.values()].map(education => this.addEducationFormGroup(education))),
      experiences: this._formBuilder.array([...this._candidateService.candidate.experiences.values()].map(experience => this.addExperienceFormGroup(experience)))
    });
  }

  private addEducationFormGroup(education?: Education): FormGroup {
    return this._formBuilder.group({
      id: [education == null ? null : education.id],
      title: [education == null ? "" : education.title, Validators.required],
      description: [education == null ? "" : education.description, Validators.required],
      level: [education == null ? "L1" : education.level, [Validators.required, Validators.pattern(`^(${this.levels.join("|").replace("+", "\\+")})$`)]]
    });
  }

  public async addEducation(): Promise<void> {
    const newEducationGroupControl: AbstractControl = this.addEducationFormGroup();
    this.educationsFormArray.push(newEducationGroupControl);
  }

  public async removeEducation(index: number): Promise<void> {
    this.educationsFormArray.removeAt(index);
  }

  private addExperienceFormGroup(experience?: Experience): FormGroup {
    return this._formBuilder.group({
      id: [experience == null ? null : experience.id],
      title: [experience == null ? "" : experience.title, Validators.required],
      description: [experience == null ? "" : experience.description, Validators.required],
      start: [experience == null ? "" : new Date(experience.start), Validators.required],
      end: [experience?.end != null ? new Date(experience.end) : null]
    });
  }

  public async addExperience(): Promise<void> {
    const newExperienceGroupControl: AbstractControl = this.addExperienceFormGroup();
    this.experiencesFormArray.push(newExperienceGroupControl);
  }

  public async removeExperience(index: number): Promise<void> {
    this.experiencesFormArray.removeAt(index);
  }

  public async onReset(): Promise<void> {
    this.updateForm.controls.mail.setValue(this._candidateService.candidate.mail);
    this.updateForm.controls.firstName.setValue(this._candidateService.candidate.firstName);
    this.updateForm.controls.lastName.setValue(this._candidateService.candidate.lastName);
    this.updateForm.controls.address.setValue(this._candidateService.candidate.address);
    this.updateForm.controls.description.setValue(this._candidateService.candidate.description);
    this.skillsFormArray.controls.forEach((skillControl, index) => skillControl.setValue(this._candidateService.candidate.skills.has(this.skills[index].id)));
    this.educationsFormArray.clear();
    [...this._candidateService.candidate.educations.values()].forEach(education => this.educationsFormArray.push(this.addEducationFormGroup(education)));
    this.experiencesFormArray.clear();
    [...this._candidateService.candidate.experiences.values()].forEach(experience => this.experiencesFormArray.push(this.addExperienceFormGroup(experience)));
  }

  public async onSubmit(): Promise<void> {
    if (this._isLoading) {
      return;
    }

    this._isLoading = true;
    setTimeout(async () => {
      try {
        const { skills, educations, experiences, ...newCandidate }: UpdateForm = this.updateForm.value;

        if (newCandidate.password === "") {
          delete newCandidate.password;
        }

        const newSkills: ReadonlyArray<Skill> = this.skills.reduce((acc, curr, index) => {
          if (skills[index] === true) {
            acc.push(curr);
          }
          return acc;
        }, []);

        const newEducations: ReadonlyArray<Education> = educations.map(education => new Education(education));

        const newExperiences: ReadonlyArray<Experience> = experiences.map(experience => {
          const { start, end, ...exp } = experience;
          return new Experience({ ...exp, start: new Date(start.getTime() - (start.getTimezoneOffset() * 60000)).toISOString(), end: end == null ? null : new Date(end.getTime() - (end.getTimezoneOffset() * 60000)).toISOString() });
        });

        await this._candidateService.updateCandidate(newCandidate, newSkills, newEducations, newExperiences);
        this.showMessage("Your candidate has been updated!");
      } catch (exception) {
        console.error(exception);
        this.showErrorMessage(exception);
        if (!this._candidateService.isSignedIn) {
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
