import { Component, OnInit } from '@angular/core';
import { Photo } from 'src/app/_models/Photo';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-manage-photo',
  templateUrl: './manage-photo.component.html',
  styleUrls: ['./manage-photo.component.css']
})
export class ManagePhotoComponent implements OnInit {
  photosToApprove: Photo[] = [];

  constructor(private serviceAdmin: AdminService) { }

  ngOnInit(): void {
  }

  // Getting photos for approval
  PhotosForApprovalGetter(){
    this.serviceAdmin.PhotosForApprovalGetter().subscribe({

      // get all user photos that are not approved and assign them to photosToApprove array
      next: p=>this.photosToApprove = p
    })
  }

  // Approving photos
  PhotoApprover(photoId: number){
    this.serviceAdmin.PhotoApprover(photoId).subscribe({

      // once approved, remove (splice) photo from array
      next: ()=>{

        // find index of photo in array which matches the photoId
        this.photosToApprove.splice(this.photosToApprove.findIndex(p=>p.id === photoId),1)
      }
    })
  }

  // Rejecting photos
  PhotoUnapproved(photoId: number){
    this.serviceAdmin.PhotoUnapproved(photoId).subscribe({
      // same as approved except for rejected photos
      next: () =>{
        this.photosToApprove.splice(this.photosToApprove.findIndex(p=>p.id === photoId),1)
      }
    })
  }



}
