import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { getHeadPagination, getResultPagination } from './HelperPagination';
import { MessageUser } from '../_models/MessageUser';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from '../_models/User';
import { BehaviorSubject, take } from 'rxjs';
import { group } from '@angular/animations';
import { SignalRGroup } from '../_models/SignalRGroup';
import { LoadingService } from './loading.service';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  private msgBetweenUsers = new BehaviorSubject<MessageUser[]>([]);
  msgBetweenUsers$ = this.msgBetweenUsers.asObservable(); // this is the observable to subscribe to
  hubURL = environment.hubUrl; // this is the URL for the SignalR hub
  baseURL = environment.apiUrl;
  private connectionToHub?: HubConnection; // This is the connection to the SignalR hub

  constructor(private httpClient: HttpClient, private serviceLoading: LoadingService) { }



  messageGetter(pageNumber: number, pageSize: number, messageContainer: string) {

    // Log the parameters being used to construct the query string
    console.log('Query Parameters:', { pageNumber, pageSize, messageContainer });

    // get the parameters for the query string
    let parameter = getHeadPagination(pageNumber, pageSize);

    // append parameters for the container
    parameter = parameter.append('messageContainer', messageContainer);

    //FIXME: For debugging
     // Construct the query string
    //  const url = `https://localhost:5001/api/messageUser?pageNumber=${pageNumber}&pageSize=${pageSize}&messageContainer=${messageContainer}`;

    //  // Log the constructed URL
    //  console.log('Constructed URL:', url);

    // return the result of the query
    return getResultPagination<MessageUser[]>(this.baseURL + 'messageUser', parameter, this.httpClient);
    // messageUser is same API endpoint as in the backend!!! 
  }

  messageLoaderBetweenUsers(username: string) {
    return this.httpClient.get<MessageUser[]>(this.baseURL + 'messageUser/message-between-users/' + username);
  }

  async messageSender(username: string, msgContent: string) {
    // Use our signalR hub to send the message -> invoke same method as the one in the backend (UsersMessageHub)
    return this.connectionToHub?.invoke('MessageSender', { messageReceivingUsername: username, messageContent: msgContent }) // return promise (using async forces a promise)
      .catch(err => console.log(err));
  }

  messageDelete(msgId: number) {
    return this.httpClient.delete(this.baseURL + 'messageUser/' + msgId);
  }

  // this is the method to start the connection to the hub
  startConnectionToHub(currUser: User, otherUser: string) {

    this.serviceLoading.requestCount();


    // same query string as in the backend (in our API to get the other user's messages)
    this.connectionToHub = new HubConnectionBuilder().withUrl(this.hubURL + 'users-message?user=' + otherUser, {
      // need to authenticate the user to the hub

      accessTokenFactory: () => currUser.token

    }).withAutomaticReconnect()
      .build(); // this will automatically reconnect if the connection is lost

    // start the connection
    this.connectionToHub.start().catch(err => console.log(err))
    .finally(() => this.serviceLoading.idle()); //get loading spinner when we initially load the messages

    // Create all hub connection methods here (from UsersMessageHub)

    // this is the method to receive the message from the hub
    this.connectionToHub.on('LoadMessageBetweenUsers', msg => {
      // observable to receive the message (we can subscribe to this observable in the component)
      this.msgBetweenUsers.next(msg);

    })

    // hub method to send new message to the other user
    this.connectionToHub.on('SendNewMessage', msg => {
      // want to update the behavior subject with the new message -> use observable to get current array, then update the array with the new message
      this.msgBetweenUsers$.pipe(take(1)).subscribe({
        next: msgArray => {
          // spread operator to get the current array, then add the new message to the array -> doesn't mutate the array, but creates a new array with msg added
          this.msgBetweenUsers.next([...msgArray, msg])
        }
      })
    })


    // hub method to update group 
    this.connectionToHub.on('GroupUpdate', (group: SignalRGroup) => {
      // once user joins, check if user has unread msg --> we mark as read (since they join group once they click on the message)

      // otherUser is user joining group 
      if (group.connectionsInGroup.some(s => s.username == otherUser)) {
        this.msgBetweenUsers$.pipe(take(1)).subscribe({
          next: msg => {
            msg.forEach(m => {
              // if message is not read, its null
              if (!m.messageReadAt) {
                m.messageReadAt = new Date(Date.now());
              }
            })
            // spread to update array of messages with new readAt date 
            this.msgBetweenUsers.next([...msg]);
          }
        })
      }
    })
  }


  // this is the method to stop the connection to the hub
  stopConnectionToHub() {
    // if we stop connection without having a connection -> will cause a crash
    if (this.connectionToHub)
      this.msgBetweenUsers.next([]); // clear the messages when we stop the connection 
      this.connectionToHub?.stop().catch(err => console.log(err));
  }

}
