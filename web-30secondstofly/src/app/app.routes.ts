import { Routes } from '@angular/router';
import { BookingPageComponent } from './pages/booking/booking-page.component';
import { BookingSearchComponent } from './pages/booking-search/booking-search.component';
import { HomeComponent } from './pages/home/home.component';


export const routes: Routes = [
    { path: '', redirectTo: '/home', pathMatch: 'full'},
    { path: 'home', component: HomeComponent},
    { path: 'book-flights', component: BookingPageComponent},
    { path: 'find-booking', component: BookingSearchComponent},
    { path: '**', redirectTo: ''}
];
