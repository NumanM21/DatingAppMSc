import { HttpClient, HttpHeaderResponse, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/Member';
import { MemberEditprofileComponent } from '../members/member-editprofile/member-editprofile.component';
import { map, of } from 'rxjs';
import { ResultPaginated } from '../_models/Pagination';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  // Service remains for lifetime of application, components distroyed and re-built, so can store memberLIST HERE (in service) once memberlist component created

  resultPaginated: ResultPaginated<Member[]> = new ResultPaginated<Member[]>();
  members: Member[] = []
  baseUrl = environment.apiUrl;

  constructor(private httpClient: HttpClient) { }

  getMembers(page? : number, perPageItems? : number) {
    // Need to send info as query string, so we need to create a new object of HttpParams (httpParams is a angular class that allows us to build up a list of parameters (query string) and pass them to our API)

    // Has to be params, parameter show error?
    let params = new HttpParams();
    if (page && perPageItems) {
      params = params.append('pageNumber', page)
      params = params.append('pageSize', perPageItems)
    }


   
    // Need full HTTP reponse, not just body, so we can access headers so we can get pagination info
    return this.httpClient.get<Member[]>(this.baseUrl + 'users', {observe: 'response', params}).pipe(
      map(response => {
        if (response.body) {
          this.resultPaginated.result = response.body;
        }
        const pagin = response.headers.get('Pagination'); // this is the header we want from our postman response
        if (pagin){
          this.resultPaginated.pagination = JSON.parse(pagin); // to convert serialized json into object (pagination is a string, so we need to convert it to an object)!
        }
        return this.resultPaginated;
      })
    )
   
  }

  getMember(username: string) {
    const member = this.members.find(x => x.userName == username)
    if (member) return of(member);
    return this.httpClient.get<Member>(this.baseUrl + 'users/' + username)
  }

  updateMember(member: Member){
    return this.httpClient.put(this.baseUrl+'users', member).pipe(
      map(() => {
        const idx = this.members.indexOf(member);
        // spread operator spreads member detail at THAT index, we can then update those details for that member
        this.members[idx] = {...this.members[idx], ...member}
      })
    )
  }

    // Allows users to select main photo on edit profile
  setPhotoMain(photoId: number){
    return this.httpClient.put(this.baseUrl + 'users/set-photo-main/'+ photoId, {}) //put so we have to sent something back (we send an empty object {})
  }

  photoDelete(photoId:number){
    // KEY: DON'T FORGET RETURN (will get error from .subscribe)
   return this.httpClient.delete(this.baseUrl + 'users/photo-delete/'+photoId);
  }

}
