import { HttpClient, HttpHeaderResponse, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/Member';
import { MemberEditprofileComponent } from '../members/member-editprofile/member-editprofile.component';
import { map, of, take, tap } from 'rxjs';
import { ResultPaginated } from '../_models/Pagination';
import { parameterUser } from '../_models/parameterUser';
import { AccountService } from './account.service';
import { User } from '../_models/User';
import { getHeadPagination, getResultPagination } from './HelperPagination';
import { createLogger } from '@microsoft/signalr/dist/esm/Utils';

@Injectable({
  providedIn: 'root'
})
export class MembersService {

  // Service remains for lifetime of application, components distroyed and re-built, so can store memberLIST HERE (in service) once memberlist component created
  members: Member[] = []
  cacheMember = new Map(); // JS Object -> KVP (similar to dictionary in C#) --> IMPORTANT! for responsive web design
  baseUrl = environment.apiUrl;
  user: User | undefined;
  parameterUser: parameterUser | undefined;


  constructor(private serviceAccount: AccountService, private httpClient: HttpClient) {

    this.serviceAccount.currentUser$.pipe(take(1)).subscribe({
      next: user => {

        if (user) {

          this.parameterUser = new parameterUser(user);
          this.user = user;
        }
      }
    })
  }

  getMembers(parameterUser: parameterUser) {

    // Get a key value pair of all the properties of parameterUser, then join them together with a dash
    const res = this.cacheMember.get(Object.values(parameterUser).join('-'));

    if (res) return of(res); // if we have a value in the cache, so already seen this query, return it


    // Need to send info as query string, so we need to create a new object of HttpParams (httpParams is a angular class that allows us to build up a list of parameters (query string) and pass them to our API)

    // Has to be params, parameter show error?
    let params = getHeadPagination(parameterUser.pageNumber, parameterUser.pageSize);

    params = params.append('orderByActive', parameterUser.orderByActive);
    params = params.append('gender', parameterUser.gender);
    params = params.append('maxAge', parameterUser.maxAge);
    params = params.append('min', parameterUser.minAge);


    return getResultPagination<Member[]>(this.baseUrl + 'users', params, this.httpClient).pipe(
      map(res => {
        this.cacheMember.set(Object.values(parameterUser).join('-'), res); // store the result in the cache (key, value)
        return res; // component will subscribe to this and use the result
      })
    )
  }

  getParameterUser() {
    return this.parameterUser;
  }

  setParameterUser(parameters: parameterUser) {
    this.parameterUser = parameters;
  }


  userFilterReset() {

    if (this.user) {
      this.parameterUser = new parameterUser(this.user);
      return this.parameterUser;
    }

    else return; // can't return clog!
  }



  getMember(username: string) {// KEY ! JS IS CASE SENSITIVE!!!!!!!!!!!!!!! FIXME::!!!!!

    const memberFromCache = [...this.cacheMember.values()] // get all the values from the cache (member objects)

      .reduce((preArr, currElement) => preArr.concat(currElement.result), []) // reduce to one array  [] -> Initial value 

      .find((member: Member) => member.username === username); // find the member with the username we want (take first instance)
      

    if (memberFromCache) {
      console.log("MemberService - Member from Cache:", memberFromCache);
      return of(memberFromCache); // if we have a value in the cache, so already seen this query, return it
    }
    
    console.log("memberservice memberfromcache:" + memberFromCache)

    return this.httpClient.get<Member>(this.baseUrl + 'users/' + username).pipe(
      tap(memberFromApi => {
        console.log("MemberService - Member from API:", memberFromApi);
      })
    );
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

  // add like to user
  likeAdd(username: string) {

    return this.httpClient.post(this.baseUrl + 'like/' + username, {}); // post so we have to sent something back (we send an empty object {})
  }

  likeGetter
    (predicate: string, pageNumber: number, pageSize: number) {

    // set up params 
    let parameters = getHeadPagination(pageNumber, pageSize);

    // add predicate to params
    parameters = parameters.append('predicate', predicate);

    return getResultPagination<Member[]>(this.baseUrl + 'like', parameters, this.httpClient);

  }


}
