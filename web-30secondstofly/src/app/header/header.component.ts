import { CommonModule } from '@angular/common';
import { Component, HostListener } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {

    @HostListener('window:resize', ['$event'])
        getScreenSize(event?){
        this.screenWidth = window.innerWidth;
    }

    public subRoute: Subscription;

    public screenWidth: number;

    constructor(private router: Router) { }

    currentPage: string = "";

    ngOnInit(): void {
        this.subRoute = this.router.events.subscribe((event)=>{
          if(event instanceof NavigationEnd){
            this.currentPage = event.url
          }
        })
      }

    headerNav(page: string){
        this.router.navigate([page])
      }
}
