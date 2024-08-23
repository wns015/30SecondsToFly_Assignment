namespace Common.Constants
{
    public enum PaymentType
    {
        CreditCard = 1,
        BankTransfer = 2,
    }

    public enum FareType
    {
        Economy = 1,
        Premium = 2,
        Business = 3,
        First = 4
    }

    public class FareClass
    {
        public string ToFareClassPrice(int fareType)
        {
            switch (fareType)
            {
                case 1:
                    return "EconomyFare";
                case 2:
                    return "PremiumFare";
                case 3:
                    return "BusinessFare";
                case 4:
                    return "FirstClassFare";
                default:
                    return "EconomyFare";
            }
        }

        public string ToFareClassSeating(int fareType)
        {
            switch (fareType)
            {
                case 1:
                    return "EconomySeating";
                case 2:
                    return "PremiumSeating";
                case 3:
                    return "BusinessSeating";
                case 4:
                    return "FirstClassSeating";
                default:
                    return "EconomySeating";
            }
        }
    }
}
