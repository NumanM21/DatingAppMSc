export interface Photo {
  id: number;
  url: string;
  isMainPhoto: boolean;
  isPhotoApproved: boolean;
  username?: string;
}
