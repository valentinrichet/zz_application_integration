import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { CompanyService } from './../providers/company.service';

@Injectable({
    providedIn: 'root',
})
export class CompanyAuthGuard implements CanActivate {
    constructor(private _router: Router, private _companyService: CompanyService) { }

    canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        if (this._companyService.isSignedIn) {
            return true;
        }
        this._router.navigateByUrl("/login");
        return false;
    }
}