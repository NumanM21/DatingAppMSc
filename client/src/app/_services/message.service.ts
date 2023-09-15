 import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { getHeadPagination, getResultPagination } from './HelperPagination';
import { MessageUser } from '../_models/MessageUser';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from '../_models/User';
import { BehaviorSubject, take } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  private msgBetweenUsers = new BehaviorSubject<MessageUser[]>([]); 
  msgBetweenUsers$ = this.msgBetweenUsers.asObservable(); // this is the observable to subscribe to
  hubURL = environment.hubUrl; // this is the URL for the SignalR hub
  baseURL = environment.apiUrl;
  private connectionToHub?: HubConnection; // This is the connection to the SignalR hub

  constructor(private httpClient: HttpClient) { }

  

  messageGetter(pageNumber:number, pageSize:number, messageContainer:string){

    // get the parameters for the query string
    let parameter = getHeadPagination(pageNumber, pageSize);

    // append parameters for the container
    parameter = parameter.append('messageContainer', messageContainer);

    // return the result of the query
    return getResultPagination<MessageUser[]>(this.baseURL + 'messageUser', parameter, this.httpClient);
    // messageUser is same API endpoint as in the backend!!! 
  }

  messageLoaderBetweenUsers(username:string){
    return this.httpClient.get<MessageUser[]>(this.baseURL + 'messageUser/message-between-users/' + username);
  }

  async messageSender( username: string, msgContent: string){
    // Use our signalR hub to send the message -> invoke same method as the one in the backend (UsersMessageHub)
    return this.connectionToHub?.invoke('MessageSender', {messageReceivingUsername: username, messageContent: msgContent}) // return promise (using async forces a promise)
    .catch(err => console.log(err)); 
  }

  messageDelete(msgId:number){
    return this.httpClient.delete(this.baseURL + 'messageUser/' + msgId);
  }

  // this is the method to start the connection to the hub
  startConnectionToHub(currUser: User, otherUser:string){
    // same query string as in the backend (in our API to get the other user's messages)
    this.connectionToHub = new HubConnectionBuilder().withUrl(this.hubURL+ 'users-message?user=' +  otherUser, {
      // need to authenticate the user to the hub

      accessTokenFactory: () => currUser.token

    }).withAutomaticReconnect()
    .build(); // this will automatically reconnect if the connection is lost

    // start the connection
    this.connectionToHub.start().catch(err => console.log(err));

    // Create all hub connection methods here (from UsersMessageHub)

    // this is the method to receive the message from the hub
    this.connectionToHub.on('LoadMessageBetweenUsers', msg => {
      // observable to receive the message (we can subscribe to this observable in the component)
      this.msgBetweenUsers.next(msg);

    })

    // hub method to send new message to the other user
    this.connectionToHub.on('SendNewMessage', msg =>{
      // want to update the behavior subject with the new message -> use observable to get current array, then update the array with the new message
      this.msgBetweenUsers$.pipe(take(1)).subscribe({
        next: msgArray =>{
          // spread operator to get the current array, then add the new message to the array -> doesn't mutate the array, but creates a new array with msg added
          this.msgBetweenUsers.next([...msgArray, msg])
        }
      })

    }) 


  }


  // this is the method to stop the connection to the hub
  stopConnectionToHub(){
    // if we stop connection without having a connection -> will cause a crash
    if (this.connectionToHub)

    this.connectionToHub?.stop().catch(err => console.log(err));
  }

}
