import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanDeactivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { MemberEditprofileComponent } from '../members/member-editprofile/member-editprofile.component';

@Injectable({
  providedIn: 'root'
})
export class UnsavedChangesGuard implements CanDeactivate<MemberEditprofileComponent> {
  canDeactivate(
    component: MemberEditprofileComponent,
    currentRoute: ActivatedRouteSnapshot,
    currentState: RouterStateSnapshot,
    nextState?: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
      if (component.formEdit?.dirty) return confirm('Unsaved changes will be LOST if you continue!')
      //TODO: Change this alert (this is browser default -> Looks bad!)



    return true;
  }
  
}
