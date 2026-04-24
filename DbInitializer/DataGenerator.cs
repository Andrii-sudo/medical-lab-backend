using Bogus;
using Bogus.DataSets;
using LabAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;


namespace DbInitializer;

internal static class DataGenerator
{
    const string SourcePath = "Source.json";

    private static readonly Random _random = new Random();

    private static int _minOfficesOnCity = 10;

    private static int _maxOfficesOnCity = 15;

    private static int _employeeCnt = 250;

    private static int _daysRange = 30; // -30 & +30

    internal static void ClearDatabase(MedicalLabsContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.Migrate();
    }

    internal static void ParseSource(MedicalLabsContext context)
    {
        using StreamReader reader = new StreamReader(SourcePath);

        string json = reader.ReadToEnd();
        List<SourceItem> items = JsonSerializer.Deserialize<List<SourceItem>>(json)
            ?? throw new InvalidOperationException($"{SourcePath} is empty or has invalid format");

        // Для уникнення дублуювання
        var addedAnalyses = new Dictionary<string, Analysis>();
        var addedParameters = new Dictionary<(string, string), Parameter>();
        var addedParamNorms = new HashSet<(string, string, byte, byte, string)>();

        foreach (var it in items)
        {
            if (it.Question.Contains("with the condition"))
            {
                continue;
            }

            int i = 0;
            i = MoveToNextQuote(it.Question, i);
            (i, string analysisName) = ReadQuote(it.Question, i);

            i = MoveToNextQuote(it.Question, i);
            (i, string paramUnit) = ReadQuote(it.Question, i);

            i = MoveToNextQuote(it.Question, i);
            (i, string sampleType) = ReadQuote(it.Question, i);

            i = MoveToNextQuote(it.Question, i);
            (i, string rawGender) = ReadQuote(it.Question, i);
            string gender = MapGender(rawGender);

            i = MoveToNextQuote(it.Question, i);
            (i, string ageGroup) = ReadQuote(it.Question, i);
            var (minAge, maxAge) = MapAgeGroup(ageGroup);


            string paramName = analysisName;
            if (it.Question.Contains("in the category"))
            {
                i = MoveToNextQuote(it.Question, i);
                (i, paramName) = ReadQuote(it.Question, i);
            }

            var (minValue, maxValue) = GetNormRange(it.Answer);


            Analysis? analysisEntity;
            if (!addedAnalyses.TryGetValue(analysisName, out analysisEntity))
            {
                analysisEntity = new Analysis
                {
                    Name = analysisName,
                    Price = Math.Round(150m + (decimal)_random.NextDouble() * (4000m - 150m), 1),
                    SampleType = sampleType,
                    ExpiryDays = (byte)_random.Next(5, 30)
                };

                context.Analyses.Add(analysisEntity);

                addedAnalyses.Add(analysisName, analysisEntity);
            }

            var paramKey = (analysisName, paramName);
            Parameter? parameterEntity;
            if (!addedParameters.TryGetValue(paramKey, out parameterEntity))
            {
                parameterEntity = new Parameter
                {
                    ParameterName = paramName,
                    Unit = paramUnit,
                    Analysis = analysisEntity
                };

                context.Parameters.Add(parameterEntity);

                addedParameters.Add(paramKey, parameterEntity);
            }

            var paramNormKey = (analysisName, paramName, minAge, maxAge, gender);
            if (!addedParamNorms.Contains(paramNormKey))
            {
                ParameterNorm parameterNorm = new ParameterNorm
                {
                    Gender = gender,
                    AgeMin = minAge,
                    AgeMax = maxAge,
                    MinValue = minValue,
                    MaxValue = maxValue,
                    Parameter = parameterEntity
                };

                context.ParameterNorms.Add(parameterNorm);

                addedParamNorms.Add(paramNormKey);
            }
        }
        context.SaveChanges(); // Ключі згенеруються тут


        static int MoveToNextQuote(string question, int i)
        {
            while (question[i] != '\'' && i < question.Length)
            {
                i++;
            }

            return ++i;
        }

        static (int, string) ReadQuote(string question, int i)
        {
            StringBuilder sb = new StringBuilder();
            while (question[i] != '\'' && i < question.Length)
            {
                sb.Append(question[i]);
                i++;
            }

            return (++i, sb.ToString());
        }

        static string MapGender(string gender)
        {
            switch (gender)
            {
                case "Male":
                    return "M";
                case "Female":
                    return "F";
                default: // 'any gender'
                    return "A";
            }
        }

        static (byte, byte) MapAgeGroup(string ageGroup)
        {
            switch (ageGroup)
            {
                case "Infant":
                    return (0, 0);
                case "Child":
                    return (1, 17);
                case "Adult":
                    return (18, 255);
                default: // any age group
                    return (0, 255);
            }
        }

        static (decimal?, decimal?) GetNormRange(string answer)
        {
            string cleanAnswer = answer.Replace("\n", "");

            decimal? minValue = null;
            decimal? maxValue = null;
            if (cleanAnswer[0] == '<')
            {
                maxValue = decimal.Parse(cleanAnswer.Trim('<'), CultureInfo.InvariantCulture);
            }
            else if (cleanAnswer[0] == '>')
            {
                minValue = decimal.Parse(cleanAnswer.Trim('>'), CultureInfo.InvariantCulture);
            }
            else if (cleanAnswer.Contains('-'))
            {
                string[] parts = cleanAnswer.Split('-');
                minValue = decimal.Parse(parts[0], CultureInfo.InvariantCulture);
                maxValue = decimal.Parse(parts[1], CultureInfo.InvariantCulture);
            }
            else
            {
                decimal exactValue = decimal.Parse(cleanAnswer, CultureInfo.InvariantCulture);
                minValue = exactValue;
                maxValue = exactValue;
            }

            return (minValue, maxValue);
        }
    }

