import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as JwtDecode from 'jwt-decode';
import { environment } from '../../environments/environment';
import { ApiCandidate, ApiCreateCandidate, ApiUpdateCandidate } from '../models/api/api-candidate';
import { Education } from '../models/education';
import { JwtToken } from '../models/jwt-token';
import { ApiSignInCandidate } from './../models/api/api-candidate';
import { ApiEducation } from './../models/api/api-education';
import { ApiExperience } from './../models/api/api-experience';
import { ApiSkill } from './../models/api/api-skill';
import { Candidate } from './../models/candidate';
import { Experience } from './../models/experience';
import { Skill } from './../models/skill';

@Injectable({
  providedIn: 'root'
})
export class CandidateService {
  private _skills: ReadonlyArray<Skill>;
  private _candidate: Candidate;
  private _token: string;

  constructor(private _http: HttpClient) {
    this._token = localStorage.getItem("token");
    this._candidate = null;
  }

  public get candidate(): Candidate {
    return this._candidate;
  }

  public get skills(): ReadonlyArray<Skill> {
    return this._skills;
  }

  public get hasToken(): boolean {
    return this._token != null;
  }

  public get isSignedIn(): boolean {
    return this._candidate != null && this._token != null;
  }

  public async signInWithPassword(mail: string, password: string): Promise<void> {
    const signInData: ApiSignInCandidate = { mail: mail, password: password };
    this._token = await this._http.post<string>(`${environment.apiBaseUrl}/Candidates/authenticate`, signInData).toPromise();
    await this.fetchCandidate();
    await this.fetchAllSkills();
  }

  public async signInWithToken(): Promise<void> {
    await this.fetchCandidate();
    await this.fetchAllSkills();
  }

  private async fetchCandidate(): Promise<void> {
    if (this._token == null) {
      throw { error: "You are not signed in yet." };
    }

    try {
      const apiCandidate: ApiCandidate = await this._http.get<ApiCandidate>(`${environment.apiBaseUrl}/Candidates/${JwtDecode<JwtToken>(this._token).Id}`, { headers: { "Authorization": `Bearer ${this._token}` } }).toPromise();
      this._candidate = new Candidate(apiCandidate);
      localStorage.setItem("token", this._token);
    } catch (exception) {
      console.error(exception);
      if (exception.status === 401) {
        this.signOut();
      }
      throw exception;
    }
  }

  private async fetchAllSkills(): Promise<void> {
    if (this._token == null) {
      throw { error: "You are not signed in yet." };
    }

    try {
      const apiSkills: ApiSkill[] = await this._http.get<ApiSkill[]>(`${environment.apiBaseUrl}/Skills`, { headers: { "Authorization": `Bearer ${this._token}` } }).toPromise();
      this._skills = apiSkills.map(apiSkill => new Skill(apiSkill));
    } catch (exception) {
      console.error(exception);
      if (exception.status === 401) {
        this.signOut();
      }
      throw exception;
    }
  }

  public async updateCandidate(newCandidate: ApiUpdateCandidate, newSkills: ReadonlyArray<Skill>, newEducations: ReadonlyArray<Education>, newExperiences: ReadonlyArray<Experience>): Promise<void> {
    if (this._token == null) {
      throw { error: "You are not signed in yet." };
    }

    try {
      const userId: string = JwtDecode<JwtToken>(this._token).Id;
      const haveSkillsChanged: boolean = await this.updateSkills(userId, newSkills);
      const haveEducationsChanged: boolean = await this.updateEducations(userId, newEducations);
      const haveExperiencesChanged: boolean = await this.updateExperiences(userId, newExperiences);
      const haveCandidateInformationChanged: boolean = newCandidate.password != null || this._candidate.mail !== newCandidate.mail || this._candidate.firstName !== newCandidate.firstName || this._candidate.lastName !== newCandidate.lastName || this._candidate.address !== newCandidate.address || this._candidate.description !== newCandidate.description;

      if (haveCandidateInformationChanged) {
        await this._http.put<ApiCandidate>(`${environment.apiBaseUrl}/Candidates/${userId}`, newCandidate, { headers: { "Authorization": `Bearer ${this._token}` } }).toPromise();
      }

      if (haveSkillsChanged || haveEducationsChanged || haveExperiencesChanged || haveCandidateInformationChanged) {
        const apiCandidate: ApiCandidate = await this._http.get<ApiCandidate>(`${environment.apiBaseUrl}/Candidates/${userId}`, { headers: { "Authorization": `Bearer ${this._token}` } }).toPromise();
        this._candidate = new Candidate(apiCandidate);
      }
    } catch (exception) {
      console.error(exception);
      if (exception.status === 401) {
        this.signOut();
      }
      throw exception;
    }
  }

