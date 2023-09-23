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

    console.log('messageLoader called', this.pageNumber, this.pageSize, this.messageContainer);
    this.pageLoad = true;
    this.serviceMessage.messageGetter(this.pageNumber, this.pageSize, this.messageContainer).subscribe({
      next: res => {

        console.log('API Response:', res);
        this.message = res.result;
        this.pagination = res.pagination;
        this.pageLoad = false;


        // Update currentPage only after a successful API call (to avoid infinite loop!!!!!)
        if (this.pagination)
          this.pagination.currentPage = this.pageNumber;
      },
      error: (error) => {
        console.log('API Error:', error);
        this.pageLoad = false;
      }
    });
  }

  pageChanged(event: any) {

    console.log('pageChanged called', event.page);

    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.messageLoader();
    }
  }

  messageDelete(msgId: number) {
    this.serviceMessage.messageDelete(msgId).subscribe({

      next: _ => this.message?.splice(this.message.findIndex(x => x.messageId === msgId), 1)
      // remove the message from the array at specified index where the message id matches the id of the message we want to delete
    })
  }
}
