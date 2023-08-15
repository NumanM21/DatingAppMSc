import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MemberEditprofileComponent } from './member-editprofile.component';

describe('MemberEditprofileComponent', () => {
  let component: MemberEditprofileComponent;
  let fixture: ComponentFixture<MemberEditprofileComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MemberEditprofileComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MemberEditprofileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
