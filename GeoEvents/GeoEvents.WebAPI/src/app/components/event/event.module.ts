import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { ReactiveFormsModule } from "@angular/forms";
import { EventRoutingModule } from "./event-routing.module";
import { NguiDatetimePickerModule } from '@ngui/datetime-picker';
import { AgmCoreModule } from '@agm/core';
import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { PaginatorModule } from 'primeng/primeng';

import { EventSearchComponent } from "./components/event-search.component";
import { EventListComponent } from "./components/event-search-list.component";
import { EventMapComponent } from "./components/event-search-map.component";
import { EventCreateComponent } from "./components/event-create.component";
import { EventCreateImagesComponent } from "./components/event-create-images.component";
import { EventCreateDataComponent } from "./components/event-create-data.component";
import { EventCreateCustomizeComponent } from './components/event-create-customize.component';
import { EventDetailComponent } from "./components/event-detail.component";

import { EventService } from "./event.service";
import { LocationService } from './location.service';
import { PreserveSearchQuerryService } from "../../shared/preserve-search-querry.service";
import { GeocodingService } from '../../shared/geocoding.service';
import { EventDetailRouteActivatorService } from './event-detail-route-activator.service';
import { EventDetailResolverService } from './event-detail-resolver.service'

import { DecimalPipe } from '@angular/common';

@NgModule({
    imports: [
		EventRoutingModule,
		ReactiveFormsModule,
		NguiDatetimePickerModule,
		CommonModule,
		BrowserModule,
		AgmCoreModule.forRoot({
			apiKey: 'AIzaSyDHKcbmM0jpW7BOet42_S92KJSr5PYKc5w',
			libraries: ['places']
        }),
        PaginatorModule
    ],

    declarations: [
		EventSearchComponent,
		EventListComponent,
		EventMapComponent,
		EventCreateComponent,
		EventCreateImagesComponent,
        EventCreateDataComponent,
        EventCreateCustomizeComponent,
		EventDetailComponent
	],
	
	providers: [ 
        EventService,
        LocationService,
        PreserveSearchQuerryService,
        GeocodingService,
        EventDetailRouteActivatorService,
        EventDetailResolverService
	]
})
export class EventModule { }