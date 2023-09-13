import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { User } from '../_models/User';
import { Observable, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseURL = environment.apiUrl; 

  constructor(private httpClient: HttpClient) {   }



  //Updating user roles 
  UserRolesUpdate(username: string, roles: string): Observable<string[]> {
    return this.httpClient.post(`${this.baseURL}admin/admin-edit-roles/${username}?newRoles=${roles}`, {}).pipe(
      map((response: any) => response as string[])
    );
  }
  

  loadUsersWithRoles(){
    return this.httpClient.get<User[]>(this.baseURL + 'admin/app-users-with-roles'); 
  }

}
