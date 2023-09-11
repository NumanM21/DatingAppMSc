import { Component, OnInit } from '@angular/core';
import { MessageUser } from '../_models/MessageUser';
import { Pagination } from '../_models/Pagination';
import { MessageService } from '../_services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  message: MessageUser[] | undefined;
  pagination: Pagination | undefined;
  messageContainer = 'Inbox';// default value (Unread / Sent )
  pageNumber = 1;
  pageSize = 3;

  constructor(private serviceMessage: MessageService) { }

  ngOnInit(): void {
    this.messageLoader(); // takes default parameters
  }

  messageLoader() {
    this.serviceMessage.messageGetter(this.pageNumber, this.pageSize, this.messageContainer).subscribe({
      next: res => {

        this.message = res.result;
        this.pagination = res.pagination;
  
        
      }
    });
  }

pageChanged(event: any){
  this.pageNumber = event.page;
  this.messageLoader();
}

}
