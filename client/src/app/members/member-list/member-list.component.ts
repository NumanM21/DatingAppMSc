import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { Observable, take } from 'rxjs';
import { Member } from 'src/app/_models/Member';
import { Pagination, ResultPaginated } from 'src/app/_models/Pagination';
import { User } from "src/app/_models/User";
import { parameterUser } from 'src/app/_models/parameterUser';
import { AccountService } from 'src/app/_services/account.service';
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
  user: User | undefined;
  parameterUser: parameterUser | undefined;


  constructor(private serviceAccount: AccountService, private memberService: MembersService) {
    this.serviceAccount.currentUser$.pipe(take(1)).subscribe({
      // user is the currUser we get from our account service
      next: user => {
        if (user) {
          this.parameterUser = new parameterUser(user);
          this.user = user;
        }
      }
    })
  }

  ngOnInit(): void {
    this.membersLoad();
  }

  membersLoad() {
    if (this.parameterUser) {
      this.memberService.getMembers(this.parameterUser).subscribe({
        // response we get from our member service is the result paginated class populated
        next: response => {
          if (response.result && response.pagination) {
            this.members = response.result;
            this.pagination = response.pagination;
          }
        }
      })
    } else return; // if parameterUser is undefined, return
  }


  // event is the page changed event

  pageChanged(event: any) {
    if (this.parameterUser && this.parameterUser?.pageNumber !== event.page) {
      this.parameterUser.pageNumber = event.page;
      this.membersLoad();
    }
  }
}
