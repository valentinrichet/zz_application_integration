import { Offer } from './../models/offer';
import { ApiOffer } from './../models/api/api-offer';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as JwtDecode from 'jwt-decode';
import { environment } from '../../environments/environment';
import { ApiCandidate } from '../models/api/api-candidate';
import { JwtToken } from '../models/jwt-token';
import { ApiSignInCandidate } from './../models/api/api-candidate';
import { ApiSkill } from './../models/api/api-skill';
import { Candidate } from './../models/candidate';
import { Skill } from './../models/skill';

@Injectable({
  providedIn: 'root'
})
export class CandidateService {
  private _skills: ReadonlyArray<Skill>;
  private _candidate: Candidate;
  private _token: string;

  constructor(private _http: HttpClient) {
    try {
      const token: string = localStorage.getItem("token");
      this._token = token == null ? null : (JwtDecode<JwtToken>(token).Type === "CANDIDATE" ? token : null);
    } catch (exception) {
      console.error(exception);
      localStorage.clear();
      this._token = null;
    }
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
    this._token = await this._http.post<string>(`${environment.apiCandidateBaseUrl}/Candidates/authenticate`, signInData).toPromise();
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
      const apiCandidate: ApiCandidate = await this._http.get<ApiCandidate>(`${environment.apiCandidateBaseUrl}/Candidates/${JwtDecode<JwtToken>(this._token).Id}`, { headers: { "Authorization": `Bearer ${this._token}` } }).toPromise();
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

  public async signOut() {
    this._candidate = null;
    this._token = null;
    this._skills = null;
    localStorage.clear();
  }

  public async fetchOffers(page?: number): Promise<Offer[]> {
    if (this._token == null) {
      throw { error: "You are not signed in yet." };
    }

    try {
      page = page == null || page < 1 ? 1 : page;
      const apiOffers: ApiOffer[] = await this._http.get<ApiOffer[]>(`${environment.apiOfferBaseUrl}/Offers?page=${page}`, { headers: { "Authorization": `Bearer ${this._token}` } }).toPromise();
      const offers: Offer[] = apiOffers.map(apiOffer => new Offer(apiOffer));
      return offers;
    } catch (exception) {
      console.error(exception);
      if (exception.status === 401) {
        this.signOut();
      }
      throw exception;
    }
  }
}