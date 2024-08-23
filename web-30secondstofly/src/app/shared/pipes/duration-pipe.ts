import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: 'duration',
    standalone: true
  })
  export class DurationPipe implements PipeTransform {
    transform(value: number): string {
        let hours = Math.floor(value / 60);
        let minutes = Math.floor(value % 60);
        return hours + ' Hrs ' + minutes + ' Mins';
      }
  }