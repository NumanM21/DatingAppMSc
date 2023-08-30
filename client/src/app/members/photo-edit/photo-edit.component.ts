import { Component, Input, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/Member';
import { Photo } from 'src/app/_models/Photo';

@Component({
  selector: 'app-photo-edit',
  templateUrl: './photo-edit.component.html',
  styleUrls: ['./photo-edit.component.css']
})
export class PhotoEditComponent implements OnInit {
  @Input() member: Member | undefined;
  //FIXME: Adding hovered property on the fly -> May need to change/ remove this in future
  hoveredStates = new Map<number, boolean>();

  constructor() { }

  ngOnInit(): void {
  }





  onHover(photo: Photo) {
    this.hoveredStates.set(photo.id, true);
  }

  hoverOut(photo: Photo) {
    this.hoveredStates.set(photo.id, false);
  }




}
