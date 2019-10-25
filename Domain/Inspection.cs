using System;

class Inspection
{
    public int Id { get; }
    public string RegistrationNumber { get; }
    public DateTime PerformedAt { get; }
    public bool IsApproved { get; private set; }

    public Inspection(string registrationNumber, bool isApproved)
    {
        RegistrationNumber = registrationNumber;
        PerformedAt = DateTime.Now;
        this.IsApproved = isApproved;   
    }
    // Använd denna konstruktorn för att skapa upp instanser baserat
    // på data som hämtas in från databasen.
    public Inspection(int id, string registrationNumber, DateTime
    performedAt, bool isApproved)
    {
        Id = id;
        RegistrationNumber = registrationNumber;
        PerformedAt = performedAt;
        IsApproved = isApproved;
    }
    public void Approve()
    {
        IsApproved = true;
    }
    public void Failed()
    {
        IsApproved = false;
    }
}