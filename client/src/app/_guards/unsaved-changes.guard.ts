import { Injectable, inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanDeactivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { MemberEditprofileComponent } from '../members/member-editprofile/member-editprofile.component';
import { PopUpConfirmationComponent } from '../modals/pop-up-confirmation/pop-up-confirmation.component';
import { PopupConfirmationService } from '../_services/popup-confirmation.service';

@Injectable({
  providedIn: 'root'
})
export class UnsavedChangesGuard implements CanDeactivate<MemberEditprofileComponent> {

  constructor(private serviceConfirm: PopupConfirmationService) {}


  canDeactivate(
    component: MemberEditprofileComponent,
    currentRoute: ActivatedRouteSnapshot,
    currentState: RouterStateSnapshot,
    nextState?: RouterStateSnapshot): Observable<boolean | 
    UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {


      if (component.formEdit?.dirty) return this.serviceConfirm.confirmAction();



    return true;
  }
  
}
