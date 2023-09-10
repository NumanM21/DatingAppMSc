// class to pass parameter to getMembers method in member.service.ts
// Reduce the number of parameters we pass to a method + DRY principle

import { User } from "./User";

export class parameterUser{
  gender: string;
  minAge: number = 18;
  maxAge: number = 150;
  orderByActive = 'lastActive';
  pageNumber = 1;
  pageSize = 5;


  constructor (paramUser: User){
    //if user gender is female, request male 
    this.gender = paramUser.gender === 'female' ? 'male':'female'

  }
}

