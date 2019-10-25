using Fordonsbesiktning.Domain;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading;
using static System.Console;

namespace Fordonsbesiktning
{
    class Program
    {
        static string connectionString = "Server=(local);Database=fordonbesiktning;Integrated Security=True";
        static void Main(string[] args)
        {

            bool shouldNotExit = true;
            while (shouldNotExit)
            {
                WriteLine(" 1. Ny Reservation");
                WriteLine(" 2. List Resevationer");
                WriteLine(" 3. Utför besiktning");
                WriteLine(" 4. Lista besiktningar");
                WriteLine(" 5.Avsluta");

                ConsoleKeyInfo keyPressed = ReadKey(true);
                Clear();
                switch (keyPressed.Key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        WriteLine("Registreringsnummer:");
                        string Registreringsnummer = ReadLine();
                        WriteLine(" Datum(yyyy - MM - dd hh: mm:");
                        DateTime datum = DateTime.Parse(ReadLine());
                        WriteLine(" Är detta korrekt ? (J)a eller(N)ej");

                        

                        bool isTrue = true;

                        do
                        {
                            keyPressed = ReadKey(true);
                            switch (keyPressed.Key)
                            {
                                case ConsoleKey.J:

                                    isTrue = false;
                                    //Reservation reservation = FindReservation(Registreringsnummer);
                                    Reservation reservation = new Reservation(Registreringsnummer, datum);
                                    SaveReservation(reservation);
                                    WriteLine("Reservation utförd");
                                    Thread.Sleep(2000);

                                    

                                    break;

                                case ConsoleKey.N:
                                    isTrue = false;
                                    break;
                            }

                        } while (isTrue);
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:

                        List<Reservation> reservationList = FetchAllReservation();
                        Console.WriteLine("  Fordon                   Datum    ");
                        Console.WriteLine("----------------------------------------------");
                        foreach (Reservation reservation in reservationList)
                        {
                            Console.WriteLine($"{reservation.RegistrationNumber}         {reservation.Date}");
                        }
                        WriteLine();
                        WriteLine("Press key to continue");
                        ReadKey();
                        break;

                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        WriteLine("Registreringsnummer:");
                        string registrationNumber = ReadLine();
                        Reservation theReservation = FindReservation(registrationNumber);

                        if (theReservation == null)
                        {
                            WriteLine("Reservation Saknas");
                            Thread.Sleep(2000);
                        }

                        else
                        {
                            WriteLine("Fordonet godkänt ? (J)a eller(N)ej");
                            bool validInput = true;
                            Inspection newinspection;

                            // inspectionsList.Add(newinspection);

                            do
                            {
                                keyPressed = ReadKey(true);
                                switch (keyPressed.Key)
                                {
                                    case ConsoleKey.J:
                                        newinspection = new Inspection(registrationNumber, true);
                                        validInput = false;
                                        SaveInspection(newinspection);

                                        WriteLine("Godkänd");
                                        Thread.Sleep(2000);


                                        break;

                                    case ConsoleKey.N:
                                        validInput = false;
                                        newinspection = new Inspection(registrationNumber, false);
                                        SaveInspection(newinspection);
                                        WriteLine(" Ej godkänd ");
                                        Thread.Sleep(2000);
                                        break;
                                }

                            } while (validInput);

                        }
                        break;

                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        List<Inspection> inspectionList = FetchAllInspection();

                        Console.WriteLine("   Fordon                          Utfört Datum                       Resultat");
                        Console.WriteLine("--------------------------------------------------------------------------------------");
                       
                        foreach (Inspection inspection in inspectionList)
                        {

                            if (inspection.IsApproved)
                            {
                            WriteLine($"{inspection.RegistrationNumber}        {inspection.PerformedAt}                            Godkänd   ");
                            }
                            else
                            {
                              WriteLine($"{inspection.RegistrationNumber}      {inspection.PerformedAt}                            Ej Godkänd   ");
                            }
                        }
                        WriteLine();
                        WriteLine("Press key to continue");
                        ReadKey();
                        break;

                    case ConsoleKey.D5:
                    case ConsoleKey.NumPad5:
                        shouldNotExit = false;
                        break;
                }
                Clear();
            }

        }


