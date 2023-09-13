import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_models/User';
import { environment } from 'src/environments/environment';


@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private currentUserSource = new BehaviorSubject<User | null>(null);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private httpClient: HttpClient) { }

  login(model: any) {
    return this.httpClient.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          this.setCurrentUser(user);
        }
      })
    )
  }

  register(model: any) {
    return this.httpClient.post<User>(this.baseUrl + 'account/register', model).pipe(
      map(user => {
        if (user) {
          this.setCurrentUser(user);
        }
      })
    )
  }
  // Use this method to follow DRY principle (repeated code in login and register)
  setCurrentUser(user: User) {
    // need to check if we usea[] or single role for roles in user.ts
    user.roles = [];
    // .role (not roles!)
    const userRoles = this.TokenDecoded(user.token).role;
    Array.isArray(userRoles) ? user.roles = userRoles : user.roles.push(userRoles);


    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }

  // Get the token from the local storage -> to get our roles (from api --> API still 'protects' the data from security breach)!
  TokenDecoded(Token:string){
    // split by . and get the middle part (where our roles array is at, 0-indexed)

    return JSON.parse(atob(Token.split('.')[1])).role;
    // atob = decode Token from base64
  }
}
