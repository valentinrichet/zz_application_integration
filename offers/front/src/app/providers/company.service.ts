import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as JwtDecode from 'jwt-decode';
import { Company } from '../models/company';
import { environment } from './../../environments/environment';
import { ApiCompany, ApiSignInCompany } from './../models/api/api-company';
import { ApiCreateOffer, ApiOffer } from './../models/api/api-offer';
import { ApiSkill } from './../models/api/api-skill';
import { JwtToken } from './../models/jwt-token';
import { Skill } from './../models/skill';

@Injectable({
  providedIn: 'root'
})
export class CompanyService {
  private _skills: ReadonlyArray<Skill>;
  private _company: Company;
  private _token: string;

  constructor(private _http: HttpClient) {
    try {
      const token: string = localStorage.getItem("token");
      this._token = this._token = token == null ? null : (JwtDecode<JwtToken>(token).Type === "COMPANY" ? token : null);
    } catch (exception) {
      console.error(exception);
      localStorage.clear();
      this._token = null;
    }

    this._company = null;
  }

  public get company(): Company {
    return this._company;
  }

  public get skills(): ReadonlyArray<Skill> {
    return this._skills;
  }

  public get hasToken(): boolean {
    return this._token != null;
  }

  public get isSignedIn(): boolean {
    return this._company != null && this._token != null;
  }

  public async signInWithPassword(mail: string, password: string): Promise<void> {
    const signInData: ApiSignInCompany = { mail: mail, password: password };
    this._token = await this._http.post<string>(`${environment.apiCompanyBaseUrl}/Companies/authenticate`, signInData).toPromise();
    await this.fetchCompany();
    await this.fetchAllSkills();
  }

  public async signInWithToken(): Promise<void> {
    await this.fetchCompany();
    await this.fetchAllSkills();
  }

  private async fetchCompany(): Promise<void> {
    if (this._token == null) {
      throw { error: "You are not signed in yet." };
    }

    try {
      const apiCompany: ApiCompany = await this._http.get<ApiCompany>(`${environment.apiCompanyBaseUrl}/Companies/${JwtDecode<JwtToken>(this._token).Id}`, { headers: { "Authorization": `Bearer ${this._token}` } }).toPromise();
      this._company = new Company(apiCompany);
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
      const apiSkills: ApiSkill[] = await this._http.get<ApiSkill[]>(`${environment.apiCandidateBaseUrl}/Skills`, { headers: { "Authorization": `Bearer ${this._token}` } }).toPromise();
      this._skills = apiSkills.map(apiSkill => new Skill(apiSkill));
    } catch (exception) {
      console.error(exception);
      if (exception.status === 401) {
        this.signOut();
      }
      throw exception;
    }
  }

  public async createOffer(apiCreateOffer: ApiCreateOffer) {
    if (this._token == null) {
      throw { error: "You are not signed in yet." };
    }

    try {
      apiCreateOffer.idCompany = this.company.id;
      await this._http.post<ApiOffer>(`${environment.apiOfferBaseUrl}/Offers`, apiCreateOffer, { headers: { "Authorization": `Bearer ${this._token}` } }).toPromise();
    } catch (exception) {
      console.error(exception);
      if (exception.status === 401) {
        this.signOut();
      }
      throw exception;
    }
  }

  public async signOut() {
    this._company = null;
    this._token = null;
    this._skills = null;
    localStorage.clear();
  }
}