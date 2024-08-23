import { Component } from '@angular/core';
import { BookingService } from '../../shared/services/booking.service';
import { BookingSearchModel, BookingResponseModel } from '../../shared/models/booking.models';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BookingInfoComponent } from "../../shared/components/booking-info/booking-info.component";
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { ErrorDialogComponent } from '../../shared/dialogs/error-dialog/error-dialog.component';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-booking-search',
  standalone: true,
  imports: [CommonModule, FormsModule, BookingInfoComponent, HttpClientModule],
  templateUrl: './booking-search.component.html',
  styleUrl: './booking-search.component.css',
  providers: [BookingService]
})
export class BookingSearchComponent {

    constructor(private bookingService: BookingService, private dialog: MatDialog){}

    public findBookingModel: BookingSearchModel = new BookingSearchModel();
    public bookingSearchResult: BookingResponseModel;
    public isLoading: boolean = false;

    private dialogConfig: MatDialogConfig = { disableClose: true };

    public noResult: boolean = false;

    public findBooking() {
        this.noResult = false;
        if(!this.findBookingModel.bookingReferenceNo || !this.findBookingModel.surname) {
            return;
        }
        this.isLoading = true;
        this.bookingService.findBooking(this.findBookingModel).subscribe((res) =>{
            if(res.data){
                this.bookingSearchResult = res.data
                this.isLoading = false;
            } else {
                this.noResult = true;
                this.isLoading = false;
            }
        }, (error) =>{
            const dialogRef = this.dialog.open(ErrorDialogComponent, this.dialogConfig);
            this.isLoading = false;
        })
    }
}
