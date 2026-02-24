using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechSto.Migrations
{
    /// <inheritdoc />
    public partial class DataBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarСategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarСategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarBrands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BrandName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarBrands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Owners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    STSNumber = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ModelName = table.Column<string>(type: "TEXT", nullable: true),
                    CarBrandId = table.Column<int>(type: "INTEGER", nullable: true),
                    CarCategoryId = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxMass = table.Column<int>(type: "INTEGER", nullable: true),
                    CurbMass = table.Column<int>(type: "INTEGER", nullable: true),
                    BrakeForceDifference = table.Column<int>(type: "INTEGER", nullable: true),
                    ParkingBrake = table.Column<int>(type: "INTEGER", nullable: true),
                    ReserveBrake = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarModels_CarBrands_CarBrandId",
                        column: x => x.CarBrandId,
                        principalTable: "CarBrands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarModels_CarСategories_CarCategoryId",
                        column: x => x.CarCategoryId,
                        principalTable: "CarСategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Axles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CarModelId = table.Column<int>(type: "INTEGER", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    RotationDirection = table.Column<int>(type: "INTEGER", nullable: false),
                    HasParkingBrake = table.Column<bool>(type: "INTEGER", nullable: false),
                    BrakeType = table.Column<int>(type: "INTEGER", nullable: false),
                    HasRegulator = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Axles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Axles_CarModels_CarModelId",
                        column: x => x.CarModelId,
                        principalTable: "CarModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TheCars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GosNumber = table.Column<string>(type: "TEXT", nullable: false),
                    BodyNumber = table.Column<string>(type: "TEXT", nullable: false),
                    FrameNumber = table.Column<string>(type: "TEXT", nullable: false),
                    CarModelId = table.Column<int>(type: "INTEGER", nullable: false),
                    OwnerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TheCars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TheCars_CarModels_CarModelId",
                        column: x => x.CarModelId,
                        principalTable: "CarModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TheCars_Owners_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Owners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Checks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data = table.Column<string>(type: "TEXT", nullable: false),
                    CarMileage = table.Column<int>(type: "INTEGER", nullable: false),
                    CheckResult = table.Column<int>(type: "INTEGER", nullable: true),
                    TheCarId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Checks_TheCars_TheCarId",
                        column: x => x.TheCarId,
                        principalTable: "TheCars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Axles_CarModelId_Order",
                table: "Axles",
                columns: new[] { "CarModelId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarModels_CarBrandId",
                table: "CarModels",
                column: "CarBrandId");

            migrationBuilder.CreateIndex(
                name: "IX_CarModels_CarCategoryId",
                table: "CarModels",
                column: "CarCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Checks_TheCarId",
                table: "Checks",
                column: "TheCarId");

            migrationBuilder.CreateIndex(
                name: "IX_TheCars_CarModelId",
                table: "TheCars",
                column: "CarModelId");

            migrationBuilder.CreateIndex(
                name: "IX_TheCars_OwnerId",
                table: "TheCars",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Axles");

            migrationBuilder.DropTable(
                name: "Checks");

            migrationBuilder.DropTable(
                name: "TheCars");

            migrationBuilder.DropTable(
                name: "CarModels");

            migrationBuilder.DropTable(
                name: "Owners");

            migrationBuilder.DropTable(
                name: "CarBrands");

            migrationBuilder.DropTable(
                name: "CarСategories");
        }
    }
}
