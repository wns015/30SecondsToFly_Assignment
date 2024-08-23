import { Component, EventEmitter, Output } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatDatepickerModule } from '@angular/material/datepicker'
import { MatInputModule } from '@angular/material/input'
import { MatFormFieldModule } from '@angular/material/form-field';
import { DateAdapter, provideNativeDateAdapter } from '@angular/material/core';
import { MatCheckboxModule } from '@angular/material/checkbox'
import { MatSelectModule } from '@angular/material/select'
import { MatAutocompleteModule } from '@angular/material/autocomplete'
import { airports } from '../../../../../assets/airports';
import { SearchModel } from '../../../../shared/models/search.models';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [FormsModule, CommonModule, ReactiveFormsModule, MatDatepickerModule, MatFormFieldModule, MatInputModule, MatCheckboxModule, MatSelectModule, MatAutocompleteModule],
  providers: [provideNativeDateAdapter()],
  templateUrl: './search.component.html',
  styleUrl: './search.component.css'
})
export class SearchComponent {

    @Output() searchEvent = new EventEmitter<SearchModel>()

    

    constructor(
        private dateAdapter: DateAdapter<Date>
      ) {
        this.minDate = new Date();
        this.dateAdapter.setLocale('en-GB');
      }

      public airportList: any = airports;
      public searchParameters: SearchModel = new SearchModel();
      public minDate: Date;
      public filteredOriginList: any;
      public filteredDestinationList: any;
    
      public ngOnInit(): void {
        this.airportList = this.airportList.sort((a, b) => a.Code.localeCompare(b.Code));
        this.filteredOriginList = this.airportList;
        this.filteredDestinationList = this.airportList;
      }

      public toggleIsOneWay(): void {
        this.searchParameters.isOneWay = !this.searchParameters.isOneWay;
      }

      public onOriginInput($event): void {
        if (this.airportList.length > 0) {
            let filterName: string = $event.target.value.trim().toLowerCase();
            this.filteredOriginList = new Array<any>();

            for (var i: number = 0; i < this.airportList.length; i++) {
                if (this.airportList[i].Code.toLowerCase().includes(filterName) || this.airportList[i].City.toLowerCase().includes(filterName)) {
                    this.filteredOriginList.push(this.airportList[i]);
                }
            }
        }
      }

      public setOriginCode(code: string): void {
        this.searchParameters.origin = code;
      }

      public onDestinationInput($event): void {
        if (this.airportList.length > 0) {
            let filterName: string = $event.target.value.trim().toLowerCase();
            this.filteredDestinationList = new Array<any>();

            for (var i: number = 0; i < this.airportList.length; i++) {
                if (this.airportList[i].Code.toLowerCase().includes(filterName) || this.airportList[i].City.toLowerCase().includes(filterName)) {
                    this.filteredDestinationList.push(this.airportList[i]);
                }
            }
        }
      }

      public setDestinationCode(code: string): void {
        this.searchParameters.destination = code;
      }

      public searchFlights() {
        if(this.validateFields()){
            this.searchEvent.emit(this.searchParameters)
        }
      }

      private validateFields(): boolean {
        if(this.searchParameters.departureDate && this.searchParameters.destination 
            && this.searchParameters.origin && this.searchParameters.fareClass && this.searchParameters.noOfPassengers
            && (this.searchParameters.isOneWay || (!this.searchParameters.isOneWay && this.searchParameters.returnDate))) {
                return true;
            }
        return false;
      }

      public setOutboundDate($event): void {
        let date: Date = $event.value;
        this.searchParameters.departureDate = date.toLocaleDateString();
      }

      public setReturnDate($event): void {
        let date: Date = $event.value;
        this.searchParameters.returnDate = date.toLocaleDateString();
      }

}

