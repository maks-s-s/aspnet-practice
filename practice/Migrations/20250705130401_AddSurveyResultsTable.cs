using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace practice.Migrations
{
    /// <inheritdoc />
    public partial class AddSurveyResultsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SurveyResult_Answers_AnswerId",
                table: "SurveyResult");

            migrationBuilder.DropForeignKey(
                name: "FK_SurveyResult_AspNetUsers_UserId",
                table: "SurveyResult");

            migrationBuilder.DropForeignKey(
                name: "FK_SurveyResult_Questions_QuestionId",
                table: "SurveyResult");

            migrationBuilder.DropForeignKey(
                name: "FK_SurveyResult_Surveys_SurveyId",
                table: "SurveyResult");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SurveyResult",
                table: "SurveyResult");

            migrationBuilder.RenameTable(
                name: "SurveyResult",
                newName: "SurveyResults");

            migrationBuilder.RenameIndex(
                name: "IX_SurveyResult_UserId",
                table: "SurveyResults",
                newName: "IX_SurveyResults_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_SurveyResult_SurveyId",
                table: "SurveyResults",
                newName: "IX_SurveyResults_SurveyId");

            migrationBuilder.RenameIndex(
                name: "IX_SurveyResult_QuestionId",
                table: "SurveyResults",
                newName: "IX_SurveyResults_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_SurveyResult_AnswerId",
                table: "SurveyResults",
                newName: "IX_SurveyResults_AnswerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SurveyResults",
                table: "SurveyResults",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyResults_Answers_AnswerId",
                table: "SurveyResults",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyResults_AspNetUsers_UserId",
                table: "SurveyResults",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyResults_Questions_QuestionId",
                table: "SurveyResults",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyResults_Surveys_SurveyId",
                table: "SurveyResults",
                column: "SurveyId",
                principalTable: "Surveys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SurveyResults_Answers_AnswerId",
                table: "SurveyResults");

            migrationBuilder.DropForeignKey(
                name: "FK_SurveyResults_AspNetUsers_UserId",
                table: "SurveyResults");

            migrationBuilder.DropForeignKey(
                name: "FK_SurveyResults_Questions_QuestionId",
                table: "SurveyResults");

            migrationBuilder.DropForeignKey(
                name: "FK_SurveyResults_Surveys_SurveyId",
                table: "SurveyResults");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SurveyResults",
                table: "SurveyResults");

            migrationBuilder.RenameTable(
                name: "SurveyResults",
                newName: "SurveyResult");

            migrationBuilder.RenameIndex(
                name: "IX_SurveyResults_UserId",
                table: "SurveyResult",
                newName: "IX_SurveyResult_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_SurveyResults_SurveyId",
                table: "SurveyResult",
                newName: "IX_SurveyResult_SurveyId");

            migrationBuilder.RenameIndex(
                name: "IX_SurveyResults_QuestionId",
                table: "SurveyResult",
                newName: "IX_SurveyResult_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_SurveyResults_AnswerId",
                table: "SurveyResult",
                newName: "IX_SurveyResult_AnswerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SurveyResult",
                table: "SurveyResult",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyResult_Answers_AnswerId",
                table: "SurveyResult",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyResult_AspNetUsers_UserId",
                table: "SurveyResult",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyResult_Questions_QuestionId",
                table: "SurveyResult",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyResult_Surveys_SurveyId",
                table: "SurveyResult",
                column: "SurveyId",
                principalTable: "Surveys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
