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
  pageLoad = false; // Hide saved icons until data is loaded from server(api)

  constructor(private serviceMessage: MessageService) { }

  ngOnInit(): void {
    this.messageLoader(); // takes default parameters
  }

  messageLoader() {
    this.pageLoad = true; // flag set to true
    this.serviceMessage.messageGetter(this.pageNumber, this.pageSize, this.messageContainer).subscribe({
      next: res => {

        this.message = res.result;
        this.pagination = res.pagination;
        this.pageLoad = false; // flag set to false

      }
    });
  }

  pageChanged(event: any) {
    this.pageNumber = event.page;
    this.messageLoader();
  }

  messageDelete(msgId: number) {
    this.serviceMessage.messageDelete(msgId).subscribe({

      next: _ => this.message?.splice(this.message.findIndex(x => x.messageId === msgId), 1) 
      // remove the message from the array at specified index where the message id matches the id of the message we want to delete
    })
  }
}