  private async updateSkills(userId: string, newSkills: ReadonlyArray<Skill>): Promise<boolean> {
    const removedSkillsMap: Map<bigint, Skill> = new Map(this._candidate.skills);
    const addedSkills: Skill[] = [];

    newSkills.forEach(skill => {
      if (removedSkillsMap.has(skill.id)) {
        removedSkillsMap.delete(skill.id);
      } else {
        addedSkills.push(skill);
      }
    });

    await Promise.all<void | ApiSkill>([
      ...[...removedSkillsMap.values()].map(removedSkill => this._http.delete<void>(`${environment.apiBaseUrl}/Candidates/${userId}/skills/${removedSkill.id}`, { headers: { "Authorization": `Bearer ${this._token}` } }).toPromise()),
      ...addedSkills.map(addedSkill => this._http.post<ApiSkill>(`${environment.apiBaseUrl}/Candidates/${userId}/skills`, { id: addedSkill.id }, { headers: { "Authorization": `Bearer ${this._token}` } }).toPromise())
    ]);

    return removedSkillsMap.size !== 0 || addedSkills.length !== 0;
  }

  private async updateEducations(userId: string, newEducations: ReadonlyArray<Education>): Promise<boolean> {
    const removedEducationMap: Map<bigint, Education> = new Map(this._candidate.educations);
    const addedEducations: Education[] = [];
    const updatedEducations: Education[] = [];

    newEducations.forEach(education => {
      if (removedEducationMap.has(education.id)) {
        const oldEducation: Education = removedEducationMap.get(education.id);
        if (oldEducation.title !== education.title || oldEducation.description !== education.description || oldEducation.level !== education.level) {
          updatedEducations.push(education);
        }
        removedEducationMap.delete(education.id);
      } else {
        addedEducations.push(education);
      }
    });

    await Promise.all<void | ApiEducation>([
      ...[...removedEducationMap.values()].map(removedEducation => this._http.delete<void>(`${environment.apiBaseUrl}/Candidates/${userId}/educations/${removedEducation.id}`, { headers: { "Authorization": `Bearer ${this._token}` } }).toPromise()),
      ...addedEducations.map(addedEducation => {
        const { id, ...apiEducation } = addedEducation;
        return this._http.post<ApiEducation>(`${environment.apiBaseUrl}/Candidates/${userId}/educations`, apiEducation, { headers: { "Authorization": `Bearer ${this._token}` } }).toPromise();
      }),
      ...updatedEducations.map(updatedEducation => {
        const { id, ...apiEducation } = updatedEducation;
        return this._http.put<ApiEducation>(`${environment.apiBaseUrl}/Candidates/${userId}/educations/${updatedEducation.id}`, apiEducation, { headers: { "Authorization": `Bearer ${this._token}` } }).toPromise()
      })
    ]);

    return removedEducationMap.size !== 0 || addedEducations.length !== 0;
  }

  private async updateExperiences(userId: string, newExperiences: ReadonlyArray<Experience>): Promise<boolean> {
    const removedExperienceMap: Map<bigint, Experience> = new Map(this._candidate.experiences);
    const addedExperiences: Experience[] = [];
    const updatedExperiences: Experience[] = [];

    newExperiences.forEach(experience => {
      if (removedExperienceMap.has(experience.id)) {
        const oldExperience: Experience = removedExperienceMap.get(experience.id);
        if (oldExperience.title !== experience.title || oldExperience.description !== experience.description || oldExperience.start !== experience.start || oldExperience.end !== experience.end) {
          updatedExperiences.push(experience);
        }
        removedExperienceMap.delete(experience.id);
      } else {
        addedExperiences.push(experience);
      }
    });

    await Promise.all<void | ApiExperience>([
      ...[...removedExperienceMap.values()].map(removedExperience => this._http.delete<void>(`${environment.apiBaseUrl}/Candidates/${userId}/experiences/${removedExperience.id}`, { headers: { "Authorization": `Bearer ${this._token}` } }).toPromise()),
      ...addedExperiences.map(addedExperience => {
        const { id, ...apiExperience } = addedExperience;
        return this._http.post<ApiExperience>(`${environment.apiBaseUrl}/Candidates/${userId}/experiences`, apiExperience, { headers: { "Authorization": `Bearer ${this._token}` } }).toPromise();
      }),
      ...updatedExperiences.map(updatedExperience => {
        const { id, ...apiExperience } = updatedExperience;
        return this._http.put<ApiExperience>(`${environment.apiBaseUrl}/Candidates/${userId}/experiences/${updatedExperience.id}`, apiExperience, { headers: { "Authorization": `Bearer ${this._token}` } }).toPromise()
      })
    ]);

    return removedExperienceMap.size !== 0 || addedExperiences.length !== 0;
  }

  public async signUp(mail: string, password: string): Promise<void> {
    const createCandidateData: ApiCreateCandidate = { mail: mail, password: password, firstName: "firstName", lastName: "lastName", address: "Address", description: "Description" };
    const apiCandidate: ApiCandidate = await this._http.post<ApiCandidate>(`${environment.apiBaseUrl}/Candidates`, createCandidateData).toPromise();

    const signInData: ApiSignInCandidate = { mail: mail, password: password };
    this._token = await this._http.post<string>(`${environment.apiBaseUrl}/Candidates/authenticate`, signInData).toPromise();
    this._candidate = new Candidate(apiCandidate);
    localStorage.setItem("token", this._token);
  }

  public async signOut() {
    this._candidate = null;
    this._token = null;
    localStorage.clear();
  }
}