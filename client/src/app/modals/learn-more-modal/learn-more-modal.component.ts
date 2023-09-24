import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-learn-more-modal',
  templateUrl: './learn-more-modal.component.html',
  styleUrls: ['./learn-more-modal.component.css']
})
export class LearnMoreModalComponent implements OnInit {

  

  constructor(public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
  }

}
