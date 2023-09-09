import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { Observable } from 'rxjs';
import { Member } from 'src/app/_models/Member';
import { Pagination } from 'src/app/_models/Pagination';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  // members$: Observable<Member[]> | undefined;
  members: Member[] = []; // this is the array of members we want to display
  pagination: Pagination | undefined;
  pageNumber = 1;
  pageSize = 5;

  constructor(private memberService: MembersService) { }

  ngOnInit(): void {
    this.membersLoad();
    // this.members$ = this.memberService.getMembers();
  }

  membersLoad() {
    this.memberService.getMembers(this.pageNumber, this.pageSize).subscribe({
      // response we get from our member service is the result paginated class populated
      next: response => {
        if (response.result && response.pagination) {
          this.members = response.result;
          this.pagination = response.pagination;
        }
      }
    })
  }

  // event is the page changed event

  pageChanged(event: any) {
    this.pageNumber = event.page;
    this.membersLoad();
  }


}
