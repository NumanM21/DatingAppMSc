import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/Member';
import { MembersService } from '../_services/members.service';
import { Pagination } from '../_models/Pagination';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  pageNumber = 1;
  pageSize = 3;
  pagination : Pagination | undefined;
  members: Member[] | undefined;
  predicate = 'liked'; // default value


  constructor(private serviceMember: MembersService) { }

  ngOnInit(): void {
    this.likesLoader();
  }

  likesLoader() {
    this.serviceMember.likeGetter(this.predicate, this.pageNumber, this.pageSize).subscribe({
      next: res => {
        this.members = res.result;
        this.pagination = res.pagination;
      }
    })
  }

  pageChanged(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.likesLoader();
    }
  }

}
