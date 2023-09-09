import { HttpClient, HttpHeaderResponse, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/Member';
import { MemberEditprofileComponent } from '../members/member-editprofile/member-editprofile.component';
import { map, of } from 'rxjs';
import { ResultPaginated } from '../_models/Pagination';
import { parameterUser } from '../_models/parameterUser';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  // Service remains for lifetime of application, components distroyed and re-built, so can store memberLIST HERE (in service) once memberlist component created
  members: Member[] = []
  baseUrl = environment.apiUrl;


  constructor(private httpClient: HttpClient) { }

  getMembers(parameterUser: parameterUser) {

    // Need to send info as query string, so we need to create a new object of HttpParams (httpParams is a angular class that allows us to build up a list of parameters (query string) and pass them to our API)

    // Has to be params, parameter show error?
    let params = this.getHeadPagination(parameterUser.pageNumber, parameterUser.pageSize);

    params = params.append('gender', parameterUser.gender);
    params = params.append('maxAge', parameterUser.maxAge);
    params = params.append('min', parameterUser.minAge);




    return this.getResultPagination<Member[]>(this.baseUrl + 'users', params);
  }

  //TODO: Create a separate class for pagination

  // Need full HTTP reponse, not just body, so we can access headers so we can get pagination info
  // <T> for reusability
  private getResultPagination<T>(URL: string, params: HttpParams) {
    const resultPaginated: ResultPaginated<T> = new ResultPaginated<T>;
    return this.httpClient.get<T>(URL , { observe: 'response', params }).pipe(
      map(response => {
        if (response.body) {
          resultPaginated.result = response.body;
        }
        // this is the header we want from our postman response
        const pagin = response.headers.get('Pagination');
        if (pagin) {
          resultPaginated.pagination = JSON.parse(pagin); // to convert serialized json into object (pagination is a string, so we need to convert it to an object)!
        }
        return resultPaginated;
      })
    );
  }

  // Helper method to get pagination info from parameterUser class
  private getHeadPagination(pageNumber: number, pageSize: number) {
    let params = new HttpParams();

    params = params.append('pageNumber', pageNumber);
    params = params.append('pageSize', pageSize);

    return params;
  }

  getMember(username: string) {
    const member = this.members.find(x => x.userName == username)
    if (member) return of(member);
    return this.httpClient.get<Member>(this.baseUrl + 'users/' + username)
  }

  updateMember(member: Member) {
    return this.httpClient.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const idx = this.members.indexOf(member);
        // spread operator spreads member detail at THAT index, we can then update those details for that member
        this.members[idx] = { ...this.members[idx], ...member }
      })
    )
  }

  // Allows users to select main photo on edit profile
  setPhotoMain(photoId: number) {
    return this.httpClient.put(this.baseUrl + 'users/set-photo-main/' + photoId, {}) //put so we have to sent something back (we send an empty object {})
  }

  photoDelete(photoId: number) {
    // KEY: DON'T FORGET RETURN (will get error from .subscribe)
    return this.httpClient.delete(this.baseUrl + 'users/photo-delete/' + photoId);
  }

}
