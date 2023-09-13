import { Injectable, inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, CanActivateFn, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable, map } from 'rxjs';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

export const AdminGuard: CanActivateFn = (route, state) => {
 
    const serviceAccount = inject(AccountService)
    const toast = inject(ToastrService)

    // Get the user from the local storage
    return serviceAccount.currentUser$.pipe(
      map(u => {

        // Check if we have user
        if (u == null) {
          toast.error('You are not authorized to access this area!')
          return false;
        }

        // Check if we have admin / Mod role
        if (u.roles.includes('Admin') || u.roles.includes('Moderator')) return true;

        // User not authorized
        else {
          toast.error('You are not authorized to access this area!');
          return false;
        }
      })
    )
};
