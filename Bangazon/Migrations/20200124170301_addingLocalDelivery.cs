using Microsoft.EntityFrameworkCore.Migrations;

namespace Bangazon.Migrations
{
    public partial class addingLocalDelivery : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "LocalDeliveryAvailable",
                table: "Product",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-ffff-ffff-ffff-ffffffffffff",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "6bc7a8e0-054a-4693-9b8f-e32d1d42b66f", "AQAAAAEAACcQAAAAEKye6Jd4RpH1Ecxti4Elj1Bgcxdsw5H3tEFVLlZ55X1KIUiVko9GTMox6oGNrMF9zQ==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocalDeliveryAvailable",
                table: "Product");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-ffff-ffff-ffff-ffffffffffff",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "9262caf3-7b5a-45ad-bf65-ddf3ff294252", "AQAAAAEAACcQAAAAEDZCm5qOTOhtNYt7Q/OiCStxlXxHaqnsnJVkEoCxWcxkkBRcZpuhbW/+tS8qSG74vg==" });
        }
    }
}
