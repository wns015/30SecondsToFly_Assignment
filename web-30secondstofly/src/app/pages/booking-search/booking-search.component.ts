import { Component } from '@angular/core';
import { BookingService } from '../../shared/services/booking.service';
import { BookingSearchModel, BookingResponseModel } from '../../shared/models/booking.models';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BookingInfoComponent } from "../../shared/components/booking-info/booking-info.component";
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { ErrorDialogComponent } from '../../shared/dialogs/error-dialog/error-dialog.component';
import { HttpClientModule } from '@angular/common/http';
import { EncryptionService } from '../../shared/services/encryption.service';
import { TransmissionModel } from '../../shared/models/request.model';

@Component({
  selector: 'app-booking-search',
  standalone: true,
  imports: [CommonModule, FormsModule, BookingInfoComponent, HttpClientModule],
  templateUrl: './booking-search.component.html',
  styleUrl: './booking-search.component.css',
  providers: [BookingService, EncryptionService]
})
export class BookingSearchComponent {

    constructor(private bookingService: BookingService, private dialog: MatDialog, private encryptionService: EncryptionService){}

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

        let encryptedObjectString = JSON.stringify(this.findBookingModel);
        let encryptedObject = this.encryptionService.encryptText(encryptedObjectString);

        this.bookingService.findBooking(encryptedObject).subscribe((res) =>{
            if(res.data){
                let response: TransmissionModel = res.data;
                this.bookingSearchResult = this.encryptionService.decryptTextToObject(response.encryptedString);
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
