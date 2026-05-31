using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResidenceMngSys.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    İd = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.İd);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    İd = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_İd = table.Column<int>(type: "int", nullable: false),
                    Roles_İd = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.İd);
                });

            migrationBuilder.CreateTable(
                name: "Announcements",
                columns: table => new
                {
                    İd = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contents = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImportanceStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcements", x => x.İd);
                    table.ForeignKey(
                        name: "FK_Announcements_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Apartment",
                columns: table => new
                {
                    İd = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlockNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FloorNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApartmentNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SquareMeters = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    İsOccupied = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apartment", x => x.İd);
                    table.ForeignKey(
                        name: "FK_Apartment_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GeneralSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResidenceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneralSettings_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PaymentsInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IbanNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountHolder = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentsInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentsInfo_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BalanceRequests",
                columns: table => new
                {
                    İd = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Apartment_İd = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    ReceiptİmagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Explanation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BalanceRequests", x => x.İd);
                    table.ForeignKey(
                        name: "FK_BalanceRequests_Apartment_Apartment_İd",
                        column: x => x.Apartment_İd,
                        principalTable: "Apartment",
                        principalColumn: "İd");
                    table.ForeignKey(
                        name: "FK_BalanceRequests_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Dues",
                columns: table => new
                {
                    İd = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Apartment_İd = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Months = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DuesPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RemainingDebt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    İsPaid = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dues", x => x.İd);
                    table.ForeignKey(
                        name: "FK_Dues_Apartment_Apartment_İd",
                        column: x => x.Apartment_İd,
                        principalTable: "Apartment",
                        principalColumn: "İd");
                    table.ForeignKey(
                        name: "FK_Dues_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FaultAndRequest",
                columns: table => new
                {
                    İd = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Apartment_İd = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdminAnswer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompletionAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaultAndRequest", x => x.İd);
                    table.ForeignKey(
                        name: "FK_FaultAndRequest_Apartment_Apartment_İd",
                        column: x => x.Apartment_İd,
                        principalTable: "Apartment",
                        principalColumn: "İd");
                    table.ForeignKey(
                        name: "FK_FaultAndRequest_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    İd = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Apartment_İd = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentPreference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    İsActive = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.İd);
                    table.ForeignKey(
                        name: "FK_Users_Apartment_Apartment_İd",
                        column: x => x.Apartment_İd,
                        principalTable: "Apartment",
                        principalColumn: "İd");
                    table.ForeignKey(
                        name: "FK_Users_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Wallet",
                columns: table => new
                {
                    İd = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Apartment_İd = table.Column<int>(type: "int", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallet", x => x.İd);
                    table.ForeignKey(
                        name: "FK_Wallet_Apartment_Apartment_İd",
                        column: x => x.Apartment_İd,
                        principalTable: "Apartment",
                        principalColumn: "İd");
                    table.ForeignKey(
                        name: "FK_Wallet_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WalletTransactions",
                columns: table => new
                {
                    İd = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Dues_İd = table.Column<int>(type: "int", nullable: true),
                    Wallet_İd = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTransactions", x => x.İd);
                    table.ForeignKey(
                        name: "FK_WalletTransactions_Dues_Dues_İd",
                        column: x => x.Dues_İd,
                        principalTable: "Dues",
                        principalColumn: "İd");
                    table.ForeignKey(
                        name: "FK_WalletTransactions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WalletTransactions_Wallet_Wallet_İd",
                        column: x => x.Wallet_İd,
                        principalTable: "Wallet",
                        principalColumn: "İd");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_TenantId",
                table: "Announcements",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Apartment_TenantId",
                table: "Apartment",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_BalanceRequests_Apartment_İd",
                table: "BalanceRequests",
                column: "Apartment_İd");

            migrationBuilder.CreateIndex(
                name: "IX_BalanceRequests_TenantId",
                table: "BalanceRequests",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Dues_Apartment_İd",
                table: "Dues",
                column: "Apartment_İd");

            migrationBuilder.CreateIndex(
                name: "IX_Dues_TenantId",
                table: "Dues",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_FaultAndRequest_Apartment_İd",
                table: "FaultAndRequest",
                column: "Apartment_İd");

            migrationBuilder.CreateIndex(
                name: "IX_FaultAndRequest_TenantId",
                table: "FaultAndRequest",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralSettings_TenantId",
                table: "GeneralSettings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentsInfo_TenantId",
                table: "PaymentsInfo",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Apartment_İd",
                table: "Users",
                column: "Apartment_İd",
                unique: true,
                filter: "[Apartment_İd] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId",
                table: "Users",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_Apartment_İd",
                table: "Wallet",
                column: "Apartment_İd",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_TenantId",
                table: "Wallet",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_Dues_İd",
                table: "WalletTransactions",
                column: "Dues_İd");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_TenantId",
                table: "WalletTransactions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_Wallet_İd",
                table: "WalletTransactions",
                column: "Wallet_İd");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Announcements");

            migrationBuilder.DropTable(
                name: "BalanceRequests");

            migrationBuilder.DropTable(
                name: "FaultAndRequest");

            migrationBuilder.DropTable(
                name: "GeneralSettings");

            migrationBuilder.DropTable(
                name: "PaymentsInfo");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WalletTransactions");

            migrationBuilder.DropTable(
                name: "Dues");

            migrationBuilder.DropTable(
                name: "Wallet");

            migrationBuilder.DropTable(
                name: "Apartment");

            migrationBuilder.DropTable(
                name: "Tenants");
        }
    }
}
