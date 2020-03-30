import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { CandidateService } from '../providers/candidate.service';

@Injectable({
    providedIn: 'root',
})
export class CandidateAuthGuard implements CanActivate {
    constructor(private _router: Router, private _candidateService: CandidateService) { }

    canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        if (this._candidateService.isSignedIn) {
            return true;
        }
        this._router.navigateByUrl("/login");
        return false;
    }
}