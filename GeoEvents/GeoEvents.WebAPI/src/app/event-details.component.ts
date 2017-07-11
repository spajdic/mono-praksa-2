﻿import { Component, Input, OnInit } from '@angular/core'
import { IEvent } from './event.model'
import { IImage } from './image.model'
import { Http, Response, Headers, RequestOptions } from '@angular/http'
import { Observable } from 'rxjs/Rx'

@Component({
    templateUrl: 'app/event-details.component.html',
    selector: 'event-details'
})

export class EventDetailsComponent implements OnInit{
    @Input() event: IEvent
    images: IImage[]

    constructor(private http: Http) {

    }

    ngOnInit() {
        this.getImages(this.event.Id).subscribe(res => {
            this.images = res
        })
    }

    getImages(id : string): Observable<IImage[]> {
        return this.http.get('/api/images/get/' + this.event.Id).map(function (response: Response) {
            return <IImage[]>response.json();
        }).catch(this.handleError);
    }

    handleError(error: Response) {
        return Observable.throw(error.statusText);
    }

}