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
    this.PhotosForApprovalGetter();
  }

  // Getting photos for approval
  PhotosForApprovalGetter() {
    this.serviceAdmin.PhotosForApprovalGetter().subscribe({

      // get all user photos that are not approved and assign them to photosToApprove array
      next: p => {
        this.photosToApprove = p.map(photo => Object.assign(new Photo(), photo));
        console.log("Photos to approve:", this.photosToApprove);
      },
      error: err => console.error('Error fetching photos:', err)
    })
  }

  // Approving photos
  PhotoApprover(Id: number) {
    this.serviceAdmin.PhotoApprover(Id).subscribe({

      // once approved, remove (splice) photo from array
      next: () => {
        console.log(Id);
        // find index of photo in array which matches the photoId
        this.photosToApprove.splice(this.photosToApprove.findIndex(p => p.Id === Id), 1)
      },
      error: err => console.error('Error fetching photos:', err)
    })
  }

  // Rejecting photos
  PhotoUnapproved(Id: number) {
    this.serviceAdmin.PhotoUnapproved(Id).subscribe({
      // same as approved except for rejected photos
      next: () => {
        console.log(Id);
        this.photosToApprove.splice(this.photosToApprove.findIndex(p => p.Id === Id), 1)
      },
      error: err => console.error('Error fetching photos:', err , Id)
    })
  }



}
