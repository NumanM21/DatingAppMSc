import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { User } from '../_models/User';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseURL = environment.apiUrl; 

  constructor(private httpClient: HttpClient) {   }



  //Updating user roles 
  UserRolesUpdate(username: string, roles: string){
    return this.httpClient.post(this.baseURL + 'admin/admin-edit-roles/' + username + '?roles=' +roles, {});
  }

  loadUsersWithRoles(){
    return this.httpClient.get<User[]>(this.baseURL + 'admin/app-users-with-roles'); 
  }

}
