import { Component } from '@angular/core';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-error-dialog',
  templateUrl: './error-dialog.component.html',
  styleUrls: ['./error-dialog.component.scss'],
  standalone: true,
  imports: [MatDialogModule]
})
export class ErrorDialogComponent {

    constructor(
        public dialogRef: MatDialogRef<ErrorDialogComponent>
    ) {}
}
