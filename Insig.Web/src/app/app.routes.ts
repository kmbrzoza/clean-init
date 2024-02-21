import { Routes } from '@angular/router';
import { MsalGuard } from '@azure/msal-angular';

import { CanActivateRoleGuard, Roles } from '@domains/shared/data';

import { AdminWrapperComponent } from './shell/admin/wrapper/admin-wrapper.component';
import { AccessDeniedComponent } from './shell/shared/access-denied/access-denied.component';
import { PageNotFoundComponent } from './shell/shared/page-not-found/page-not-found.component';
import { SiteWrapperComponent } from './shell/site/wrapper/site-wrapper.component';
import { DashboardComponent } from './shell/admin/dashboard/dashboard.component';
import { HomeComponent } from './shell/site/home/home.component';

export const APP_ROUTES: Routes = [
    {
        path: 'auth',
        loadComponent: () => import('@azure/msal-angular').then(mod => mod.MsalRedirectComponent)
    },
    {
        path: '',
        component: SiteWrapperComponent,
        children: [
            {
                path: '',
                component: HomeComponent
            },
            {
                path: 'example',
                loadChildren: () => import('./domains/test/feature-example').then(m => m.FEATURE_EXAMPLE)
            }
        ]
    },
    {
        path: 'admin',
        component: AdminWrapperComponent,
        canActivate: [MsalGuard, CanActivateRoleGuard],
        data: [Roles.Admin],
        children: [
            {
                path: '',
                component: DashboardComponent
            },
            {
                path: 'notifications',
                loadChildren: () => import('./domains/communication/feature-notifications').then(m => m.FEATURE_NOTIFICATIONS_ROUTES)
            },
            {
                path: 'playground',
                loadChildren: () => import('./domains/communication/feature-playground').then(m => m.FEATURE_PLAYGROUND_ROUTES)
            }
        ]
    },
    {
        path: 'not-found',
        component: PageNotFoundComponent
    },
    {
        path: 'access-denied',
        component: AccessDeniedComponent
    },
    {
        path: '**',
        redirectTo: 'not-found',
        pathMatch: 'full'
    }
];

export default APP_ROUTES;
