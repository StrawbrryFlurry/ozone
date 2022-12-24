import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthorizationShell } from './shell/authorization/authorization.shell';
import { AuthorizationPage } from './pages/authorization/authorization.page';
import { AuthorizationRoutingModule } from './authorization.routing-module';
import { AuthorizationConsentItemComponent } from './components/authorization-consent-item/authorization-consent-item.component';
import { AuthorizationConsentListComponent } from './components/authorization-consent-list/authorization-consent-list.component';
import { AuthorizationConsentButtonsComponent } from './components/authorization-consent-buttons/authorization-consent-buttons.component';
import { MatButtonModule } from '@angular/material/button';
import { AuthorizationConsentInfoComponent } from './components/authorization-consent-info/authorization-consent-info.component';
import { MatExpansionModule } from '@angular/material/expansion';
import { AuthorizationService } from './services/authorization.service';

@NgModule({
  declarations: [
    AuthorizationShell,
    AuthorizationPage,
    AuthorizationConsentItemComponent,
    AuthorizationConsentListComponent,
    AuthorizationConsentButtonsComponent,
    AuthorizationConsentInfoComponent,
  ],
  imports: [
    CommonModule,
    AuthorizationRoutingModule,
    MatButtonModule,
    MatExpansionModule,
  ],
  providers: [AuthorizationService],
})
export class AuthorizationModule {}
