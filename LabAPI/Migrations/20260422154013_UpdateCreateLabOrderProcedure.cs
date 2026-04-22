using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCreateLabOrderProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS create_lab_order;");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE create_lab_order
                    @patient_id INT,
                    @office_id INT,
                    @analysis_ids int_list READONLY
                AS 
                BEGIN 
                    SET NOCOUNT ON;

                    IF NOT EXISTS (SELECT 1 FROM @analysis_ids)
                    BEGIN
                        RAISERROR('Cannot create lab order without analyses.', 16, 1);
                        RETURN;
                    END

                    BEGIN TRY
                        BEGIN TRANSACTION;

                        DECLARE @order_number INT;
                        DECLARE @total_price DECIMAL(10,2);

                        SELECT @total_price = SUM(price)
                        FROM analysis
                        WHERE analysis.id IN (SELECT id FROM @analysis_ids);

                        INSERT INTO lab_order(patient_id, office_id, total_price)
                        VALUES (@patient_id, @office_id, @total_price);

                        SET @order_number = SCOPE_IDENTITY();

                        INSERT INTO order_analysis(order_number, analysis_id)
                        SELECT @order_number, id 
                        FROM @analysis_ids;

                        -- Використання курсору, щоб не дублювати sample_type в sample
                        DECLARE @current_type NVARCHAR(50);
                        DECLARE @new_sample_id INT;

                        DECLARE @inserted_results TABLE 
                        (
                            result_id INT, 
                            analysis_id INT
                        );

                        DECLARE type_cursor CURSOR FOR
                            SELECT DISTINCT sample_type
                            FROM analysis
                            WHERE id IN (SELECT id FROM @analysis_ids)

                        OPEN type_cursor;
                        FETCH NEXT FROM type_cursor INTO @current_type;
        
                        WHILE @@FETCH_STATUS = 0
                        BEGIN
                            INSERT INTO sample(order_number)
                            VALUES (@order_number);

                            SET @new_sample_id = SCOPE_IDENTITY();

                            DELETE FROM @inserted_results;

                            INSERT INTO result (sample_id, analysis_id)
                            OUTPUT inserted.id, inserted.analysis_id INTO @inserted_results(result_id, analysis_id)
                            SELECT @new_sample_id, id
                            FROM analysis 
                            WHERE sample_type = @current_type 
                                AND id IN (SELECT id FROM @analysis_ids);

                            INSERT INTO parameter_result (result_id, parameter_id)
                            SELECT ir.result_id, p.id
                            FROM @inserted_results ir
                            JOIN parameter p ON ir.analysis_id = p.analysis_id;

                            FETCH NEXT FROM type_cursor INTO @current_type;
                        END
        
                        CLOSE type_cursor;
                        DEALLOCATE type_cursor;

                        COMMIT TRANSACTION;
                    END TRY
                    BEGIN CATCH
                        IF @@TRANCOUNT > 0
                            ROLLBACK TRANSACTION;
                        THROW; 
                    END CATCH
                END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS create_lab_order;");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE create_lab_order
                    @patient_id INT,
                    @office_id INT,
                    @analysis_ids int_list READONLY
                AS 
                BEGIN 
                    SET NOCOUNT ON;

                    IF NOT EXISTS (SELECT 1 FROM @analysis_ids)
                    BEGIN
                        RAISERROR('Cannot create lab order without analyses.', 16, 1);
                        RETURN;
                    END

                    BEGIN TRY
                        BEGIN TRANSACTION;

                        DECLARE @order_number INT;
                        DECLARE @total_price DECIMAL(10,2);

                        SELECT @total_price = SUM(price)
                        FROM analysis
                        WHERE analysis.id IN (SELECT id FROM @analysis_ids);

                        INSERT INTO lab_order(patient_id, office_id, total_price)
                        VALUES (@patient_id, @office_id, @total_price);

                        SET @order_number = SCOPE_IDENTITY();

                        INSERT INTO order_analysis(order_number, analysis_id)
                        SELECT @order_number, id 
                        FROM @analysis_ids;

                        -- Використання курсору, щоб не дублювати sample_type в sample
                        DECLARE @current_type NVARCHAR(50);
                        DECLARE @new_sample_id INT;

                        DECLARE type_cursor CURSOR FOR
                            SELECT DISTINCT sample_type
                            FROM analysis
                            WHERE id IN (SELECT id FROM @analysis_ids)

                        OPEN type_cursor;
                        FETCH NEXT FROM type_cursor INTO @current_type;
        
                        WHILE @@FETCH_STATUS = 0
                        BEGIN
                            INSERT INTO sample(order_number)
                            VALUES (@order_number);

                            SET @new_sample_id = SCOPE_IDENTITY();

                            INSERT INTO result (sample_id, analysis_id)
                            SELECT @new_sample_id, id
                            FROM analysis 
                            WHERE sample_type = @current_type 
                                AND id IN (SELECT id FROM @analysis_ids);

                            FETCH NEXT FROM type_cursor INTO @current_type;
                        END
        
                        CLOSE type_cursor;
                        DEALLOCATE type_cursor;

                        COMMIT TRANSACTION;
                    END TRY
                    BEGIN CATCH
                        IF @@TRANCOUNT > 0
                            ROLLBACK TRANSACTION;
                        THROW; 
                    END CATCH
                END;");
        }
    }
}
