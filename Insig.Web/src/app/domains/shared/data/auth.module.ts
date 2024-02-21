import { ModuleWithProviders, NgModule } from '@angular/core';

import { AuthService } from './auth.service';


@NgModule({})
export class AuthModule {
    public static forRoot(): ModuleWithProviders<AuthModule> {
        return {
            ngModule: AuthModule,
            providers: [AuthService]
        };
    }
}
