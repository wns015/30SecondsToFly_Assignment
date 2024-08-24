import { Component, OnInit } from '@angular/core';
import { SearchComponent } from "./components/search/search.component";
import { SearchResultsComponent } from "./components/search-results/search-results.component";
import { MatExpansionModule, MatExpansionPanel } from '@angular/material/expansion'
import { FlightModel, SearchModel, SearchResultModel } from '../../shared/models/search.models';
import { CommonModule } from '@angular/common';
import { BookingService } from '../../shared/services/booking.service';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { BookingRequestModel, PassengerModel } from '../../shared/models/booking.models';
import { BookingFormComponent } from './components/booking-form/booking-form.component';
import { PaymentFormComponent } from '../../shared/forms/payment-form/payment-form.component';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { ErrorDialogComponent } from '../../shared/dialogs/error-dialog/error-dialog.component';
import { EncryptionService } from '../../shared/services/encryption.service';
import { TransmissionModel } from '../../shared/models/request.model';

@Component({
  selector: 'app-booking-page',
  standalone: true,
  imports: [SearchComponent, SearchResultsComponent, MatExpansionModule, CommonModule, HttpClientModule, BookingFormComponent, PaymentFormComponent],
  templateUrl: './booking-page.component.html',
  styleUrl: './booking-page.component.css',
  viewProviders: [MatExpansionPanel],
  providers: [BookingService, HttpClient, EncryptionService]
})
export class BookingPageComponent {

    public airportList: any;
    public searchResult: SearchResultModel;
    public searchParameters: SearchModel;
    public noResults: boolean = false;
    public isSearchExpanded: boolean = true;
    public bookingRequest: BookingRequestModel = new BookingRequestModel();
    public outboundPrice: number = 0;
    public returnPrice: number = 0;
    public singlePersonPrice: number = 0;
    public isBooking: boolean = false;
    public outboundFlight: FlightModel;
    public returnFlight: FlightModel;
    public isLoading: boolean = false;

    constructor(private bookingService: BookingService, private dialog: MatDialog, private encryptionService: EncryptionService) {}

    private dialogConfig: MatDialogConfig = { disableClose: true };
    
    public searchFlights($event) {
        this.resetSearch();
        this.searchParameters = $event;
        this.isLoading = true;

        let encryptedObjectString = JSON.stringify(this.searchParameters);
        let encryptedObject = this.encryptionService.encryptText(encryptedObjectString);
        this.bookingService.searchFlights(encryptedObject).subscribe((res) =>
            {
                if(res.data){
                    let response: TransmissionModel = res.data;
                    this.searchResult = this.encryptionService.decryptTextToObject(response.encryptedString)
                    
                    this.isSearchExpanded = false;
                    this.isLoading = false;
                } else {
                    this.noResults = true;
                    this.isLoading = false;
                }
                
            },
            (error) => {
                const dialogRef = this.dialog.open(ErrorDialogComponent, this.dialogConfig);
                this.isLoading = false;
            }
        )
    }

    public expandSearchPanel(): void {
        this.isSearchExpanded = !this.isSearchExpanded;
    }

    public setOutboundFlight($event): void {
        this.outboundFlight = $event;
        this.bookingRequest.outboundFlightId = $event.id;
        this.outboundPrice = $event.price;
        this.singlePersonPrice = this.outboundPrice + this.returnPrice;
        this.bookingRequest.totalPrice = this.searchParameters.noOfPassengers * this.singlePersonPrice;
    }

    public setReturnFilght($event): void {
        this.returnFlight = $event;
        this.bookingRequest.returnFlightId = $event.id;
        this.returnPrice = $event.price;
        this.singlePersonPrice = this.outboundPrice + this.returnPrice;
        this.bookingRequest.totalPrice = this.searchParameters.noOfPassengers * this.singlePersonPrice;
    }

    private resetSearch(): void {
        this.outboundPrice = 0;
        this.returnPrice = 0;
        this.singlePersonPrice = 0;
        this.searchResult = new SearchResultModel();
        this.bookingRequest = new BookingRequestModel();
        this.noResults = false;
    }

    public bookFlight(): void {
        if(!this.bookingRequest.outboundFlightId) {
            return;
        }

        if(!this.searchParameters.isOneWay && !this.bookingRequest.returnFlightId) {
            return;
        }

        for(let i = 0; i < this.searchParameters.noOfPassengers; i++){
            let passenger: PassengerModel = new PassengerModel();
            this.bookingRequest.passengers.push(passenger);
        }
        this.isBooking = true;
    }

    public cancelBookingEntry() {
        this.isBooking = false;
    }
}
