import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/Member';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  members: Member[] | undefined;
  predicate = 'liked'; // default value


  constructor(private serviceMember: MembersService) { }

  ngOnInit(): void {
    this.likesLoader();
  }

  likesLoader() {
    this.serviceMember.likeGetter(this.predicate).subscribe({
      next: res => {
        this.members = res;
      }
    })
  }

}
