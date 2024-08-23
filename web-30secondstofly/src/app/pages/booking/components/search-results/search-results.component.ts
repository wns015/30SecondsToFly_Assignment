import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FlightModel } from '../../../../shared/models/search.models';
import { CommonModule } from '@angular/common';
import { DurationPipe } from '../../../../shared/pipes/duration-pipe';
import moment from 'moment';

@Component({
  selector: 'app-search-results',
  standalone: true,
  imports: [CommonModule, DurationPipe],
  templateUrl: './search-results.component.html',
  styleUrl: './search-results.component.css',
})
export class SearchResultsComponent {
    @Input() flights: FlightModel[];
    
    @Output() chooseFlight = new EventEmitter<FlightModel>();

    public selectedFlight: FlightModel;

    public selectFlight(id: number): void {
        this.selectedFlight = this.flights.find(p => p.id == id);
        this.chooseFlight.emit(this.selectedFlight);
    }

    public getDateTimeString(dateTime: Date): string {
        
        let day = moment(dateTime).format('DD MMM YYYY HH:mm')

        return day;
    }
}
