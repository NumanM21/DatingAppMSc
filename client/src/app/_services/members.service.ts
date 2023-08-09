import { HttpClient, HttpHeaderResponse, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/Member';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;

  constructor(private httpClient: HttpClient) { }

  getMembers()
  {
    return this.httpClient.get<Member[]>(this.baseUrl + 'users', this.getHttpAuthOptions())

  }

  getMember(username: string)
  {
    return this.httpClient.get<Member>(this.baseUrl + 'users/' + username, this.getHttpAuthOptions())
  }

  // Getting our token to pass with user request
  getHttpAuthOptions()
  {
    const userStr = localStorage.getItem('user'); // user is the key

    if (!userStr) return; // Shouldn't be possible, since MemberService requires user to already be authenticated (for typescript 'checking')

    const user = JSON.parse(userStr);

    return {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + user.token // need space after bearer to separate token
      })
    }

  }

  

}
