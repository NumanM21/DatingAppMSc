import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabsModule} from 'ngx-bootstrap/tabs';
import { Member } from 'src/app/_models/Member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  standalone: true,
  imports: [CommonModule, TabsModule, GalleryModule] // import specific modules since this is now standalone component
})
export class MemberDetailComponent implements OnInit {
  member: Member | undefined;
  img: GalleryItem[] = [];

  constructor(private serviceMember: MembersService, private activatedRoute: ActivatedRoute) { }

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

}
