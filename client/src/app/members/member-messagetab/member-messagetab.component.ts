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
  @Input() message: MessageUser[] = [];
  @Input() username: string | undefined;
  

  constructor(private serviceMessage: MessageService) { }

  ngOnInit(): void {
  }

  

}
