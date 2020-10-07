using Microsoft.EntityFrameworkCore.Migrations;

namespace PM.MVC.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectResources",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false),
                    ResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectResources", x => new { x.ProjectId, x.ResourceId });
                    table.ForeignKey(
                        name: "FK_ProjectResources_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectResources_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Qualifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SkillId = table.Column<int>(nullable: false),
                    Level = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Qualifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Qualifications_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectQualifications",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false),
                    QualificationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectQualifications", x => new { x.ProjectId, x.QualificationId });
                    table.ForeignKey(
                        name: "FK_ProjectQualifications_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectQualifications_Qualifications_QualificationId",
                        column: x => x.QualificationId,
                        principalTable: "Qualifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QualificationResources",
                columns: table => new
                {
                    QualificationId = table.Column<int>(nullable: false),
                    ResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualificationResources", x => new { x.QualificationId, x.ResourceId });
                    table.ForeignKey(
                        name: "FK_QualificationResources_Qualifications_QualificationId",
                        column: x => x.QualificationId,
                        principalTable: "Qualifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QualificationResources_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Space Project" });

            migrationBuilder.InsertData(
                table: "Resources",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Steve" },
                    { 4, "Katrin" },
                    { 2, "Bob" },
                    { 3, "Jhon" }
                });

            migrationBuilder.InsertData(
                table: "Skills",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "C#" },
                    { 2, "Excel" },
                    { 3, "Driving licence" },
                    { 4, "Project management" }
                });

            migrationBuilder.InsertData(
                table: "ProjectResources",
                columns: new[] { "ProjectId", "ResourceId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 4 }
                });

            migrationBuilder.InsertData(
                table: "Qualifications",
                columns: new[] { "Id", "Level", "SkillId" },
                values: new object[,]
                {
                    { 1, 0, 1 },
                    { 5, 1, 1 },
                    { 9, 2, 1 },
                    { 2, 0, 2 },
                    { 6, 1, 2 },
                    { 10, 2, 2 },
                    { 3, 0, 3 },
                    { 7, 1, 3 },
                    { 11, 2, 3 },
                    { 4, 0, 4 },
                    { 8, 1, 4 },
                    { 12, 2, 4 }
                });

            migrationBuilder.InsertData(
                table: "ProjectQualifications",
                columns: new[] { "ProjectId", "QualificationId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 6 },
                    { 1, 7 },
                    { 1, 4 }
                });

            migrationBuilder.InsertData(
                table: "QualificationResources",
                columns: new[] { "QualificationId", "ResourceId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 6, 2 },
                    { 6, 3 },
                    { 7, 1 },
                    { 7, 4 },
                    { 4, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectQualifications_QualificationId",
                table: "ProjectQualifications",
                column: "QualificationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectResources_ResourceId",
                table: "ProjectResources",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_QualificationResources_ResourceId",
                table: "QualificationResources",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Qualifications_SkillId",
                table: "Qualifications",
                column: "SkillId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectQualifications");

            migrationBuilder.DropTable(
                name: "ProjectResources");

            migrationBuilder.DropTable(
                name: "QualificationResources");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Qualifications");

            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropTable(
                name: "Skills");
        }
    }
}
