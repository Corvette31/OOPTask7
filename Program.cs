namespace OOPTask7
{
    internal class Program
    {
        private const string CommandCreateTrain = "1";
        private const string CommandSkeepTime = "2";
        private const string CommandExit = "3";

        static void Main(string[] args)
        {
            bool isRun = true;
            ControlRoom controlRoom = new ControlRoom();
            Draw.DrawTrainStatus(null);

            while (isRun)
            {
                Console.WriteLine($"Доступные команды:\n{CommandCreateTrain} Сформировать состав\n{CommandSkeepTime} Пропустить время\n{CommandExit} Выйти");
                Console.Write("Введите команду: ");
                string userInput = Console.ReadLine();

                switch (userInput)
                {
                    case CommandCreateTrain:
                        controlRoom.SendTrain();
                        break;
                    case CommandSkeepTime:
                        controlRoom.SkeepTime();
                        break;
                    case CommandExit:
                        isRun = false;
                        break;
                    default:
                        Console.WriteLine("Не известная команда!");
                        break;
                }
            }
        }
    }

    class Draw
    {
        public static void DrawTrainStatus(List<Train>? trains)
        {
            string title = "Комната управления";
            string separator = "********************************************************";
            Console.Clear();
            Console.SetCursorPosition(Console.BufferWidth / 2 - title.Length/2, 0);
            Console.WriteLine(title);
            Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
            Console.WriteLine(separator);
            ShowTrains(trains);
            Console.WriteLine(separator);
        }

        private static void ShowTrains(List<Train>? trains)
        {
            if (trains != null)
            {
                if (trains.Count > 0 && trains != null)
                {
                    foreach (var train in trains)
                    {
                        train.ShowInfo();
                    }
                }
                else
                {
                    Console.WriteLine("На линиях нет поездов!");
                }
            }
            else
            {
                Console.WriteLine("На линиях нет поездов!");
            }
        }
    }

    class City
    {
        public City(string name, double latitude, double longitude)
        {
            Name = name;
            Latitude = latitude;
            Longitude = longitude;
        }

        public string Name { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
    }

    class Direction
    {
        public Direction(City cityDeparture, City cityArrival)
        {
            CityDeparture = cityDeparture;
            CityArrival = cityArrival;
            Distance = DistanceBetweenPoints(cityDeparture.Latitude, cityDeparture.Longitude, cityArrival.Latitude, cityArrival.Longitude);
            int trainSpeed = 100;
            TravelTime = Distance / trainSpeed;
        }

        public City CityDeparture { get; private set; }
        public City CityArrival { get; private set; }
        public double Distance { get; private set; }
        public double TravelTime { get; private set; }

        public void ShowInfo()
        {
            var minutes = (TravelTime - Math.Truncate(TravelTime)) * 60;
            Console.WriteLine($"Маршрут {CityDeparture.Name} - {CityArrival.Name} , расстояние : {Math.Round(Distance, 0)} км. , время в пути {Math.Truncate(TravelTime)} ч. {Math.Round(minutes, 0)} мин.");
        }

        private double ConvertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }
        private double DistanceBetweenPoints(double latitude1, double longitude1, double latitude2, double longitude2, double earthRadius = 6371)
        {
            double deltaLatitude = ConvertToRadians(latitude2 - latitude1);
            double deltaLongitude = ConvertToRadians(longitude2 - longitude1);
            double formulaPart1 = Math.Sin(deltaLatitude / 2) * Math.Sin(deltaLatitude / 2) +
            Math.Sin(deltaLongitude / 2) * Math.Sin(deltaLongitude / 2) * Math.Cos(ConvertToRadians(latitude1)) * Math.Cos(ConvertToRadians(latitude2));
            double formulaPart2 = 2 * Math.Atan2(Math.Sqrt(formulaPart1), Math.Sqrt(1 - formulaPart1));
            return earthRadius * formulaPart2;
        }
    }

    enum VagonType
    {
        ReservedSeat = 54,
        Coupe = 32,
        SV = 16
    }

    enum TrainStatus
    {
        InWay,
        Arrived
    }

    class Vagon
    {
        public Vagon(VagonType type, int occupiedPlaces)
        {
            Type = type;
            OccupiedPlaces = occupiedPlaces;
        }

        public VagonType Type { get; private set; }
        public int OccupiedPlaces { get; private set; }
        public const string CommandReservedSeatVagonType = "1";
        public const string CommandCoupeVagonType = "2";
        public const string CommandSVVagonType = "3";

        public static void ShowInfo()
        {
            Console.WriteLine($"{CommandReservedSeatVagonType} - Плацкартный , вместимость {(int)VagonType.ReservedSeat} пас.");
            Console.WriteLine($"{CommandCoupeVagonType} - Купе , вместимость {(int)VagonType.Coupe} пас.");
            Console.WriteLine($"{CommandSVVagonType} - СВ , вместимость {(int)VagonType.SV} пас.");
        }
    }

    class Train
    {
        private Direction _direction;
        private List<Vagon> _vagons;
        private double _timeLeftInWay;
        private int _passengersCount;

        public Train(Direction direction, List<Vagon> vagons, int passengersCount)
        {
            _direction = direction;
            _vagons = vagons;
            Status = TrainStatus.InWay;
            _passengersCount = passengersCount;
            _timeLeftInWay = direction.TravelTime;
        }

        public TrainStatus Status { get; private set; }

        public void UpdateStatus(double deltaTime)
        {
            _timeLeftInWay -= deltaTime;

            if (_timeLeftInWay <= 0)
            {
                Status = TrainStatus.Arrived;
            }
        }

        public void AddVagon(VagonType vagonType, int occupiedPlaces)
        {
            _vagons.Add(new Vagon(vagonType, occupiedPlaces));
        }

        public void ShowInfo()
        {
            if (Status == TrainStatus.Arrived)
            {
                Console.WriteLine($"Поезд {_direction.CityDeparture.Name} - {_direction.CityArrival.Name} прибыл! Кол-во пассажиров : {_passengersCount}.");
            }
            else
            {
                var minutes = (_timeLeftInWay - Math.Truncate(_timeLeftInWay)) * 60;
                Console.WriteLine($"Поезд {_direction.CityDeparture.Name} - {_direction.CityArrival.Name} в пути! До прибылия осталось : {Math.Truncate(_timeLeftInWay)} ч. {Math.Round(minutes, 0)} мин. Кол-во пассажиров : {_passengersCount}.");
            }
        }
    }

    class ControlRoom
    {
        private List<City> _cities;
        private List<Train> _trains;
        private Random _random;
        private double _skeepTime;

        public ControlRoom()
        {
            _cities = CreateCitys();
            _trains = new List<Train>();
            _random = new Random();
            _skeepTime = 1;
        }

        public void SendTrain()
        {
            SkeepTime();
            Draw.DrawTrainStatus(_trains);

            var direction = CreateDirection();

            if (direction != null)
            {
                Draw.DrawTrainStatus(_trains);
                int passengersCount = SellTickets();
                Console.WriteLine($"На направление {direction.CityDeparture.Name} - {direction.CityArrival.Name} продано {passengersCount} билетов!");
                var train = CreateTrain(direction,passengersCount);
                _trains.Add(train);
                Draw.DrawTrainStatus(_trains);
                Console.WriteLine("Состав отправился в путь!");
            }
            else
            {
                Draw.DrawTrainStatus(_trains);
                Console.WriteLine("Ошибака создания направления!");
                Console.WriteLine("Нажмите любую клавишу для продолжения: ");
                Console.ReadKey();
            }
        }

        public void SkeepTime()
        {
            foreach (var train in _trains)
            {
                train.UpdateStatus(_skeepTime);
            }

            Draw.DrawTrainStatus(_trains);
        }

        private int SellTickets()
        {
            int minPassengersCount = 100;
            int maxPassengersCount = 1001;

            return _random.Next(minPassengersCount, maxPassengersCount);
        }

        private Direction CreateDirection()
        {
            var availableCities = CopyListCity(_cities);
            Console.WriteLine("Выберите город отправления:");
            ShowCities(availableCities);
            int cityNaumer = GetNuber();

            if (IsValidNuber(cityNaumer, availableCities))
            {
                var cityDeparture = availableCities[cityNaumer];
                availableCities.RemoveAt(cityNaumer);
                Draw.DrawTrainStatus(_trains);
                Console.WriteLine("Выберите город назначениея:");
                ShowCities(availableCities);
                cityNaumer = GetNuber();

                if (IsValidNuber(cityNaumer, availableCities))
                {
                    var cityArrival = availableCities[cityNaumer];
                    return new Direction(cityDeparture, cityArrival);
                }
                else
                {
                    Console.WriteLine("Не корректный номер!");
                    return null;
                }
            }
            else
            {
                Console.WriteLine("Не корректный номер!");
                return null;
            }
        }

        private Train CreateTrain(Direction direction, int passengersCount)
        {
            int trainCapacity = 0;
            var vagons = new List<Vagon>();

            while (trainCapacity < passengersCount)
            {
                Console.WriteLine("Формирование состава:");
                Console.WriteLine($"Текущая вместимость состава (пас.): {trainCapacity} из {passengersCount} , кол-во вагонов : {vagons.Count}\n");
                Console.WriteLine("Выберите тип вагона для добавления к составу :");
                Vagon.ShowInfo();
                string vagonTypeNumber = Console.ReadLine();
                VagonType vagonType = new VagonType();

                switch (vagonTypeNumber)
                {
                    case Vagon.CommandReservedSeatVagonType:
                        vagonType = VagonType.ReservedSeat;
                        break;
                    case Vagon.CommandCoupeVagonType:
                        vagonType = VagonType.Coupe;
                        break;
                    case Vagon.CommandSVVagonType:
                        vagonType = VagonType.SV;
                        break;
                    default:
                        vagonType = VagonType.ReservedSeat;
                        break;
                }

                int occupiedPlaces = 0;
                trainCapacity += (int)vagonType;

                if (trainCapacity <= passengersCount)
                {
                    occupiedPlaces = (int)vagonType;
                }
                else
                {
                    occupiedPlaces = trainCapacity - passengersCount;
                }

                vagons.Add(new Vagon(vagonType, occupiedPlaces));
                Draw.DrawTrainStatus(_trains);
            }

            return new Train(direction, vagons, passengersCount);
        }

        private List<City> CreateCitys()
        {
            return new List<City> { new City("Белгород", 50.595413, 36.587258), new City("Москва", 55.755793, 37.617134), new City("Санкт-Петербург", 59.938799, 30.314093),
                      new City("Нижний Новгород", 56.326769, 44.006565), new City("Казань", 55.796043, 49.106094), new City("Сочи", 43.585419, 39.722925),
                      new City("Екатеринбург", 56.837958, 60.597114), new City("Новосибирск", 55.030329, 82.919439), new City("Владивосток", 43.115421, 131.885624) };
        }

        private List<City> CopyListCity(List<City> cities)
        {
            var newList = new List<City>();

            foreach (var city in cities)
            {
                newList.Add(city);
            }

            return newList;
        }

        private void ShowCities(List<City> cities)
        {
            for (int i = 0; i < cities.Count; i++)
            {
                Console.WriteLine($"{i} - {cities[i].Name}");
            }
        }

        private int GetNuber()
        {
            int number;
            string userInput = Console.ReadLine();

            if (int.TryParse(userInput, out number) == false)
            {
                Console.WriteLine("Не корректное значение!");
                return -1;
            }

            return number;
        }

        private bool IsValidNuber(int number, List<City> cities)
        {
            return number >= 0 && number < cities.Count;
        }
    }
}