import { Injectable, inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, CanActivateFn, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable, map } from 'rxjs';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

export const AdminGuard: CanActivateFn = () => {

  const serviceAccount = inject(AccountService)
  const toast = inject(ToastrService)

  // Get the user from the local storage
  return serviceAccount.currentUser$.pipe(
    map(u => {

      //FIXME: Debugg clogs
      // console.log('User:', u); // This will print the entire user object
      
      // console.log('User roles:', u?.appRole); // This will print the roles of the user

      // Check if we have user
      if (u == null) {
        toast.error('You are not authorized to access this area!')
        return false;
      }

      // Check if we have admin / Mod role
      if (u.appRole.includes('Admin') || u.appRole.includes('Moderator')) {

        // console.log('User has Admin or Moderator role. Access granted.');

        return true;
      }
      // User not authorized
      else {

        // console.log('User does not have Admin or Moderator role. Access denied.');

        toast.error('You are not authorized to access this area!');

        return false;
      }

    })
  )
};
