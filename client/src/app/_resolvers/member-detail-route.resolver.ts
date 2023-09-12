import { Injectable, inject } from '@angular/core';
import {
  Router, ResolveFn,
  RouterStateSnapshot,
  ActivatedRouteSnapshot
} from '@angular/router';
import { Observable, of } from 'rxjs';
import { Member } from '../_models/Member';
import { MembersService } from '../_services/members.service';

//Route resolver -> will load the member data as the route is being activated. Passed onto the components (before components get constructed and loaded)


export const MemberDetailRouteResolver: ResolveFn<Member> = (resolverRoute) => {
  const serviceMember = inject(MembersService);
  return serviceMember.getMember(resolverRoute.paramMap.get('username')!)
  
  };

