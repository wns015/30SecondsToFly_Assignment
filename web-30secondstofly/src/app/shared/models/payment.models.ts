export class CreditCardPaymentModel {
    public name: string;
    public cardNo: string;
    public cID: string;
    public expiration: string;
}

export enum PaymentTypes {
    CreditCard = 1
}