import { TestBed } from '@angular/core/testing';

import { PopupConfirmationService } from './popup-confirmation.service';

describe('PopupConfirmationService', () => {
  let service: PopupConfirmationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PopupConfirmationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
