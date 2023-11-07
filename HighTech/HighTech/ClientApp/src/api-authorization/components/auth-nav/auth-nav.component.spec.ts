import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { AuthNavComponent } from './auth-nav.component';
import { AuthorizeService } from '../../services/authorize-facade.service';
import { of } from 'rxjs';

describe('LoginMenuComponent', () => {
  let component: AuthNavComponent;
  let fixture: ComponentFixture<AuthNavComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [RouterTestingModule], 
      declarations: [ AuthNavComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    let authService = TestBed.get(AuthorizeService);

    spyOn(authService, 'ensureUserManagerInitialized').and.returnValue(
      Promise.resolve());
    spyOn(authService, 'getUserFromStorage').and.returnValue(
      of(null));

    fixture = TestBed.createComponent(AuthNavComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
