import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { User } from '../_models/User';

@Injectable({
  providedIn: 'root'
})
export class UserPresenceService { // service to connect to web hub -> Listen to two events: UserIsOnline and UserIsOffline (in UserPresenceHub.cs)
  hubURL = environment.hubUrl; // URL to web hub
  private connectionToHub?: HubConnection; // connection to web hub 


  constructor(private serviceToast: ToastrService) { }

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
        // display toast message
        this.serviceToast.info(username + ' is now online');
      })

      // listen to events (User offline)
      this.connectionToHub.on('OfflineUser', username =>{
        this.serviceToast.warning(username + ' is now offline');
      })
  }

  // stop connection
  DisconnectConnectionToHub(){
    this.connectionToHub?.stop().catch(err => console.log(err));
  }


}
