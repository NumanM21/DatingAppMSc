import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { TimeagoModule, TimeagoPipe } from 'ngx-timeago';
import { MessageUser } from 'src/app/_models/MessageUser';
import { MessageService } from 'src/app/_services/message.service';

// Will be a child component of member-detail
@Component({
  selector: 'app-member-messagetab',
  templateUrl: './member-messagetab.component.html',
  styleUrls: ['./member-messagetab.component.css'],
  standalone: true,
  imports: [CommonModule, TimeagoModule, FormsModule]
})
export class MemberMessagetabComponent implements OnInit {
  msgContent = '';
  @Input() message: MessageUser[] = [];
  @Input() username: string | undefined;
  @ViewChild('msgForm') msgForm?: NgForm;
  

  constructor(private serviceMessage: MessageService) { }

  ngOnInit(): void {
  }

  messageSend(){
    if (!this.username) return;
    this.serviceMessage.messageSender(this.username, this.msgContent).subscribe({
      // push our message response we get back into our message
      next: messageResponse => {
        this.message.push(messageResponse)
      
        // Need to reset the form after we send the message
        this.msgForm?.reset();
        
      }

    })
  }
  

}
