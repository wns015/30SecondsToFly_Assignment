import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { SearchModel } from "../models/search.models";
import { BookingRequestModel, BookingSearchModel } from "../models/booking.models";
import { Observable, throwError } from "rxjs";
import { TransmissionModel } from "../models/request.model";


@Injectable()
export class BookingService {

    constructor(
        private http: HttpClient
    ) {
    }

    private searchApi = "https://localhost:7027/api/search";
    private bookingApi = "https://localhost:7027/api/booking";

    public searchFlights(encryptedString: string) : Observable<any> {
        
        return this.http.post(this.searchApi + "/flights", this.createRequestObject(encryptedString))
    }

    public bookFlight(encryptedString: string) : Observable<any> {
        return this.http.post(this.bookingApi + "/flights", this.createRequestObject(encryptedString));
    }

    public findBooking(encryptedString: string) : Observable<any> {
        return this.http.post(this.bookingApi + "/find", this.createRequestObject(encryptedString));
    }

    private createRequestObject(string: string): TransmissionModel {
        let encryptedRequest = new TransmissionModel();
        encryptedRequest.encryptedString = string;
        return encryptedRequest;
    }
}

