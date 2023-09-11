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
}
