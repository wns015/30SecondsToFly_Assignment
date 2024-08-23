import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FlightModel } from '../../../../shared/models/search.models';
import { BookingRequestModel } from '../../../../shared/models/booking.models';
import moment from 'moment';
import { DurationPipe } from '../../../../shared/pipes/duration-pipe';
import { CommonModule } from '@angular/common';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import {  FormArray, FormBuilder,FormsModule, ReactiveFormsModule} from '@angular/forms';
import { Countries } from '../../../../../assets/countries';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { PaymentFormComponent } from "../../../../shared/forms/payment-form/payment-form.component";
import { provideNativeDateAdapter } from '@angular/material/core';

@Component({
  selector: 'app-booking-form',
  standalone: true,
  imports: [DurationPipe, CommonModule, MatDatepickerModule, MatFormFieldModule, MatInputModule, ReactiveFormsModule, FormsModule, MatAutocompleteModule, PaymentFormComponent],
  templateUrl: './booking-form.component.html',
  styleUrl: './booking-form.component.css',
  providers: [provideNativeDateAdapter()]
})
export class BookingFormComponent {
    @Input() outboundFlight: FlightModel;
    @Input() returnFlight: FlightModel;
    @Input() booking: BookingRequestModel;

    @Output() cancelBooking = new EventEmitter<void>();

    public price: number;
    public countryList = Countries;
    public filteredCountryList: any[] = [];
    public readyToPay: boolean = false;
    public bookingGroup: FormArray;

    constructor(private formBuilder: FormBuilder){}

    public getDateTimeString(dateTime: Date): string {
        
        let day = moment(dateTime).format('DD MMM YYYY HH:mm')

        return day;
    }

    public ngOnInit(): void {
        this.price = this.outboundFlight.price;
        if(this.returnFlight){
            this.price += this.returnFlight.price;
        }
        this.booking.passengers.forEach(p => {
            this.filteredCountryList.push(this.countryList);
        });
    }

    public setDateOfBirth($event, index: number): void {
        let date: Date = $event.value;
        this.booking.passengers[index].dateOfBirth = moment(date).format("YYYY-MM-DD").toString();
    }

    public cancel(): void {
        this.cancelBooking.emit();
    }

    public onCountryInput($event, index: number): void {
        if (this.countryList.length > 0) {
            let filterName: string = $event.target.value.trim().toLowerCase();
            this.filteredCountryList[index] = new Array<any>();

            for (var i: number = 0; i < this.countryList.length; i++) {
                if (this.countryList[i].toLowerCase().includes(filterName) || this.countryList[i].toLowerCase().includes(filterName)) {
                    this.filteredCountryList[index].push(this.countryList[i]);
                }
            }
        }
    }

    public setPassportCountry(country: string, index: number){
        this.booking.passengers[index].passportCountry = country;
    }

    
    public continueToPayment(): void {
        

        if(this.validPassengers()){
            
            this.readyToPay = true;
        }
    }

    public canceledPayment(): void {
        this.readyToPay = false;
    }

    private validPassengers(): boolean {
        let ps = this.booking.passengers
        for(let i = 0; i < ps.length; i++) {
            if(!ps[i].name || !ps[i].surname || !ps[i].dateOfBirth || 
                !ps[i].passportCountry || !ps[i].passportNo) {

                return false;
            }
        }
        return true;
    }
}
