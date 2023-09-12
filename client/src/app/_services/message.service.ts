import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { getHeadPagination, getResultPagination } from './HelperPagination';
import { MessageUser } from '../_models/MessageUser';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseURL = environment.apiUrl;

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

  messageSender( username: string, msgContent: string){
    // should match what our API is expecting to receive in the create messageDTO 
    return this.httpClient.post<MessageUser>(this.baseURL+ 'messageUser', {messageReceivingUsername: username, messageContent: msgContent})
  }

  messageDelete(msgId:number){
    return this.httpClient.delete(this.baseURL + 'messageUser/' + msgId);
  }
}
