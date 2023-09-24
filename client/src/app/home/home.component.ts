import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { LearnMoreModalComponent } from '../modals/learn-more-modal/learn-more-modal.component';


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode = false;
  users:any;
  bsModalRef: BsModalRef | undefined;

  constructor(private modalService: BsModalService) { }

  ngOnInit(): void {
  }

  registerToggle()
  {
    this.registerMode = !this.registerMode;
  }




  cancelRegisterMode(event:boolean)
  {
    this.registerMode = event;
  }


  openLearnMoreModal() {
    this.bsModalRef = this.modalService.show(LearnMoreModalComponent);
  }

}
