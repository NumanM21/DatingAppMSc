import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_models/User';
import { environment } from 'src/environments/environment';
import { UserPresenceService } from './user-presence.service';


@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private currentUserSource = new BehaviorSubject<User | null>(null);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private serviceUserPresence: UserPresenceService, private httpClient: HttpClient) { }

  login(model: any) {
    return this.httpClient.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          this.setCurrentUser(user);
          console.log('User data after login:', user);
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

  // called when login, register and refresh token (from local storage)
  setCurrentUser(user: User) {
    // User role might be single or array of roles, we need to make sure it is always an array
    user.roles = [];

    const userRoles = this.TokenDecoded(user.token); // Directly get the roles without .role

    // console.log('Decoded roles:', userRoles); // This will print the decoded roles from the token

    Array.isArray(userRoles) ? user.roles = userRoles : user.roles.push(userRoles);

    // console.log('Final user object:', user); // This will print the final user object with roles

    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
    // console.log(this.currentUserSource);

    // create connection to hub (user presence)
    this.serviceUserPresence.CreateConnectionToHub(user);

  }


  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);

    // disconnect connection to hub (user presence)
    this.serviceUserPresence.DisconnectConnectionToHub();
  }


  // Get the token from the local storage -> to get our roles (from api --> API still 'protects' the data from security breach)!

  TokenDecoded(Token: string) {
    // split by . and get the middle part (where our roles array is at, 0-indexed)

    const decodedToken = JSON.parse(atob(Token.split('.')[1]));

    // console.log('Decoded token:', decodedToken); // This will print the entire decoded token

    const roles = decodedToken.role;

    // console.log('Roles from decoded token:', roles); // This will print the roles extracted from the decoded token

    return roles;

    // atob = decode Token from base64
  }

}
