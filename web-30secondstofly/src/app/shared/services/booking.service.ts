import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { SearchModel } from "../models/search.models";
import { BookingRequestModel, BookingSearchModel } from "../models/booking.models";
import { Observable, throwError } from "rxjs";


@Injectable()
export class BookingService {

    constructor(
        private http: HttpClient
    ) {
    }

    private searchApi = "https://localhost:7027/api/search";
    private bookingApi = "https://localhost:7027/api/booking";

    public searchFlights(model: SearchModel) : Observable<any> {
        return this.http.post(this.searchApi + "/flights", model)
    }

    public bookFlight(model: BookingRequestModel) : Observable<any> {
        return this.http.post(this.bookingApi + "/flights", model);
    }

    public findBooking(model: BookingSearchModel) : Observable<any> {
        return this.http.post(this.bookingApi + "/find", model);
    }
}

