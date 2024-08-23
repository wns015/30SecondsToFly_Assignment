import { Component, EventEmitter, Input, Output } from '@angular/core';
import { BookingRequestModel, BookingResponseModel } from '../../models/booking.models';
import { CommonModule } from '@angular/common';
import { CreditCardPaymentModel, PaymentTypes } from '../../models/payment.models';
import { EncryptionService } from '../../services/encryption.service';
import { BookingService } from '../../services/booking.service';
import { FormsModule } from '@angular/forms';
import { BookingInfoComponent } from "../../components/booking-info/booking-info.component";
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { ErrorDialogComponent } from '../../dialogs/error-dialog/error-dialog.component';

@Component({
  selector: 'app-payment-form',
  standalone: true,
  imports: [CommonModule, FormsModule, BookingInfoComponent],
  templateUrl: './payment-form.component.html',
  styleUrl: './payment-form.component.css',
  providers: [EncryptionService]
})
export class PaymentFormComponent {

    @Input() bookingRequest: BookingRequestModel;
    @Input() isBooking: boolean;

    @Output() cancel = new EventEmitter<void>();

    public paymentMethod: number;
    public paymentPurpose: any;
    public paymentTypes: typeof PaymentTypes = PaymentTypes;
    public isLoading: boolean;
    public paymentSuccess: boolean = false;
    public bookingDetails: BookingResponseModel;

    public paymentDetails: CreditCardPaymentModel = new CreditCardPaymentModel();
    private dialogConfig: MatDialogConfig = { disableClose: true };

    constructor(private encryptService: EncryptionService, private bookingService: BookingService, private dialog: MatDialog) {}

    public ngOnInit(): void {
        if(this.isBooking) {
            this.paymentPurpose = this.bookingRequest;
        }
    }

    public onExpDateKeyPress(event: KeyboardEvent): any {
        const input = event.target as HTMLInputElement;
        const NumReg: RegExp = /[0-9]/;
    
        if(!NumReg.test(event.key)){
          event.preventDefault();
        }
        
        const dateString = input.value;
        if(dateString.length == 2){
          return (input.value = dateString + '/')
        }
    }

    public onCCDetailKeyPress(event: KeyboardEvent): void {
        const NumReg: RegExp = /[0-9]/;
    
        if(!NumReg.test(event.key)){
          event.preventDefault();
        }
    }

    public submitPayment(){
        if(this.validPaymentDetails()) {
            this.paymentPurpose.paymentMethod = this.paymentMethod;
            let payment = JSON.stringify(this.paymentDetails)

            this.paymentPurpose.paymentDetails = this.encryptService.encryptText(payment);
            this.isLoading = true;

            if(this.isBooking) {
                this.bookingService.bookFlight(this.paymentPurpose).subscribe((res) =>{
                    if(res.data) {
                        this.bookingDetails = res.data;
                        this.isLoading = false;
                        this.paymentSuccess = true;
                    }
                }, (error) => {
                    const dialogRef = this.dialog.open(ErrorDialogComponent, this.dialogConfig);
                    this.isLoading = false;
                });
            }
        }
        
    }

    private validPaymentDetails() {
        let pd = this.paymentDetails
        if(!pd.name || !pd.cID || !pd.cardNo || !pd.expiration) {
            return false;
        }
        return true;
    }
}
