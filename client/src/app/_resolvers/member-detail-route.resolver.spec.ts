import { TestBed } from '@angular/core/testing';

import { MemberDetailRouteResolver } from './member-detail-route.resolver';

describe('MemberDetailRouteResolver', () => {
  let resolver: MemberDetailRouteResolver;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    resolver = TestBed.inject(MemberDetailRouteResolver);
  });

  it('should be created', () => {
    expect(resolver).toBeTruthy();
  });
});
