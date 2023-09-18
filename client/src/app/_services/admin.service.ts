import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { User } from '../_models/User';
import { Observable, map } from 'rxjs';
import { Photo } from '../_models/Photo';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseURL = environment.apiUrl;

  constructor(private httpClient: HttpClient) { }



  //Updating user roles 
  UserRolesUpdate(username: string, roles: string): Observable<string[]> {
    return this.httpClient.post(`${this.baseURL}admin/admin-edit-roles/${username}?newRoles=${roles}`, {}).pipe(
      map((response: any) => response as string[])
    );
  }

  // Getting users with roles
  loadUsersWithRoles() {
    return this.httpClient.get<User[]>(this.baseURL + 'admin/app-users-with-roles');
  }

  // Getting photos for approval
  PhotosForApprovalGetter() {
    return this.httpClient.get<Photo[]>(this.baseURL + 'admin/moderate-unapproved-photos');
  }

  // Approving photos
  PhotoApprover(photoId: number) {
    return this.httpClient.post(this.baseURL + 'admin/photo-approve/' + photoId, {});
  }

  // Rejecting photos
  PhotoUnapproved(photoId: number) {
    return this.httpClient.post(this.baseURL + 'admin/photo-unapproved/' + photoId, {});
  }


}
