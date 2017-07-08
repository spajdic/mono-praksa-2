﻿import { Routes } from '@angular/router'

import { CreateEventComponent } from './create-event.component'
import { DisplayListComponent } from './display-list.component'
import { Error404Component } from './errors/error-404.component'

export const routes: Routes = [
    { path: 'create', component: CreateEventComponent },
    { path: 'display/list', component: DisplayListComponent },
    { path: '', redirectTo: 'create', pathMatch: 'full' },
    { path: '404', component: Error404Component },
    { path: '**', redirectTo: '404' }
]