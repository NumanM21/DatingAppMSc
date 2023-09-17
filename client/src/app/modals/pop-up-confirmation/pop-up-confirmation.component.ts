import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-pop-up-confirmation',
  templateUrl: './pop-up-confirmation.component.html',
  styleUrls: ['./pop-up-confirmation.component.css']
})
export class PopUpConfirmationComponent implements OnInit {
  title = '';
  message = '';
  btnOkText = '';
  btnCancelText = '';
  res: boolean = false;


  constructor(public refModalBs:BsModalRef) { }

  ngOnInit(): void {
  }


  
  confirmAction(){
    this.res = true;
    this.refModalBs.hide();
  }

  declineAction(){
    this.refModalBs.hide();
  }

}
