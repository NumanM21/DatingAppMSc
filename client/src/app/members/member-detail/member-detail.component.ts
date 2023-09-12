import { CommonModule } from '@angular/common';
import { Component, Directive, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsModule, TabsetComponent} from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { Member } from 'src/app/_models/Member';
import { MembersService } from 'src/app/_services/members.service';
import { MemberMessagetabComponent } from '../member-messagetab/member-messagetab.component';
import { MessageService } from 'src/app/_services/message.service';
import { MessageUser } from 'src/app/_models/MessageUser';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  standalone: true,
  imports: [CommonModule, TabsModule, GalleryModule, TimeagoModule, MemberMessagetabComponent] // import specific modules since this is now standalone component
})
export class MemberDetailComponent implements OnInit {
  // view child to get access to child component (member tab)
  @ViewChild('tabsMember') tabsMember: TabsetComponent | undefined;
  tabActive: TabDirective | undefined;
  member: Member | undefined;
  message: MessageUser[] = [];
  img: GalleryItem[] = [];

  constructor(private serviceMessage: MessageService ,private serviceMember: MembersService, private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.memberLoad();
  }

  memberLoad() {
    const username = this.activatedRoute.snapshot.paramMap.get('username');
    if (username)
      this.serviceMember.getMember(username).subscribe({
    next: member => {
      this.member = member,
      this.getImg() // will populate our img array (once we get our user)
    }
    })
    else return;
  }

  getImg(){
    if (this.member)
    for (const photo of this.member.photos){
      this.img.push(new ImageItem({src: photo.url, thumb: photo.url}));
    }
    else return;
  }

  messageLoader() {
    if (this.member?.userName) {
      this.serviceMessage.messageLoaderBetweenUsers(this.member.userName).subscribe({
        next: msg => this.message = msg,
      })
    }
  }

  tabActivated(tabDirective: TabDirective){

    // set the tab active to the tab directive
    this.tabActive = tabDirective; 

    // if the tab active is the messages tab, then we want to call the child component (member-messagetab) and call the loadMessages method (FIXES te double loading of messages)
    if (this.tabActive.heading == 'Messages'){
      this.messageLoader();
    }



  }

}
