import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/Member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-display',
  templateUrl: './member-display.component.html',
  styleUrls: ['./member-display.component.css']
})
export class MemberDisplayComponent implements OnInit {
  @Input() member: Member | undefined;

  constructor(private toast : ToastrService ,private serviceMember: MembersService) { }

  ngOnInit(): void {
  }

  likeAdd(member: Member) {
    this.serviceMember.likeAdd(member.userName).subscribe({
      next: res => {
        this.toast.success(member.knownAs + " has been liked");
      }
    })
  }

}
