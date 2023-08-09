import { Photo } from "./Photo"

export interface Member {
  id: number
  userName: string
  age: number
  knownAs: string
  userCreated: string
  lastActive: string
  userGender: string
  introduction: string
  lookingFor: string
  userInterests: string
  city: string
  country: string
  photoUrl: string
  photos: Photo[]
}

