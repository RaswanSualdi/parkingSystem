using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        ParkingLot parkingLot = null;

        while (true)
        {
            Console.Write("$ ");
            string input = Console.ReadLine();
            string[] args = input.Split(' ');

            if (args.Length == 0)
            {
                Console.WriteLine("Invalid command");
                continue;
            }

            string command = args[0].ToLower();

            switch (command)
            {
                case "create_parking_lot":
                    if (args.Length != 2 || !int.TryParse(args[1], out int totalSlots))
                    {
                        Console.WriteLine("Invalid command format. Usage: create_parking_lot [totalSlots]");
                        continue;
                    }

                    parkingLot = new ParkingLot(totalSlots);
                    Console.WriteLine($"Created a parking lot with {totalSlots} slots");
                    break;

                case "park":
                    if (parkingLot == null)
                    {
                        Console.WriteLine("Please create a parking lot first.");
                        continue;
                    }

                    if (args.Length != 4)
                    {
                        Console.WriteLine("Invalid command format. Usage: park [PlateNumber] [Color] [Type]");
                        continue;
                    }

                    string plateNumber = args[1];
                    string color = args[2];
                    string vehicleTypeStr = args[3];

                    if (!Enum.TryParse(vehicleTypeStr, out VehicleType type))
                    {
                        Console.WriteLine("Invalid vehicle type. Use 'Motor' or 'Mobil'.");
                        continue;
                    }

                    string result = parkingLot.ParkVehicle(new Vehicle(plateNumber, color, type));
                    Console.WriteLine(result);
                    break;


                case "leave":
                    if (parkingLot == null)
                    {
                        Console.WriteLine("Please create a parking lot first.");
                        break;
                    }

                    int slotNumber = int.Parse(args[1]);
                    string leaveResult = parkingLot.Leave(slotNumber);
                    Console.WriteLine(leaveResult);
                    break;

                case "status":
                    if (parkingLot == null)
                    {
                        Console.WriteLine("Please create a parking lot first.");
                        break;
                    }

                    parkingLot.PrintStatus();
                    break;

                case "type_of_vehicles":
                    if (parkingLot == null)
                    {
                        Console.WriteLine("Please create a parking lot first.");
                        break;
                    }

                    string requestedType = args[1];
                    int count = parkingLot.GetVehicleCountByType(requestedType);
                    Console.WriteLine(count);
                    break;

                case "registration_numbers_for_vehicles_with_odd_plate":
                    if (parkingLot == null)
                    {
                        Console.WriteLine("Please create a parking lot first.");
                        break;
                    }

                    string oddPlateNumbers = parkingLot.GetRegistrationNumbersByPlateType("odd");
                    Console.WriteLine(oddPlateNumbers);
                    break;

                case "registration_numbers_for_vehicles_with_even_plate":
                    if (parkingLot == null)
                    {
                        Console.WriteLine("Please create a parking lot first.");
                        break;
                    }

                    string evenPlateNumbers = parkingLot.GetRegistrationNumbersByPlateType("even");
                    Console.WriteLine(evenPlateNumbers);
                    break;

                case "registration_numbers_for_vehicles_with_colour":
                    if (parkingLot == null)
                    {
                        Console.WriteLine("Please create a parking lot first.");
                        break;
                    }

                    string requestedColor = args[1];
                    string colorPlateNumbers = parkingLot.GetRegistrationNumbersByColor(requestedColor);
                    Console.WriteLine(colorPlateNumbers);
                    break;

                case "slot_numbers_for_vehicles_with_colour":
                    if (parkingLot == null)
                    {
                        Console.WriteLine("Please create a parking lot first.");
                        break;
                    }

                    string colorSlotNumbers = parkingLot.GetSlotNumbersByColor(args[1]);
                    Console.WriteLine(colorSlotNumbers);
                    break;

                case "slot_number_for_registration_number":
                    if (parkingLot == null)
                    {
                        Console.WriteLine("Please create a parking lot first.");
                        break;
                    }

                    string regNumber = args[1];
                    string slotNumberResult = parkingLot.GetSlotNumberByRegistrationNumber(regNumber);
                    Console.WriteLine(slotNumberResult);
                    break;

                case "exit":
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Invalid command");
                    break;
            }
        }
    }
}

public enum VehicleType
{
    Motor,
    Mobil
}

public class Vehicle
{
    public string PlateNumber { get; }
    public string Color { get; }
    public VehicleType Type { get; }

    public Vehicle(string plateNumber, string color, VehicleType type)
    {
        PlateNumber = plateNumber;
        Color = color;
        Type = type;
    }
}

public class ParkingLot
{
    private readonly List<Vehicle> vehicles;
    private readonly int totalSlots;

    public ParkingLot(int totalSlots)
    {
        this.totalSlots = totalSlots;
        vehicles = new List<Vehicle>();
    }

    public string ParkVehicle(Vehicle vehicle)
    {
        if (vehicles.Count < totalSlots)
        {
            vehicles.Add(vehicle);
            return $"Allocated slot number: {vehicles.Count}";
        }
        else
        {
            return "Sorry, parking lot is full";
        }
    }

    public string Leave(int slotNumber)
    {
        if (slotNumber > 0 && slotNumber <= vehicles.Count)
        {
            vehicles.RemoveAt(slotNumber - 1);
            return $"Slot number {slotNumber} is free";
        }
        else
        {
            return $"Invalid slot number {slotNumber}";
        }
    }

    public void PrintStatus()
    {
        Console.WriteLine("Slot\tNo.\tType\tRegistration No\tColour");
        for (int i = 0; i < vehicles.Count; i++)
        {
            Console.WriteLine($"{i + 1}\t{vehicles[i].PlateNumber}\t{vehicles[i].Type}\t{vehicles[i].Color}");
        }
    }

    public int GetVehicleCountByType(string type)
    {
        VehicleType vehicleType;
        Enum.TryParse(type, out vehicleType);
        return vehicles.Count(v => v.Type == vehicleType);
    }

    public string GetRegistrationNumbersByPlateType(string plateType)
    {
        bool isOdd = plateType.ToLower() == "odd";

        var filteredVehicles = vehicles
            .Where(v => (int.Parse(v.PlateNumber.Substring(v.PlateNumber.Length - 1)) % 2 == 1) == isOdd)
            .Select(v => v.PlateNumber);

        return string.Join(", ", filteredVehicles);
    }

    public string GetRegistrationNumbersByColor(string color)
    {
        var filteredVehicles = vehicles.Where(v => v.Color.ToLower() == color.ToLower()).Select(v => v.PlateNumber);
        return string.Join(", ", filteredVehicles);
    }

    public string GetSlotNumbersByColor(string color)
    {
        var filteredSlots = vehicles
            .Where(v => v.Color.ToLower() == color.ToLower())
            .Select((v, index) => (index + 1).ToString());

        return string.Join(", ", filteredSlots);
    }

    public string GetSlotNumberByRegistrationNumber(string regNumber)
    {
        int slotNumber = vehicles.FindIndex(v => v.PlateNumber == regNumber) + 1;
        return slotNumber != 0 ? slotNumber.ToString() : "Not found";
    }
}
