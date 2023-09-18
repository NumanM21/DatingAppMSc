import { CommonModule } from '@angular/common';
import { Component, Directive, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { Member } from 'src/app/_models/Member';
import { MembersService } from 'src/app/_services/members.service';
import { MemberMessagetabComponent } from '../member-messagetab/member-messagetab.component';
import { MessageService } from 'src/app/_services/message.service';
import { MessageUser } from 'src/app/_models/MessageUser';
import { UserPresenceService } from 'src/app/_services/user-presence.service';
import { AccountService } from 'src/app/_services/account.service';
import { User } from 'src/app/_models/User';
import { take } from 'rxjs';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  standalone: true,
  imports: [CommonModule, TabsModule, GalleryModule, TimeagoModule, MemberMessagetabComponent] // import specific modules since this is now standalone component
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  // view child to get access to child component (member tab)
  @ViewChild('tabsMember', { static: true }) tabsMember: TabsetComponent | undefined;
  tabActive: TabDirective | undefined;
  member: Member = {} as Member; // initialize member to empty object (to avoid undefined error) (route resolver will populate this)
  message: MessageUser[] = [];
  currUser?: User;
  img: GalleryItem[] = [];


  constructor(private serviceMessage: MessageService, private serviceMember: MembersService, private activatedRoute: ActivatedRoute, public serviceUserPresence: UserPresenceService, private serviceAccount: AccountService) {
    this.serviceAccount.currentUser$.pipe(take(1)).subscribe({
      next: u => {
        if (u) this.currUser = u;
      }
    })
  }

  ngOnInit(): void {
    this.activatedRoute.data.subscribe({
      next: d => {
        this.member = d['member']
      }
    })

    // Using Angular routing (direct user to message tab if they click on a message button)

    // query params is an observable, so we need to subscribe to it
    this.activatedRoute.queryParams.subscribe({
      next: p => {
        // [tab] is key in our query params for messages
        p['tab'] && this.tabSelector(p['tab']);
      }
    })

    this.getImg() // will populate our img array (once we get our user)
  }

  getImg() {
    if (this.member)
      for (const photo of this.member.photos) {
        this.img.push(new ImageItem({ src: photo.photoURL, thumb: photo.photoURL }));
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

  // this is the method to stop the connection to the hub (when we leave the component)-> destroy the component
  ngOnDestroy(): void{ 
    this.serviceMessage.stopConnectionToHub();
  }

  tabActivated(tabDirective: TabDirective) {

    // set the tab active to the tab directive
    this.tabActive = tabDirective;

    // if the tab active is the messages tab, then we want to call the child component (member-messagetab) and call the loadMessages method (FIXES te double loading of messages)
    if (this.tabActive.heading == 'Messages') {
      // Start connection to hub -> to retrieve messages between users
      if (this.currUser && this.member)
      this.serviceMessage.startConnectionToHub(this.currUser, this.member.userName);

      else 
      this.serviceMessage.stopConnectionToHub();
    }
  }

  tabSelector(head: string) {
    // if the tab member is not null, then we want to find the tab with the heading of the head parameter and set it to active

    if (this.tabsMember) {
      this.tabsMember.tabs.find(x => x.heading == head)!.active = true;
    }
  }

}
