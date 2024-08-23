export class SearchResultModel{
    public outboundFlights: FlightModel[];
    public returnFlights: FlightModel[];
}

export class FlightModel {
    public id: number;
    public origin: string;
    public destination: string;
    public departureTime: Date;
    public arrivalTime: Date;
    public price: number;
    public duration: number;
    public flightNo: string;
    public airline: string;
}

export class SearchModel {
    public origin: string;
    public destination: string;
    public departureDate: string;
    public returnDate: string;
    public noOfPassengers: number;
    public fareClass: number;
    public isOneWay: boolean = false;
}

export class FareType {
    public Economy = 1;
    public Premium = 2;
    public Business = 3;
    public FirstClass = 4;

    public CodeToString(code: number): string{
        switch (code){
            case 1:
                return "Economy";
            case 2:
                return "Premium";
            case 3:
                return "Business";
            case 4:
                return "First Class";
            default:
                return "Economy";
        }
    }
}