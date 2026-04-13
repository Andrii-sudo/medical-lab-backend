using DbInitializer;
using LabAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var options = new DbContextOptionsBuilder<MedicalLabsContext>()
    .UseSqlServer(config["ConnectionStrings:DefaultConnection"])
    .Options;

using var context = new MedicalLabsContext(options);

if (!context.Database.CanConnect())
{
    Console.WriteLine("DB not connected");
    return;
}

Console.WriteLine("DB is connected");

DataGenerator.ClearDatabase(context);
DataGenerator.ParseSource(context);
DataGenerator.GenerateOffices(context);
DataGenerator.GenerateEmployees(context);
DataGenerator.GeneratePatients(context);
DataGenerator.GenerateAppointmentsAndOrders(context);