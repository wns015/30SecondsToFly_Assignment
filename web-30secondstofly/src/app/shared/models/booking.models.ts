export class BookingRequestModel {
    public outboundFlightId: number;
    public returnFlightId: number;
    public totalPrice: number;
    public passengers: PassengerModel[] = new Array<PassengerModel>();
    public fareClass: number;
    public paymentDetails: string;
    public paymentMethod: number;
}

export class PassengerModel {
    public name: string;
    public surname: string;
    public dateOfBirth: string;
    public passportIssuer: string;
    public passportNo: string;
}

export class BookingSearchModel {
    public bookingReferenceNo: string;
    public surname: string;
}

export class BookingResponseModel {
    public origin: string;
    public destination: string;
    public outboundDeparture: Date;
    public outboundArrival: Date;
    public outboundDuration: number;
    public outboundAirline: string;
    public outboundFlightNo: string;
    public returnDeparture: Date;
    public returnArrival: Date;
    public returnDuration: number;
    public returnAirline: string;
    public returnFlightNo: string;
    public passengers: PassengerModel[];
    public bookingReferenceNo: string;
    public email: string;
}