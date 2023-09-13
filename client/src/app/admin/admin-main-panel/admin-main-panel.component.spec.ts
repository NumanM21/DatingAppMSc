import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminMainPanelComponent } from './admin-main-panel.component';

describe('AdminMainPanelComponent', () => {
  let component: AdminMainPanelComponent;
  let fixture: ComponentFixture<AdminMainPanelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AdminMainPanelComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminMainPanelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
