using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SimplyRugby.Migrations
{
    /// <inheritdoc />
    public partial class Inital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    DoctorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TelephoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctors", x => x.DoctorID);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    PositionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.PositionID);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    SkillsID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kicking_Drop = table.Column<int>(type: "int", nullable: false),
                    Kicking_Punt = table.Column<int>(type: "int", nullable: false),
                    Kicking_Grubber = table.Column<int>(type: "int", nullable: false),
                    Kicking_Goal = table.Column<int>(type: "int", nullable: false),
                    Passing_Standard = table.Column<int>(type: "int", nullable: false),
                    Passing_Spin = table.Column<int>(type: "int", nullable: false),
                    Passing_Pop = table.Column<int>(type: "int", nullable: false),
                    Tackling_Front = table.Column<int>(type: "int", nullable: false),
                    Tackling_Rear = table.Column<int>(type: "int", nullable: false),
                    Tackling_Side = table.Column<int>(type: "int", nullable: false),
                    Tackling_Scramble = table.Column<int>(type: "int", nullable: false),
                    MemberID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.SkillsID);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    TeamID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.TeamID);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    MatchID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamID = table.Column<int>(type: "int", nullable: false),
                    Opponent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatePlayed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TeamScore = table.Column<int>(type: "int", nullable: false),
                    OpponentScore = table.Column<int>(type: "int", nullable: false),
                    FirstHalf_TeamScore = table.Column<int>(type: "int", nullable: false),
                    FirstHalf_OpponentScore = table.Column<int>(type: "int", nullable: false),
                    FirstHalf_Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondHalf_TeamScore = table.Column<int>(type: "int", nullable: false),
                    SecondHalf_OpponentScore = table.Column<int>(type: "int", nullable: false),
                    SecondHalf_Comments = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.MatchID);
                    table.ForeignKey(
                        name: "FK_Matches_Teams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Teams",
                        principalColumn: "TeamID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountType = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    AccessLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberID = table.Column<int>(type: "int", nullable: true),
                    TeamID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Username);
                    table.ForeignKey(
                        name: "FK_Accounts_Teams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Teams",
                        principalColumn: "TeamID");
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    MemberID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SkillsID = table.Column<int>(type: "int", nullable: true),
                    TeamID = table.Column<int>(type: "int", nullable: true),
                    SRUNumber = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Postcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TelephoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberType = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    NextOfKinID = table.Column<int>(type: "int", nullable: true),
                    DoctorID = table.Column<int>(type: "int", nullable: true),
                    HealthConditions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HasConsentForm = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.MemberID);
                    table.ForeignKey(
                        name: "FK_Members_Doctors_DoctorID",
                        column: x => x.DoctorID,
                        principalTable: "Doctors",
                        principalColumn: "DoctorID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Members_Skills_SkillsID",
                        column: x => x.SkillsID,
                        principalTable: "Skills",
                        principalColumn: "SkillsID");
                    table.ForeignKey(
                        name: "FK_Members_Teams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Teams",
                        principalColumn: "TeamID");
                });

            migrationBuilder.CreateTable(
                name: "NextOfKins",
                columns: table => new
                {
                    NextOfKinID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberID = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TelephoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KinType = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Relationship = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JuniorPlayerMemberID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NextOfKins", x => x.NextOfKinID);
                    table.ForeignKey(
                        name: "FK_NextOfKins_Members_JuniorPlayerMemberID",
                        column: x => x.JuniorPlayerMemberID,
                        principalTable: "Members",
                        principalColumn: "MemberID");
                });

            migrationBuilder.CreateTable(
                name: "PlayerPosition",
                columns: table => new
                {
                    PlayersMemberID = table.Column<int>(type: "int", nullable: false),
                    PositionsPositionID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerPosition", x => new { x.PlayersMemberID, x.PositionsPositionID });
                    table.ForeignKey(
                        name: "FK_PlayerPosition_Members_PlayersMemberID",
                        column: x => x.PlayersMemberID,
                        principalTable: "Members",
                        principalColumn: "MemberID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerPosition_Positions_PositionsPositionID",
                        column: x => x.PositionsPositionID,
                        principalTable: "Positions",
                        principalColumn: "PositionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingSessions",
                columns: table => new
                {
                    TrainingSessionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TeamID = table.Column<int>(type: "int", nullable: false),
                    CoachMemberID = table.Column<int>(type: "int", nullable: true),
                    SkillsAndActivities = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InjuriesAndAccidents = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingSessions", x => x.TrainingSessionID);
                    table.ForeignKey(
                        name: "FK_TrainingSessions_Members_CoachMemberID",
                        column: x => x.CoachMemberID,
                        principalTable: "Members",
                        principalColumn: "MemberID");
                    table.ForeignKey(
                        name: "FK_TrainingSessions_Teams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Teams",
                        principalColumn: "TeamID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerTrainingSession",
                columns: table => new
                {
                    PlayersMemberID = table.Column<int>(type: "int", nullable: false),
                    TrainingSessionsTrainingSessionID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerTrainingSession", x => new { x.PlayersMemberID, x.TrainingSessionsTrainingSessionID });
                    table.ForeignKey(
                        name: "FK_PlayerTrainingSession_Members_PlayersMemberID",
                        column: x => x.PlayersMemberID,
                        principalTable: "Members",
                        principalColumn: "MemberID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerTrainingSession_TrainingSessions_TrainingSessionsTrainingSessionID",
                        column: x => x.TrainingSessionsTrainingSessionID,
                        principalTable: "TrainingSessions",
                        principalColumn: "TrainingSessionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Positions",
                columns: new[] { "PositionID", "Name" },
                values: new object[,]
                {
                    { 1, "Full Back" },
                    { 2, "Wing" },
                    { 3, "Centre" },
                    { 4, "Fly Half" },
                    { 5, "Scrum Half" },
                    { 6, "Hooker" },
                    { 7, "Prop" },
                    { 8, "2nd Row" },
                    { 9, "Back Row" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_MemberID",
                table: "Accounts",
                column: "MemberID",
                unique: true,
                filter: "[MemberID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_TeamID",
                table: "Accounts",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_TeamID",
                table: "Matches",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_Members_DoctorID",
                table: "Members",
                column: "DoctorID");

            migrationBuilder.CreateIndex(
                name: "IX_Members_NextOfKinID",
                table: "Members",
                column: "NextOfKinID");

            migrationBuilder.CreateIndex(
                name: "IX_Members_SkillsID",
                table: "Members",
                column: "SkillsID",
                unique: true,
                filter: "[SkillsID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Members_TeamID",
                table: "Members",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_NextOfKins_JuniorPlayerMemberID",
                table: "NextOfKins",
                column: "JuniorPlayerMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPosition_PositionsPositionID",
                table: "PlayerPosition",
                column: "PositionsPositionID");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTrainingSession_TrainingSessionsTrainingSessionID",
                table: "PlayerTrainingSession",
                column: "TrainingSessionsTrainingSessionID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_CoachMemberID",
                table: "TrainingSessions",
                column: "CoachMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_TeamID",
                table: "TrainingSessions",
                column: "TeamID");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Members_MemberID",
                table: "Accounts",
                column: "MemberID",
                principalTable: "Members",
                principalColumn: "MemberID");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_NextOfKins_NextOfKinID",
                table: "Members",
                column: "NextOfKinID",
                principalTable: "NextOfKins",
                principalColumn: "NextOfKinID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NextOfKins_Members_JuniorPlayerMemberID",
                table: "NextOfKins");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "PlayerPosition");

            migrationBuilder.DropTable(
                name: "PlayerTrainingSession");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "TrainingSessions");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "Doctors");

            migrationBuilder.DropTable(
                name: "NextOfKins");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "Teams");
        }
    }
}