        private static Reservation FindReservation(string registerNummber)
        {
            List<Reservation> reservationList = FetchAllReservation();

            foreach (Reservation reservation in reservationList)
            {
                if (registerNummber == reservation.RegistrationNumber)
                {
                    return reservation;
                }
            }

            return null;
        }

        private static Inspection FindInspection(string registerNummber)
        {
            List<Inspection> inspectionsList = new List<Inspection>();

            foreach (Inspection inspection in inspectionsList)
            {
                if (registerNummber == inspection.RegistrationNumber)
                {
                    return inspection;
                }
            }

            return null;
        }
        static void SaveReservation(Reservation reservation)
        {
            string cmdText = @"
              INSERT INTO Reservation (
              RegistrationNumber,
              Date)
              Values(
              @RegistrationNumber,
              @Date
              )";
            SqlConnection connection = new SqlConnection(connectionString);
            using (SqlCommand command = new SqlCommand(cmdText, connection))
            {
                
                command.Parameters.AddWithValue("RegistrationNumber", reservation.RegistrationNumber);
                command.Parameters.AddWithValue("Date", reservation.Date);

                connection.Open();

                command.ExecuteNonQuery();

                connection.Close();
            }

        }

        static List<Reservation> FetchAllReservation()
        {
            List<Reservation> reservationList = new List<Reservation>();

            string cmdText = @"
               SELECT
                Id,
                RegistrationNumber,
                Date
               FROM Reservation
            ";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(cmdText, connection))
            {
                connection.Open();

                SqlDataReader dataReader = command.ExecuteReader();
              //  Reservation newReservation = new Reservation(registrationNumber, date);

               // reservationList.Add(newReservation);


                while (dataReader.Read())
                {
                    int id = int.Parse(dataReader["Id"].ToString());
                    string registrationNumber = dataReader["RegistrationNumber"].ToString();
                    DateTime date = DateTime.Parse(dataReader["Date"].ToString());

                    Reservation reservation = new Reservation(

                        registrationNumber,
                        date);

                    reservationList.Add(reservation);
                }

                connection.Close();
            }

            return reservationList;
        }

        static void SaveInspection(Inspection inspection)
        {
            string cmdText = @"
              INSERT INTO Inspection (
             
              RegistrationNumber,
              PerformedAt,
              IsApproved)
              Values(
             
              @RegistrationNumber,
              @PerformedAt,
              @IsApproved
              )";


            SqlConnection connection = new SqlConnection(connectionString);
            using (SqlCommand command = new SqlCommand(cmdText, connection))
            {
                //command.Parameters.AddWithValue("Id", inspection.Id);
                command.Parameters.AddWithValue("RegistrationNumber", inspection.RegistrationNumber);
                command.Parameters.AddWithValue("PerformedAt", DateTime.Now);
                command.Parameters.AddWithValue("IsApproved", inspection.IsApproved);

                connection.Open();

                command.ExecuteNonQuery();

                connection.Close();
            }

        }
        static List<Inspection> FetchAllInspection()
        {
            List<Inspection> inspectionList = new List<Inspection>();

            string cmdText = @"
               SELECT Id,
                RegistrationNumber,
                PerformedAt,
                IsApproved
               FROM Inspection
            ";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(cmdText, connection))
            {
                connection.Open();

                SqlDataReader dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    int id = int.Parse(dataReader["Id"].ToString());
                    string registrationNumber = dataReader["RegistrationNumber"].ToString();
                    DateTime performAt = DateTime.Parse(dataReader["PerformedAt"].ToString());
                    bool isApproved = bool.Parse(dataReader["IsApproved"].ToString());

                    Inspection inspection = new Inspection(
                        id,
                        registrationNumber,
                        performAt,
                        isApproved);

                    inspectionList.Add(inspection);
                }

                connection.Close();
            }

            return inspectionList;
        }
    }
}