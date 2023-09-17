import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import{HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { SharedModule } from './_modules/shared.module';
import { TestErrorComponent } from './errors/test-error/test-error.component';
import { ErrorInterceptor } from './_interceptors/error.interceptor';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { MemberDisplayComponent } from './members/member-display/member-display.component';
import { JwtInterceptor } from './_interceptors/jwt.interceptor';
import { MemberEditprofileComponent } from './members/member-editprofile/member-editprofile.component';
import { LoadingInterceptor } from './_interceptors/loading.interceptor';
import { PhotoEditComponent } from './members/photo-edit/photo-edit.component';
import { InputTextComponent } from './_forms/input-text/input-text.component';
import { PickDateComponent } from './_forms/pick-date/pick-date.component';
import { MemberMessagetabComponent } from './members/member-messagetab/member-messagetab.component';
import { AdminMainPanelComponent } from './admin/admin-main-panel/admin-main-panel.component';
import { RoleOfUserDirective } from './_directives/role-of-user.directive';
import { ManageUserComponent } from './admin/manage-user/manage-user.component';
import { ManagePhotoComponent } from './admin/manage-photo/manage-photo.component';
import { ModalForRolesComponent } from './modal/modal-for-roles/modal-for-roles.component';
import { RouteReuseStrategy } from '@angular/router';
import { ReusingRoute } from './_services/ReusingRoutes';
import { PopUpConfirmationComponent } from './modals/pop-up-confirmation/pop-up-confirmation.component';







@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    HomeComponent,
    RegisterComponent,
    MemberListComponent,
    ListsComponent,
    MessagesComponent,
    TestErrorComponent,
    NotFoundComponent,
    ServerErrorComponent,
    MemberDisplayComponent,
    MemberEditprofileComponent,
    PhotoEditComponent,
    InputTextComponent,
    PickDateComponent,
    AdminMainPanelComponent,
    RoleOfUserDirective,
    ManageUserComponent,
    ManagePhotoComponent,
    ModalForRolesComponent,
    PopUpConfirmationComponent,
  ],
  imports: [
    BrowserAnimationsModule,
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    SharedModule,
    ReactiveFormsModule
  ],
  providers: [
    {provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true},
    {provide: RouteReuseStrategy, useClass: ReusingRoute} 
    ],
  bootstrap: [AppComponent]
})
export class AppModule { }
