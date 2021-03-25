namespace Travel_Agency
{
    class City
    {
        public string Name;
        public int Vacancies;

        public City(string name, int maxResidents)
        {
            Name = name;
            Vacancies = maxResidents;
        }
    }
}
