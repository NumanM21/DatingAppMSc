import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/Member';
import { Photo } from 'src/app/_models/Photo';
import { User } from 'src/app/_models/User';
import { AccountService } from 'src/app/_services/account.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-photo-edit',
  templateUrl: './photo-edit.component.html',
  styleUrls: ['./photo-edit.component.css']
})
export class PhotoEditComponent implements OnInit {
  @Input() member: Member | undefined;
  //FIXME: Adding hovered property on the fly -> May need to change/ remove this in future
  hoveredStates = new Map<number, boolean>();
  uploader: FileUploader | undefined;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  user: User | undefined;
  // Input properties for select single of multiple files
  selectedFileName: string = ''; // single
  selectedFiles: File[] = []; // multiple


  constructor(private serviceAccount: AccountService) {
    // Sets our user property
    this.serviceAccount.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user)
          this.user = user;
      }
    })
  }

  ngOnInit(): void {
    this.initializeUploader();
  }

  //To get dropzone functionality
  fileOverBase(event: any) {
    this.hasBaseDropZoneOver = event;
  }

  // initialize the file uploader
  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo',
      authToken: 'Bearer ' + this.user?.token,
      isHTML5: true,
      allowedFileType: ['image'], // allows all image types TODO: Can add one for videos (something else we wanted to add)'
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024 // max file size cloudinary allows
    });
    // (file) -> Passing the file to the event method
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false
    }

    //once files uploaded success
    this.uploader.onSuccessItem = (item, response, status, header) => {
      if (response) { // see if we have a response
        const photo = JSON.parse(response);
        this.member?.photos.push(photo); // push photo we get from members API
      }
    }
  }


  // Select single or multiple files (displayed uploaded file name UNDER the btn)
  onFileSelected(event: any): void {
    if (event.target.files && event.target.files[0]) {
      this.selectedFileName = event.target.files[0].name
    }
  }

  
  onMultipleFilesSelected(event: any): void {
    this.selectedFiles = Array.from(event.target.files);
    console.log('Native event.target.files:', this.selectedFiles); // Debugging

    // If you have an upload queue, add the files to it
    for (let i = 0; i < this.selectedFiles.length; i++) {
       this.uploader?.addToQueue([this.selectedFiles[i]]);
    }
}





  // Img hover when user goes over the 'main' button
  onHover(photo: Photo) {
    this.hoveredStates.set(photo.id, true);
  }

  hoverOut(photo: Photo) {
    this.hoveredStates.set(photo.id, false);
  }




}
