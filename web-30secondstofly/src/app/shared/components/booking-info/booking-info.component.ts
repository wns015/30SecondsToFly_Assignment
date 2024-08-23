import { Component, Input } from '@angular/core';
import { BookingResponseModel } from '../../models/booking.models';
import moment from 'moment';
import { DurationPipe } from '../../pipes/duration-pipe';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-booking-info',
  standalone: true,
  imports: [DurationPipe, CommonModule],
  templateUrl: './booking-info.component.html',
  styleUrl: './booking-info.component.css'
})
export class BookingInfoComponent {

    @Input() bookingDetails: BookingResponseModel;

    public getDateTimeString(dateTime: Date): string {
        
        let day = moment(dateTime).format('DD MMM YYYY HH:mm')

        return day;
    }
}
