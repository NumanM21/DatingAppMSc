export interface Photo {
  id: number;
  photoURL: string;
  isMainPhoto: boolean;
  isPhotoApproved: boolean;
  username?: string;
}
