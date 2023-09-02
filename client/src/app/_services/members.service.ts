import { HttpClient, HttpHeaderResponse, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/Member';
import { MemberEditprofileComponent } from '../members/member-editprofile/member-editprofile.component';
import { map, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  // Service remains for lifetime of application, components distroyed and re-built, so can store memberLIST HERE (in service) once memberlist component created
  //FIXME: Delete  later 

  members: Member[] = []
  baseUrl = environment.apiUrl;

  constructor(private httpClient: HttpClient) { }

  getMembers() {
    if (this.members.length > 0) return of(this.members); //  'of' is rxjs to allow us to return an obervable (can't directly return members)
    // Don't have members, we request member list from API then .pipe (rxjs again)
    return this.httpClient.get<Member[]>(this.baseUrl + 'users').pipe(
      map(member => {
        this.members = member;
        return member;
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

}
