import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { concatWith, take } from 'rxjs';
import { Member } from 'src/app/_models/Member';
import { User } from 'src/app/_models/User';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-editprofile',
  templateUrl: './member-editprofile.component.html',
  styleUrls: ['./member-editprofile.component.css']
})
export class MemberEditprofileComponent implements OnInit {
  @ViewChild('formEdit') formEdit: NgForm | undefined
  @HostListener('window:beforeunload', ['$event']) beforeUnloadNotification($event: any){
    if (this.formEdit?.dirty){
      $event.returnValue = true;
    }
  }
  member: Member | undefined;
  user: User | null = null;


  constructor(private toastrService: ToastrService, private serviceAccount: AccountService, private serviceMember: MembersService) {
    this.serviceAccount.currentUser$.pipe(take(1)).subscribe({
      next: user => this.user = user
      // user retrieved from the currentUser observable ($), we take the first instance (user) then subscribe so we can assign our user field to user we get back from account service
    })
  }

  ngOnInit(): void {
    this.memberLoad();
  }


  memberLoad() {
    if (this.user)
      this.serviceMember.getMember(this.user.username).subscribe({
        next: member => this.member = member
      })
    else return;
  }

  memberUpdate() {
    console.log(this.member);
    this.toastrService.success('Profile Update was Successful')
    this.formEdit?.reset(this.member);
  }



}
