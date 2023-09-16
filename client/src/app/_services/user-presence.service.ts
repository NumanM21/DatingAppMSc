import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { User } from '../_models/User';
import { BehaviorSubject, take } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class UserPresenceService { // service to connect to web hub -> Listen to two events: UserIsOnline and UserIsOffline (in UserPresenceHub.cs)
  hubURL = environment.hubUrl; // URL to web hub
  private connectionToHub?: HubConnection; // connection to web hub 
  private sourceUsersOnline = new BehaviorSubject<string[]>([]); // array of users online 
  usersOnline$ = this.sourceUsersOnline.asObservable(); // observable for components to subscribe to track users online



  constructor(private route: Router, private serviceToast: ToastrService) { }

  // create connection
  CreateConnectionToHub(user: User) {
    this.connectionToHub = new HubConnectionBuilder().withUrl(this.hubURL + 'user-presence', {
      // pass user token
      accessTokenFactory: () => user.token
    })
      .withAutomaticReconnect() // reconnect if connection is lost 
      .build(); // build connection


    // built connection ^, now need to START connection
    this.connectionToHub.start().catch(err => console.log(err)); // .start() returns a promise, so we can use .catch() to catch any errors

    // listen to events (User online)
    this.connectionToHub.on('OnlineUser', username => {
      this.usersOnline$.pipe(take(1)).subscribe({
        // observale contains array of users online, so we need to add new user to array -> Use spread operator to add new user to array (returns new array with new user added)
        next: currUsersOnline => {
          this.sourceUsersOnline.next([...currUsersOnline, username])
        }
      })
    })

    // listen to events (User offline)
    this.connectionToHub.on('OfflineUser', username => {
      this.usersOnline$.pipe(take(1)).subscribe({
        next: currUsersOnline =>{
          this.sourceUsersOnline.next(currUsersOnline.filter(f=> f !== username)) // .next to return new array and filter returns the array without the user that went offline)
        }
      })
    })

    // listen to events (Get users currently online)
    this.connectionToHub.on('GetUsersCurrentlyOnline', usernames => this.sourceUsersOnline.next(usernames)); // update users online array using .next() method


    // listen to events (Get users currently online)
    this.connectionToHub.on("ReceiveNewMessage", ({ username, knownAs }) => {
      // console.log(username); // Debugging
      
      this.serviceToast.info('You have received a new message from ' + knownAs + '! Click to view message').onTap.pipe(take(1)).subscribe({
        next: () => {
          // redirect to sender user message page
          this.route.navigateByUrl('/members/' + username + '?tab=Messages')
        }
      })
    })

  }

  // stop connection
  DisconnectConnectionToHub() {
    this.connectionToHub?.stop().catch(err => console.log(err));
  }


}
