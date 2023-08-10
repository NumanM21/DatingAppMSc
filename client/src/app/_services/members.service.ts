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
    return this.httpClient.get<Member[]>(this.baseUrl + 'users')

  }

  getMember(username: string)
  {
    return this.httpClient.get<Member>(this.baseUrl + 'users/' + username)
  }


}