    internal static void GenerateOffices(MedicalLabsContext context)
    {
        string[] cities = { "Київ", "Львів", "Харків", "Одеса", "Дніпро" };
        string[] types  = { "collection", "analysis", "mixed" };

        var faker = new Faker("uk");
        var offices = new List<Office>();

        foreach (var city in cities)
        {
            short number = 1;
            int count = _random.Next(_minOfficesOnCity, _maxOfficesOnCity);
            for (int i = 0; i < count; i++)
            {
                offices.Add(new Office
                {
                    Number = number++,
                    City = city,
                    Address = faker.Address.StreetAddress(),
                    Type = faker.PickRandom(types)
                });
            }
        }

        context.Offices.AddRange(offices);
        context.SaveChanges();

        
        var officesSchedules = new List<OfficeSchedule>();
        int[] openTimes = { 7, 8, 9, 10 };
        int[] closeTimes = { 18, 19, 20, 21 };

        foreach (var office in offices)
        {
            for (byte day = (byte)DayOfWeek.Sunday; day <= (byte)DayOfWeek.Saturday; day++)
            {
                officesSchedules.Add(new OfficeSchedule
                {
                    Office = office,
                    DayOfWeek = day,
                    OpenTime = new TimeOnly(faker.PickRandom(openTimes), 0),
                    CloseTime = new TimeOnly(faker.PickRandom(closeTimes), 0)
                });
            }
        }

        context.OfficeSchedules.AddRange(officesSchedules);
        context.SaveChanges();
    }

