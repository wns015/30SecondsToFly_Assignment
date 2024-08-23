import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {

    constructor(private router: Router){}

    public navigateBookingPage(): void {
        this.router.navigate(['/book-flights']);
    }

    public navigateFindBooking(): void {
        this.router.navigate(['/find-booking']);
    }
}
