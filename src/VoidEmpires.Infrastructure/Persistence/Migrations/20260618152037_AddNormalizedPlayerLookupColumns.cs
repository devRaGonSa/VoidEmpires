using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoidEmpires.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNormalizedPlayerLookupColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_civilizations_profile_name",
                table: "civilizations");

            migrationBuilder.AddColumn<string>(
                name: "normalized_display_name",
                table: "player_profiles",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "normalized_name",
                table: "civilizations",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            ApplyNormalizedNameBackfill(migrationBuilder);

            migrationBuilder.AlterColumn<string>(
                name: "normalized_display_name",
                table: "player_profiles",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "normalized_name",
                table: "civilizations",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ux_player_profiles_normalized_display_name",
                table: "player_profiles",
                column: "normalized_display_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_civilizations_normalized_name",
                table: "civilizations",
                column: "normalized_name");

            migrationBuilder.CreateIndex(
                name: "ux_civilizations_profile_normalized_name",
                table: "civilizations",
                columns: new[] { "player_profile_id", "normalized_name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_player_profiles_normalized_display_name",
                table: "player_profiles");

            migrationBuilder.DropIndex(
                name: "ix_civilizations_normalized_name",
                table: "civilizations");

            migrationBuilder.DropIndex(
                name: "ux_civilizations_profile_normalized_name",
                table: "civilizations");

            migrationBuilder.DropColumn(
                name: "normalized_display_name",
                table: "player_profiles");

            migrationBuilder.DropColumn(
                name: "normalized_name",
                table: "civilizations");

            migrationBuilder.CreateIndex(
                name: "ux_civilizations_profile_name",
                table: "civilizations",
                columns: new[] { "player_profile_id", "name" },
                unique: true);
        }

        private void ApplyNormalizedNameBackfill(MigrationBuilder migrationBuilder)
        {
            if (ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                migrationBuilder.Sql(
                    """
                    UPDATE player_profiles
                    SET normalized_display_name = UPPER(BTRIM(display_name))
                    WHERE normalized_display_name IS NULL;

                    UPDATE civilizations
                    SET normalized_name = UPPER(BTRIM(name))
                    WHERE normalized_name IS NULL;

                    DO $$
                    BEGIN
                        IF EXISTS (
                            SELECT 1
                            FROM player_profiles
                            GROUP BY normalized_display_name
                            HAVING COUNT(*) > 1
                        ) THEN
                            RAISE EXCEPTION 'Duplicate normalized display names must be resolved before applying AddNormalizedPlayerLookupColumns.';
                        END IF;

                        IF EXISTS (
                            SELECT 1
                            FROM civilizations
                            GROUP BY player_profile_id, normalized_name
                            HAVING COUNT(*) > 1
                        ) THEN
                            RAISE EXCEPTION 'Duplicate normalized civilization names per profile must be resolved before applying AddNormalizedPlayerLookupColumns.';
                        END IF;
                    END $$;
                    """);

                return;
            }

            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.Sql(
                    """
                    UPDATE player_profiles
                    SET normalized_display_name = UPPER(LTRIM(RTRIM(display_name)))
                    WHERE normalized_display_name IS NULL;

                    UPDATE civilizations
                    SET normalized_name = UPPER(LTRIM(RTRIM(name)))
                    WHERE normalized_name IS NULL;

                    IF EXISTS (
                        SELECT 1
                        FROM player_profiles
                        GROUP BY normalized_display_name
                        HAVING COUNT(*) > 1
                    )
                    BEGIN
                        THROW 50000, 'Duplicate normalized display names must be resolved before applying AddNormalizedPlayerLookupColumns.', 1;
                    END;

                    IF EXISTS (
                        SELECT 1
                        FROM civilizations
                        GROUP BY player_profile_id, normalized_name
                        HAVING COUNT(*) > 1
                    )
                    BEGIN
                        THROW 50000, 'Duplicate normalized civilization names per profile must be resolved before applying AddNormalizedPlayerLookupColumns.', 1;
                    END;
                    """);

                return;
            }

            throw new NotSupportedException($"Provider '{ActiveProvider}' is not supported by {nameof(AddNormalizedPlayerLookupColumns)}.");
        }
    }
}
