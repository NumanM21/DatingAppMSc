import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Member } from 'src/app/_models/Member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  member: Member | undefined;

  constructor(private serviceMember: MembersService, private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.memberLoad();
  }

  memberLoad() {
    const username = this.activatedRoute.snapshot.paramMap.get('username');
    if (username)
      this.serviceMember.getMember(username).subscribe({
    next: member => this.member = member})
    else return;
  }

}
