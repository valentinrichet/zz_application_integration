import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as JwtDecode from 'jwt-decode';
import { Company } from '../models/company';
import { environment } from './../../environments/environment';
import { ApiCompany, ApiCreateCompany, ApiSignInCompany, ApiUpdateCompany } from './../models/api-company';
import { JwtToken } from './../models/jwt-token';

@Injectable({
  providedIn: 'root'
})
export class CompanyService {
  private _company: Company;
  private _token: string;

  constructor(private _http: HttpClient) {
    this._token = localStorage.getItem("token");
    this._company = null;
  }

  public get company(): Company {
    return this._company;
  }

  public get hasToken(): boolean {
    return this._token != null;
  }

  public get isSignedIn(): boolean {
    return this._company != null && this._token != null;
  }

  public async signInWithPassword(mail: string, password: string): Promise<void> {
    const signInData: ApiSignInCompany = { mail: mail, password: password };
    this._token = await this._http.post<string>(`${environment.apiBaseUrl}/Companies/authenticate`, signInData).toPromise();
    await this.fetchCompany();
  }

  public async signInWithToken(): Promise<void> {
    await this.fetchCompany();
  }

  private async fetchCompany(): Promise<void> {
    if (this._token == null) {
      throw { error: "You are not signed in yet." };
    }

    try {
      const apiCompany: ApiCompany = await this._http.get<ApiCompany>(`${environment.apiBaseUrl}/Companies/${JwtDecode<JwtToken>(this._token).Id}`, { headers: { "Authorization": `Bearer ${this._token}` } }).toPromise();
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

  public async updateCompany(newCompany: ApiUpdateCompany): Promise<void> {
    if (this._token == null) {
      throw { error: "You are not signed in yet." };
    }

    try {
      const apiCompany: ApiCompany = await this._http.put<ApiCompany>(`${environment.apiBaseUrl}/Companies/${JwtDecode<JwtToken>(this._token).Id}`, newCompany, { headers: { "Authorization": `Bearer ${this._token}` } }).toPromise();
      this._company = new Company(apiCompany);
    } catch (exception) {
      console.error(exception);
      if (exception.status === 401) {
        this.signOut();
      }
      throw exception;
    }
  }

  public async signUp(mail: string, password: string): Promise<void> {
    const createCompanyData: ApiCreateCompany = { mail: mail, password: password, name: "Name", address: "Address", description: "Description" };
    const apiCompany: ApiCompany = await this._http.post<ApiCompany>(`${environment.apiBaseUrl}/Companies`, createCompanyData).toPromise();

    const signInData: ApiSignInCompany = { mail: mail, password: password };
    this._token = await this._http.post<string>(`${environment.apiBaseUrl}/Companies/authenticate`, signInData).toPromise();
    this._company = new Company(apiCompany);
    localStorage.setItem("token", this._token);
  }

  public async signOut() {
    this._company = null;
    this._token = null;
    localStorage.clear();
  }
}