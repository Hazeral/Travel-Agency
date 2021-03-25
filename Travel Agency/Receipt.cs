using System.Collections.Generic;

namespace Travel_Agency
{
    class Receipt
    {
        public int NumAdults;
        public int NumChildren;
        public int NumNights;
        public Car CurrentCar;
        public bool MysteryTour;
        public string CurrentCity;
        public float Total;

        private float adultPrice = 90;
        private float childPrice = 75;
        private float tourPrice = 20;

        public void Update(int adults, int children, int nights, Car car, bool tour, string city)
        {
            NumAdults = adults;
            NumChildren = children;
            NumNights = nights;
            CurrentCar = car;
            MysteryTour = tour;
            CurrentCity = city;

            Total = 0;
            Total += adults * adultPrice * nights;
            Total += children * childPrice * nights;
            if (car != null) Total += car.Price * nights;
            if (tour) Total += tourPrice;
        }

        public List<string> Info()
        {
            List<string> res = new List<string>();
            if (NumAdults > 0) res.Add($"Adults x{NumAdults} — £{NumAdults * adultPrice}");
            if (NumChildren > 0) res.Add($"Children x{NumChildren} — £{NumChildren * childPrice}");
            if (CurrentCar != null) res.Add($"{CurrentCar.Name} x1 — £{CurrentCar.Price}");
            if (MysteryTour) res.Add($"Mystery tour x1 — £{tourPrice}");
            res.Add($"Nights x{NumNights}");
            res.Add($"City: {CurrentCity}");
            res.Add(""); // spacer
            res.Add($"Total: £{Total}");

            return res;
        }

        public Receipt Clone()
        {
            return (Receipt)MemberwiseClone();
        }
    }
}
