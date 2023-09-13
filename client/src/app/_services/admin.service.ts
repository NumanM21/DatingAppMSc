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



  loadUsersWithRoles(){
    return this.httpClient.get<User[]>(this.baseURL + 'admin/app-users-with-roles'); 
  }
}
