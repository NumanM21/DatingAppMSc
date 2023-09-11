import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { TimeagoModule, TimeagoPipe } from 'ngx-timeago';
import { MessageUser } from 'src/app/_models/MessageUser';
import { MessageService } from 'src/app/_services/message.service';

// Will be a child component of member-detail
@Component({
  selector: 'app-member-messagetab',
  templateUrl: './member-messagetab.component.html',
  styleUrls: ['./member-messagetab.component.css'],
  standalone: true,
  imports: [CommonModule, TimeagoModule]
})
export class MemberMessagetabComponent implements OnInit {
  @Input() username: string | undefined;
  message: MessageUser[] = [];

  constructor(private serviceMessage: MessageService) { }

  ngOnInit(): void {
    this.messageLoader();
  }

  messageLoader() {
    if (this.username) {
      this.serviceMessage.messageLoaderBetweenUsers(this.username).subscribe({
        next: msg => this.message = msg,
      })
    }
  }

}
