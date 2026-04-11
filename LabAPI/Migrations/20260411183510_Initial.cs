using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabAPI.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "analysis",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    sample_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    expiry_days = table.Column<byte>(type: "tinyint", nullable: false),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("analysis_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "employee",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    first_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    middle_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("employee_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "office",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    number = table.Column<short>(type: "smallint", nullable: false),
                    city = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("office_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "patient",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    first_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    middle_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: false),
                    gender = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: false),
                    phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("patient_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "parameter",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    parameter_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    analysis_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("parameter_pkey", x => x.id);
                    table.ForeignKey(
                        name: "parameter_analysis_fkey",
                        column: x => x.analysis_id,
                        principalTable: "analysis",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employee_schedule",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employee_id = table.Column<int>(type: "int", nullable: false),
                    office_id = table.Column<int>(type: "int", nullable: false),
                    day_of_week = table.Column<byte>(type: "tinyint", nullable: false),
                    start_time = table.Column<TimeOnly>(type: "time(0)", precision: 0, nullable: false),
                    end_time = table.Column<TimeOnly>(type: "time(0)", precision: 0, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("est_pkey", x => x.id);
                    table.ForeignKey(
                        name: "est_employee_fkey",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "est_office_fkey",
                        column: x => x.office_id,
                        principalTable: "office",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "employee_shift",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employee_id = table.Column<int>(type: "int", nullable: false),
                    office_id = table.Column<int>(type: "int", nullable: true),
                    shift_date = table.Column<DateOnly>(type: "date", nullable: false),
                    start_time = table.Column<TimeOnly>(type: "time(0)", precision: 0, nullable: true),
                    end_time = table.Column<TimeOnly>(type: "time(0)", precision: 0, nullable: true),
                    shift_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "work")
                },
                constraints: table =>
                {
                    table.PrimaryKey("es_pkey", x => x.id);
                    table.ForeignKey(
                        name: "es_employee_fkey",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "es_office_fkey",
                        column: x => x.office_id,
                        principalTable: "office",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "office_schedule",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    office_id = table.Column<int>(type: "int", nullable: false),
                    day_of_week = table.Column<byte>(type: "tinyint", nullable: false),
                    open_time = table.Column<TimeOnly>(type: "time(0)", precision: 0, nullable: false),
                    close_time = table.Column<TimeOnly>(type: "time(0)", precision: 0, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("office_schedule_pkey", x => x.id);
                    table.ForeignKey(
                        name: "office_schedule_office_fkey",
                        column: x => x.office_id,
                        principalTable: "office",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "appointment",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    patient_id = table.Column<int>(type: "int", nullable: false),
                    office_id = table.Column<int>(type: "int", nullable: false),
                    visit_date = table.Column<DateOnly>(type: "date", nullable: false),
                    visit_time = table.Column<TimeOnly>(type: "time(0)", precision: 0, nullable: false),
                    purpose = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "pending")
                },
                constraints: table =>
                {
                    table.PrimaryKey("appointment_pkey", x => x.id);
                    table.ForeignKey(
                        name: "appointment_office_fkey",
                        column: x => x.office_id,
                        principalTable: "office",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "appointment_patient_fkey",
                        column: x => x.patient_id,
                        principalTable: "patient",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patient",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "lab_order",
                columns: table => new
                {
                    number = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    created_date = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false, defaultValueSql: "(sysdatetime())"),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "unpaid"),
                    total_price = table.Column<int>(type: "int", nullable: false),
                    patient_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("lab_order_pkey", x => x.number);
                    table.ForeignKey(
                        name: "lab_order_patient_fkey",
                        column: x => x.patient_id,
                        principalTable: "patient",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "parameter_norm",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    age_min = table.Column<byte>(type: "tinyint", nullable: false),
                    age_max = table.Column<byte>(type: "tinyint", nullable: false),
                    gender = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: false),
                    min_value = table.Column<decimal>(type: "decimal(10,3)", nullable: true),
                    max_value = table.Column<decimal>(type: "decimal(10,3)", nullable: true),
                    parameter_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("parameter_norm_pkey", x => x.id);
                    table.ForeignKey(
                        name: "parameter_norm_fkey",
                        column: x => x.parameter_id,
                        principalTable: "parameter",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_analysis",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_number = table.Column<int>(type: "int", nullable: false),
                    analysis_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("order_analysis_pkey", x => x.id);
                    table.ForeignKey(
                        name: "order_analysis_analysis_fkey",
                        column: x => x.analysis_id,
                        principalTable: "analysis",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "order_analysis_order_fkey",
                        column: x => x.order_number,
                        principalTable: "lab_order",
                        principalColumn: "number");
                });

            migrationBuilder.CreateTable(
                name: "sample",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    collection_date = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "waiting"),
                    expiry_date = table.Column<DateOnly>(type: "date", nullable: true),
                    order_number = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("sample_pkey", x => x.id);
                    table.ForeignKey(
                        name: "sample_order_fkey",
                        column: x => x.order_number,
                        principalTable: "lab_order",
                        principalColumn: "number");
                });

            migrationBuilder.CreateTable(
                name: "result",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    result_date = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "pending"),
                    conclusion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sample_id = table.Column<int>(type: "int", nullable: false),
                    analysis_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("result_pkey", x => x.id);
                    table.ForeignKey(
                        name: "result_analysis_fkey",
                        column: x => x.analysis_id,
                        principalTable: "analysis",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "result_sample_fkey",
                        column: x => x.sample_id,
                        principalTable: "sample",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "parameter_result",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    measured_value = table.Column<decimal>(type: "decimal(10,3)", nullable: true),
                    is_normal = table.Column<bool>(type: "bit", nullable: true),
                    result_id = table.Column<int>(type: "int", nullable: false),
                    parameter_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("parameter_result_pkey", x => x.id);
                    table.ForeignKey(
                        name: "parameter_result_parameter_fkey",
                        column: x => x.parameter_id,
                        principalTable: "parameter",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "parameter_result_result_fkey",
                        column: x => x.result_id,
                        principalTable: "result",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "analysis_name_unique",
                table: "analysis",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_appointment_office_date",
                table: "appointment",
                columns: new[] { "office_id", "visit_date" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_appointment_patient_id",
                table: "appointment",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EmployeeId",
                table: "AspNetUsers",
                column: "EmployeeId",
                unique: true,
                filter: "[EmployeeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PatientId",
                table: "AspNetUsers",
                column: "PatientId",
                unique: true,
                filter: "[PatientId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "employee_email_unique",
                table: "employee",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "employee_phone_unique",
                table: "employee",
                column: "phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employee_schedule_employee_id",
                table: "employee_schedule",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_schedule_office_id",
                table: "employee_schedule",
                column: "office_id");

            migrationBuilder.CreateIndex(
                name: "es_employee_date_unique",
                table: "employee_shift",
                columns: new[] { "employee_id", "shift_date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employee_shift_office_id",
                table: "employee_shift",
                column: "office_id");

            migrationBuilder.CreateIndex(
                name: "IX_lab_order_patient_id",
                table: "lab_order",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "office_number_city_unique",
                table: "office",
                columns: new[] { "number", "city" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "office_schedule_unique",
                table: "office_schedule",
                columns: new[] { "office_id", "day_of_week" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_order_analysis_analysis_id",
                table: "order_analysis",
                column: "analysis_id");

            migrationBuilder.CreateIndex(
                name: "order_analysis_unique",
                table: "order_analysis",
                columns: new[] { "order_number", "analysis_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_parameter_analysis_id",
                table: "parameter",
                column: "analysis_id");

            migrationBuilder.CreateIndex(
                name: "parameter_name_analysis_unique",
                table: "parameter",
                columns: new[] { "parameter_name", "analysis_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "parameter_norm_unique",
                table: "parameter_norm",
                columns: new[] { "parameter_id", "age_min", "age_max", "gender" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_parameter_result_parameter_id",
                table: "parameter_result",
                column: "parameter_id");

            migrationBuilder.CreateIndex(
                name: "parameter_result_result_parameter_unique",
                table: "parameter_result",
                columns: new[] { "result_id", "parameter_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_patient_name_search",
                table: "patient",
                columns: new[] { "last_name", "first_name", "middle_name" });

            migrationBuilder.CreateIndex(
                name: "patient_phone_unique",
                table: "patient",
                column: "phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_result_sample_lookup",
                table: "result",
                column: "sample_id");

            migrationBuilder.CreateIndex(
                name: "IX_result_analysis_id",
                table: "result",
                column: "analysis_id");

            migrationBuilder.CreateIndex(
                name: "result_sample_analysis_unique",
                table: "result",
                columns: new[] { "sample_id", "analysis_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_sample_order_search",
                table: "sample",
                column: "order_number");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appointment");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "employee_schedule");

            migrationBuilder.DropTable(
                name: "employee_shift");

            migrationBuilder.DropTable(
                name: "office_schedule");

            migrationBuilder.DropTable(
                name: "order_analysis");

            migrationBuilder.DropTable(
                name: "parameter_norm");

            migrationBuilder.DropTable(
                name: "parameter_result");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "office");

            migrationBuilder.DropTable(
                name: "parameter");

            migrationBuilder.DropTable(
                name: "result");

            migrationBuilder.DropTable(
                name: "employee");

            migrationBuilder.DropTable(
                name: "analysis");

            migrationBuilder.DropTable(
                name: "sample");

            migrationBuilder.DropTable(
                name: "lab_order");

            migrationBuilder.DropTable(
                name: "patient");
        }
    }
}
