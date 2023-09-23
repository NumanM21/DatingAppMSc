import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Subject, concatWith, take, takeUntil } from 'rxjs';
import { Member } from 'src/app/_models/Member';
import { User } from "src/app/_models/User";
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-member-editprofile',
  templateUrl: './member-editprofile.component.html',
  styleUrls: ['./member-editprofile.component.css']
})
export class MemberEditprofileComponent implements OnInit {
  @ViewChild('formEdit') formEdit: NgForm | undefined
  @HostListener('window:beforeunload', ['$event']) beforeUnloadNotification($event: any) {
    if (this.formEdit?.dirty) {
      $event.returnValue = true;
    }
  }
  private unsubscribe$ = new Subject<void>();
  member: Member | undefined;
  user: User | null = null;
  // changeDetectorRef: ChangeDetectorRef;


  constructor(private toastrService: ToastrService, private serviceAccount: AccountService, private serviceMember: MembersService, changeDetectorRef: ChangeDetectorRef) {
    // this.changeDetectorRef = changeDetectorRef;
    this.serviceAccount.currentUser$.pipe(take(1)).subscribe({
      next: user => this.user = user
      // user retrieved from the currentUser observable ($), we take the first instance (user) then subscribe so we can assign our user field to user we get back from account service
    })
  }

   ngOnInit(): void {
    this.memberLoad();

    // Subscribe to member$ observable 
    this.serviceMember.member$
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: member => {
          if (member) {
            this.member = member;
          }
        }
      });
  }

  ngOnDestroy(): void {
    // Emits a value to complete the observable subscription
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  memberLoad() {
    if (this.user) {
      this.serviceMember.getMember(this.user.username).subscribe(member => {
        this.member = member;

        if (this.member) this.serviceMember.updateMemberState(this.member);

        console.log('MemberEditProfile - Member:', this.member);
        // this.changeDetectorRef.detectChanges();
      });
    }
  }



  memberUpdate() {
    this.serviceMember.updateMember(this.formEdit?.value).subscribe({
      // Get nothing from API, but still have to subscribe since we get an observable returned
      next: () => {
        this.toastrService.success('Profile Update was Successful')
        this.formEdit?.reset(this.member);


        if (this.member) this.serviceMember.updateMemberState(this.member);
      }
    })
  }


}
