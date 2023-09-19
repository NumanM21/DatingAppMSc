export class Photo {
  id: number = 0; 
  photoURL: string = '';
  isMainPhoto: boolean = false;
  isPhotoApproved: boolean = false;
  username?: string;

  get Id(): number {
    return this.id;
  }
}



