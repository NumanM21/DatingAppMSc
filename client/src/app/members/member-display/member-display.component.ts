import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { Member } from 'src/app/_models/Member';

@Component({
  selector: 'app-member-display',
  templateUrl: './member-display.component.html',
  styleUrls: ['./member-display.component.css']
})
export class MemberDisplayComponent implements OnInit {
  @Input() member: Member | undefined;

  constructor() { }

  ngOnInit(): void {
  }

}
