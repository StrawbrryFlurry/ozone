import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthorizationRequestResolver } from './services/authorization-request.resolver';
import { AuthorizationShell } from './shell/authorization/authorization.shell';

const routes: Routes = [
  {
    component: AuthorizationShell,
    path: 'authorize',
    resolve: {
      authorizationRequest: AuthorizationRequestResolver,
    },
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AuthorizationRoutingModule {}
