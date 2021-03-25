using System.Collections.Generic;

namespace Travel_Agency
{
    class Booking
    {
        public Receipt receipt;
        public bool Discount = false;

        public Booking(Receipt _receipt)
        {
            receipt = _receipt;
        }

        public string Info()
        {
            List<string> res = new List<string>();
            if (receipt.NumAdults > 0) res.Add($"\tAdults x{receipt.NumAdults}");
            if (receipt.NumChildren > 0) res.Add($"\tChildren x{receipt.NumChildren}");
            if (receipt.CurrentCar != null) res.Add($"\t{receipt.CurrentCar.Name} x1");
            if (receipt.MysteryTour) res.Add($"\tMystery tour x1");
            res.Add($"\tNights x{receipt.NumNights}");
            res.Add($"\tCity: {receipt.CurrentCity}");
            if (Discount)
            {
                res.Add($"\tDiscount: 20%");
                res.Add($"\n\tTotal: £{receipt.Total * .8}");
            } else res.Add($"\n\tTotal: £{receipt.Total}");

            return string.Join("\n", res);
        }
    }
}