    internal static void GenerateEmployees(MedicalLabsContext context)
    {
        var offices = context.Offices.Include(o => o.OfficeSchedules).ToList();

        var maleEmployeesFaker = new Faker<Employee>("uk")
            .RuleFor(e => e.FirstName, f => f.Name.FirstName(Name.Gender.Male))
            .RuleFor(e => e.LastName, f => f.Name.LastName(Name.Gender.Male))
            .RuleFor(e => e.MiddleName, f => f.PickRandom(f.Name.FirstName(Name.Gender.Male) + "ович", null))
            .RuleFor(e => e.Phone, f => f.Phone.PhoneNumber("0#########"))
            .RuleFor(e => e.Email, f => f.Internet.Email());

        var femaleEmployeesFaker = new Faker<Employee>("uk")
            .RuleFor(e => e.FirstName, f => f.Name.FirstName(Name.Gender.Female))
            .RuleFor(e => e.LastName, f => f.Name.LastName(Name.Gender.Female))
            .RuleFor(e => e.MiddleName, f => f.PickRandom(f.Name.FirstName(Name.Gender.Female) + "івна", null))
            .RuleFor(e => e.Phone, f => f.Phone.PhoneNumber("0#########"))
            .RuleFor(e => e.Email, f => f.Internet.Email());


        var employees = maleEmployeesFaker.Generate(_employeeCnt / 2);
        employees.AddRange(femaleEmployeesFaker.Generate(_employeeCnt / 2));

        context.Employees.AddRange(employees);
        context.SaveChanges();

        foreach (var employee in employees)
        {
            var assignedOffice = offices[_random.Next(offices.Count)];

            var days = Enumerable.Range((int)DayOfWeek.Sunday, 7)
                .OrderBy(_ => _random.Next())
                .Take(5)
                .Select(d => (byte)d);

            foreach (var day in days)
            {
                var officeSchedule = assignedOffice.OfficeSchedules.Single(os => os.DayOfWeek == day);
                context.EmployeeSchedules.Add(new EmployeeSchedule
                {
                    Employee = employee,
                    Office = assignedOffice,
                    DayOfWeek = day,
                    StartTime = officeSchedule.OpenTime,
                    EndTime = officeSchedule.CloseTime
                });
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            for (int day = -_daysRange; day <= _daysRange; day++)
            {
                var date = today.AddDays(day);
                string shiftType;

                if (date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday)
                {
                    shiftType = "day_off";
                }
                else
                {
                    int chance = _random.Next(100);
                    if (chance < 5)
                    {
                        shiftType = "sick_leave";
                    }
                    else if (chance < 15)
                    {
                        shiftType = "vacation";
                    }
                    else
                    {
                        shiftType = "work";
                    }
                }

                var shift = new EmployeeShift
                {
                    Employee = employee,
                    ShiftDate = date,
                    ShiftType = shiftType
                };

                if (shiftType == "work")
                {
                    shift.Office = assignedOffice;
                    shift.StartTime = new TimeOnly(8, 0);
                    shift.EndTime = new TimeOnly(17, 0);
                }
                else // Для решти це відстуність у відділенні
                {
                    shift.Office = null;
                    shift.StartTime = null;
                    shift.EndTime = null;
                }

                context.EmployeeShifts.Add(shift);
            }
        }

        context.SaveChanges();
    }

    internal static void GeneratePatients(MedicalLabsContext context)
    {
        var patientsFaker = new Faker<Patient>("uk")
            .RuleFor(p => p.Gender, f => f.PickRandom("M", "F"))
            .RuleFor(p => p.FirstName, (f, p) => 
                f.Name.FirstName(p.Gender == "M" 
                    ? Name.Gender.Male 
                    : Name.Gender.Female))
            .RuleFor(p => p.LastName, (f, p) => 
                f.Name.LastName(p.Gender == "M"
                    ? Name.Gender.Male
                    : Name.Gender.Female))
            .RuleFor(p => p.MiddleName, (f, p) => 
                f.PickRandom(
                    f.Name.FirstName(Name.Gender.Male) + 
                    (p.Gender == "M" ? "ович" : "івна"), 
                    null))
            .RuleFor(p => p.BirthDate,f => 
                DateOnly.FromDateTime(
                    f.Date.Past(80, DateTime.Now.AddMonths(-2))))
            .RuleFor(p => p.Email, (f, p) => 
                f.PickRandom(
                    f.Internet.Email(p.FirstName, p.LastName), 
                    null))
            .RuleFor(p => p.Phone, f =>
                f.Phone.PhoneNumber("0#########"));

        var patients = patientsFaker.Generate(3000);

        context.Patients.AddRange(patients);
        
        context.SaveChanges();
    }

    internal static void GenerateAppointmentsAndOrders(MedicalLabsContext context)
    {
        var patients = context.Patients.ToList();
        var offices = context.Offices.Include(o => o.OfficeSchedules).ToList();
        var analyses = context.Analyses
            .Include(a => a.Parameters)
            .ThenInclude(p => p.ParameterNorms)
            .ToList();

        string[] purposes = ["first_visit", "sample", "results"];
        var today = DateOnly.FromDateTime(DateTime.Today);

        foreach (var patient in patients.Take(3000))
        {
            int count = _random.Next(2, 18);
            LabOrder? lastOrder = null;

            for (int i = 0; i < count; i++)
            {
                var office = offices[_random.Next(offices.Count)];

                var visitDate = today.AddDays(_random.Next(-_daysRange, _daysRange));
                var dayOfWeek = (byte) visitDate.DayOfWeek;

                var officeSchedule = office.OfficeSchedules.FirstOrDefault(os => os.DayOfWeek == dayOfWeek);
                if (officeSchedule == null) // Якщо випав день в який відділення не працює
                {
                    i--;
                    continue;
                }

                var hour = _random.Next(officeSchedule.OpenTime.Hour, officeSchedule.CloseTime.Hour - 1);
                var minute = _random.Next(0, 4) * 15;
                
                string purpose = purposes[_random.Next(purposes.Length)];
                string status = visitDate < today
                    ? _random.Next(100) < 80 ? "completed" : "cancelled"
                    : "pending";

                context.Appointments.Add(new Appointment
                {
                    Patient = patient,
                    Office = office,
                    VisitDate = visitDate,
                    VisitTime = new TimeOnly(hour, minute),
                    Purpose = purpose,
                    Status = status
                });


                int level = 0;
                if (purpose == "first_visit")
                {
                    if (status == "completed") level = 1;
                    else level = 0; // Якщо всі решта + first-visit -> нічого
                }
                else if (purpose == "sample")
                {
                    if (status == "completed") level = 2; // замовлення + зразок (з expirydate) + пусті результати
                    else level = 1; // всі решта + sample -> теж саме що і completed + first-visit
                }
                else if (purpose == "results")
                {
                    level = 3; // замовлення + зразок + результат заповнений
                }

                if (level == 0) continue;

                var visitDateTime = visitDate.ToDateTime(new TimeOnly(hour, minute));
                var chosenAnalyses = analyses.OrderBy(_ => _random.Next()).Take(_random.Next(1, 3)).ToList();

                var rawOrderDate = level == 3
                    ? visitDateTime.AddDays(-_random.Next(2, 10))
                    : visitDateTime.AddDays(-_random.Next(1, 5));
                var orderDate = rawOrderDate > DateTime.Now
                    ? DateTime.Now.AddHours(-_random.Next(1, 72))
                    : rawOrderDate;

                string orderStatus = level switch { 1 => "unpaid", 2 => "in_progress", 3 => "completed", _ => "pending" };

                lastOrder = new LabOrder
                {
                    CreatedDate = orderDate,
                    Status = orderStatus,
                    TotalPrice = (int)chosenAnalyses.Sum(a => a.Price),
                    Patient = patient,
                    Office = office,
                    OrderAnalyses = chosenAnalyses.Select(a => new OrderAnalysis { Analysis = a }).ToList()
                };

                int patientAge = today.Year - patient.BirthDate.Year;
                if (patient.BirthDate.AddYears(patientAge) > today) patientAge--;

                foreach (var analysis in chosenAnalyses)
                {
                    DateTime? collectionDate = level >= 2 ? (level == 3 ? orderDate : visitDateTime) : null;

                    DateOnly? expiryDate = collectionDate.HasValue
                        ? DateOnly.FromDateTime(collectionDate.Value).AddDays(analysis.ExpiryDays)
                        : null;

                    string sampleStatus = level switch { 1 => "waiting", 2 => "collected", 3 => "analyzed", _ => "waiting" };

                    var sample = new Sample
                    {
                        CollectionDate = collectionDate,
                        ExpiryDate = expiryDate,
                        Status = sampleStatus,
                        OrderNumberNavigation = lastOrder
                    };
                    context.Samples.Add(sample);

                    var result = new Result
                    {
                        ResultDate = level == 3 ? orderDate.AddDays(1) : null,
                        Status = "pending", // Зміниться на normal/abnormal тільки для 3 рівня
                        Sample = sample,
                        Analysis = analysis
                    };
                    context.Results.Add(result);

                    // заповнення параметрів (тільки для Level 3)
                    bool allNormal = true;
                    foreach (var param in analysis.Parameters)
                    {
                        var norm = param.ParameterNorms.FirstOrDefault(n =>
                            n.AgeMin <= patientAge && patientAge <= n.AgeMax &&
                            (n.Gender == patient.Gender || n.Gender == "A"));

                        decimal? measured = null;
                        bool? isNormal = null;

                        if (level == 3)
                        {
                            isNormal = _random.Next(100) < 80;
                            if (!isNormal.Value) allNormal = false;
                            measured = GenerateMeasuredValue(norm, isNormal.Value);
                        }

                        context.ParameterResults.Add(new ParameterResult
                        {
                            Parameter = param,
                            Result = result,
                            MeasuredValue = measured,
                            IsNormal = isNormal
                        });
                    }

                    if (level == 3) result.Status = allNormal ? "normal" : "abnormal";
                }
                context.LabOrders.Add(lastOrder);
            }
        }

        context.SaveChanges();


        static decimal? GenerateMeasuredValue(ParameterNorm? norm, bool isNormal)
        {
            if (norm is null) return null;

            decimal min = norm.MinValue ?? 0m;
            decimal max = norm.MaxValue ?? min * 2;

            if (min == 0 && max == 0) return null;

            if (isNormal)
            {
                return Math.Round(min + (decimal)_random.NextDouble() * (max - min), 3);
            }

            if (_random.Next(2) == 0)
            {
                return Math.Round(min * (decimal)(0.3 + _random.NextDouble() * 0.5), 3);
            }
            else
            {
                return Math.Round(max * (decimal)(1.2 + _random.NextDouble() * 0.8), 3);
            }
        }
    }

    internal static void AssignUsersToFirstRecords(MedicalLabsContext context)
    {
        var admRole = context.Roles.FirstOrDefault(r => r.NormalizedName == "ADMIN");
        if (admRole == null)
        {
            admRole = new IdentityRole<int> { Name = "Admin", NormalizedName = "ADMIN" };
            context.Roles.Add(admRole);
        }

        var empRole = context.Roles.FirstOrDefault(r => r.NormalizedName == "EMPLOYEE");
        if (empRole == null)
        {
            empRole = new IdentityRole<int> { Name = "Employee", NormalizedName = "EMPLOYEE" };
            context.Roles.Add(empRole);
        }

        var patRole = context.Roles.FirstOrDefault(r => r.NormalizedName == "PATIENT");
        if (patRole == null)
        {
            patRole = new IdentityRole<int> { Name = "Patient", NormalizedName = "PATIENT" };
            context.Roles.Add(patRole);
        }
        context.SaveChanges();

        var patient  = context.Patients.OrderBy(p => p.Id).First();
        var admin    = context.Employees.OrderBy(e => e.Id).First();
        var employee = context.Employees.OrderBy(e => e.Id).Skip(1).First();

        string patPassword = "patient123";
        string admPassword = "admin123";
        string empPassword = "employee123";

        var hasher = new PasswordHasher<AppUser>();

        var admUser = new AppUser
        {
            UserName = admin.Email,
            NormalizedUserName = admin.Email.ToUpper(),
            Email = admin.Email,
            NormalizedEmail = admin.Email.ToUpper(),
            EmployeeId = admin.Id,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };
        admUser.PasswordHash = hasher.HashPassword(admUser, admPassword);

        var empUser = new AppUser
        {
            UserName = employee.Email,
            NormalizedUserName = employee.Email.ToUpper(),
            Email = employee.Email,
            NormalizedEmail = employee.Email.ToUpper(),
            EmployeeId = employee.Id, 
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };
        empUser.PasswordHash = hasher.HashPassword(empUser, empPassword);

        var patUser = new AppUser
        {
            UserName = patient.Phone,
            NormalizedUserName = patient.Phone,
            PhoneNumber = patient.Phone,
            PatientId = patient.Id, 
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };
        patUser.PasswordHash = hasher.HashPassword(patUser, patPassword);

        context.Users.AddRange(admUser, empUser, patUser);
        context.SaveChanges();

        context.UserRoles.AddRange(
            new IdentityUserRole<int> { UserId = admUser.Id, RoleId = admRole.Id },
            new IdentityUserRole<int> { UserId = empUser.Id, RoleId = empRole.Id },
            new IdentityUserRole<int> { UserId = patUser.Id, RoleId = patRole.Id }
        );

        context.SaveChanges();

        Console.WriteLine($"Patient,  phone: {patUser.PhoneNumber} password: {patPassword}");
        Console.WriteLine($"Employee, email: {empUser.Email} password: {empPassword}");
        Console.WriteLine($"Admin,    email: {admUser.Email} password: {admPassword}");
    }
}
