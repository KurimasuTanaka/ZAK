using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorApp.DB.Migrations
{
    /// <inheritdoc />
    public partial class FixedSchema3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "brigades",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    brigadeNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    applicationsIds = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_brigades", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "coefficients",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    parameter = table.Column<string>(type: "TEXT", nullable: false),
                    parameterAlias = table.Column<string>(type: "TEXT", nullable: false),
                    coefficient = table.Column<double>(type: "REAL", nullable: false),
                    infinite = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coefficients", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "districts",
                columns: table => new
                {
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    color = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_districts", x => x.name);
                });

            migrationBuilder.CreateTable(
                name: "addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    streetName = table.Column<string>(type: "TEXT", nullable: false),
                    building = table.Column<string>(type: "TEXT", nullable: false),
                    districtName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_addresses_districts_districtName",
                        column: x => x.districtName,
                        principalTable: "districts",
                        principalColumn: "name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "addressAliases",
                columns: table => new
                {
                    addressId = table.Column<int>(type: "INTEGER", nullable: false),
                    streetAlias = table.Column<string>(type: "TEXT", nullable: false),
                    buildingAlias = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_addressAliases", x => x.addressId);
                    table.ForeignKey(
                        name: "FK_addressAliases_addresses_addressId",
                        column: x => x.addressId,
                        principalTable: "addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AddressesCoordinates",
                columns: table => new
                {
                    addressId = table.Column<int>(type: "INTEGER", nullable: false),
                    lat = table.Column<double>(type: "REAL", nullable: false),
                    lon = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressesCoordinates", x => x.addressId);
                    table.ForeignKey(
                        name: "FK_AddressesCoordinates_addresses_addressId",
                        column: x => x.addressId,
                        principalTable: "addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "addressPriorities",
                columns: table => new
                {
                    addressId = table.Column<int>(type: "INTEGER", nullable: false),
                    priority = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_addressPriorities", x => x.addressId);
                    table.ForeignKey(
                        name: "FK_addressPriorities_addresses_addressId",
                        column: x => x.addressId,
                        principalTable: "addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "applications",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    addressId = table.Column<int>(type: "INTEGER", nullable: true),
                    year = table.Column<int>(type: "INTEGER", nullable: false),
                    month = table.Column<int>(type: "INTEGER", nullable: false),
                    day = table.Column<int>(type: "INTEGER", nullable: false),
                    maxDaysForConnection = table.Column<int>(type: "INTEGER", nullable: false),
                    stretchingStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    timeRangeIsSet = table.Column<bool>(type: "INTEGER", nullable: false),
                    secondPart = table.Column<bool>(type: "INTEGER", nullable: false),
                    firstPart = table.Column<bool>(type: "INTEGER", nullable: false),
                    startHour = table.Column<int>(type: "INTEGER", nullable: false),
                    endHour = table.Column<int>(type: "INTEGER", nullable: false),
                    tarChangeApp = table.Column<bool>(type: "INTEGER", nullable: false),
                    statusWasChecked = table.Column<bool>(type: "INTEGER", nullable: false),
                    operatorComment = table.Column<string>(type: "TEXT", nullable: false),
                    masterComment = table.Column<string>(type: "TEXT", nullable: false),
                    inSchedule = table.Column<bool>(type: "INTEGER", nullable: false),
                    hot = table.Column<bool>(type: "INTEGER", nullable: false),
                    freeCable = table.Column<bool>(type: "INTEGER", nullable: false),
                    urgent = table.Column<bool>(type: "INTEGER", nullable: false),
                    ignored = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_applications", x => x.id);
                    table.ForeignKey(
                        name: "FK_applications_addresses_addressId",
                        column: x => x.addressId,
                        principalTable: "addresses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_addresses_districtName",
                table: "addresses",
                column: "districtName");

            migrationBuilder.CreateIndex(
                name: "IX_applications_addressId",
                table: "applications",
                column: "addressId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "addressAliases");

            migrationBuilder.DropTable(
                name: "AddressesCoordinates");

            migrationBuilder.DropTable(
                name: "addressPriorities");

            migrationBuilder.DropTable(
                name: "applications");

            migrationBuilder.DropTable(
                name: "brigades");

            migrationBuilder.DropTable(
                name: "coefficients");

            migrationBuilder.DropTable(
                name: "addresses");

            migrationBuilder.DropTable(
                name: "districts");
        }
    }
}
