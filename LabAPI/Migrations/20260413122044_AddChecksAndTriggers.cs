using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddChecksAndTriggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "sample_collection_expiry_date_check",
                table: "sample",
                sql: "expiry_date IS NULL OR (collection_date IS NOT NULL AND collection_date < expiry_date)");

            migrationBuilder.AddCheckConstraint(
                name: "sample_status_check",
                table: "sample",
                sql: "status IN ('waiting', 'collected', 'analyzed', 'expired')");

            migrationBuilder.AddCheckConstraint(
                name: "result_status_check",
                table: "result",
                sql: "status IN ('pending', 'normal', 'abnormal')");

            migrationBuilder.AddCheckConstraint(
                name: "patient_gender_check",
                table: "patient",
                sql: "gender IN ('M','F')");

            migrationBuilder.AddCheckConstraint(
                name: "parameter_norm_age_max_check",
                table: "parameter_norm",
                sql: "age_max >= age_min");

            migrationBuilder.AddCheckConstraint(
                name: "parameter_norm_age_min_check",
                table: "parameter_norm",
                sql: "age_min >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "parameter_norm_gender_check",
                table: "parameter_norm",
                sql: "gender IN ('M','F','A')");

            migrationBuilder.AddCheckConstraint(
                name: "parameter_norm_max_val_check",
                table: "parameter_norm",
                sql: "max_value IS NULL OR min_value IS NULL OR max_value >= min_value");

            migrationBuilder.AddCheckConstraint(
                name: "parameter_norm_val_null_check",
                table: "parameter_norm",
                sql: "max_value IS NOT NULL OR min_value IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "office_schedule_day_check",
                table: "office_schedule",
                sql: "day_of_week BETWEEN 0 AND 6");

            migrationBuilder.AddCheckConstraint(
                name: "office_schedule_time_check",
                table: "office_schedule",
                sql: "close_time > open_time");

            migrationBuilder.AddCheckConstraint(
                name: "office_type_check",
                table: "office",
                sql: "type IN ('collection', 'analysis', 'mixed')");

            migrationBuilder.AddCheckConstraint(
                name: "lab_order_status_check",
                table: "lab_order",
                sql: "status IN ('unpaid', 'pending', 'in_progress', 'completed', 'cancelled')");

            migrationBuilder.AddCheckConstraint(
                name: "es_office_check",
                table: "employee_shift",
                sql: "(shift_type = 'work'  AND office_id IS NOT NULL) OR (shift_type <> 'work' AND office_id IS NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "es_time_check",
                table: "employee_shift",
                sql: "(shift_type = 'work' AND start_time IS NOT NULL AND end_time IS NOT NULL AND end_time > start_time) OR (shift_type <> 'work' AND start_time IS NULL AND end_time IS NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "es_type_check",
                table: "employee_shift",
                sql: "shift_type IN ('work', 'day_off', 'sick_leave', 'vacation')");

            migrationBuilder.AddCheckConstraint(
                name: "est_day_check",
                table: "employee_schedule",
                sql: "day_of_week BETWEEN 0 AND 6");

            migrationBuilder.AddCheckConstraint(
                name: "est_time_check",
                table: "employee_schedule",
                sql: "end_time > start_time");

            migrationBuilder.AddCheckConstraint(
                name: "appointment_purpose_check",
                table: "appointment",
                sql: "purpose IN ('first_visit', 'sample', 'results')");

            migrationBuilder.AddCheckConstraint(
                name: "appointment_status_check",
                table: "appointment",
                sql: "status IN ('pending', 'arrived', 'completed', 'cancelled')");

            migrationBuilder.AddCheckConstraint(
                name: "analysis_price_check",
                table: "analysis",
                sql: "price >= 0");

            migrationBuilder.Sql(@"
                CREATE TRIGGER sample_status_sync_order
                ON sample
                AFTER UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;
	                IF UPDATE(status)
                    BEGIN
                        UPDATE lab_order
                        SET status = CASE 
                            -- any expired
                            WHEN 'expired' = ANY 
                            (
                                SELECT status FROM sample
                                WHERE order_number = lab_order.number
                            ) THEN 'cancelled'

                            -- all anylazed
                            WHEN 'analyzed' = ALL
                            (
                                SELECT status FROM sample
                                WHERE order_number = lab_order.number
                            ) THEN 'completed'

                            -- any collected
                            WHEN 'collected' = ANY 
                            (
                                SELECT status FROM sample
                                WHERE order_number = lab_order.number
                            ) THEN 'in_progress'

                            ELSE status
                        END
                        WHERE status != 'unpaid' AND number IN (SELECT DISTINCT order_number FROM inserted);
                    END
                END;");

            migrationBuilder.Sql(@"
                CREATE TRIGGER sample_set_expiry_date
                ON sample
                AFTER UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;
                    IF UPDATE(status)
                    BEGIN
                        UPDATE sample
                        SET 
                        collection_date = ISNULL(collection_date, SYSDATETIME()),
                        expiry_date = DATEADD(day,
                        (
                            SELECT MIN(a.expiry_days)
                            FROM result AS r
                            JOIN analysis a ON r.analysis_id = a.id
                            WHERE r.sample_id = sample.id
                        ), CAST(ISNULL(collection_date, SYSDATETIME()) AS DATE))
                        WHERE status = 'collected' AND id IN (SELECT id FROM inserted);
                    END
                END;");

            migrationBuilder.Sql("CREATE TYPE int_list AS TABLE (id INT NOT NULL);");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE create_lab_order
                    @patient_id INT,
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

                        INSERT INTO lab_order(patient_id, total_price)
                        VALUES (@patient_id, @total_price);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "sample_collection_expiry_date_check",
                table: "sample");

            migrationBuilder.DropCheckConstraint(
                name: "sample_status_check",
                table: "sample");

            migrationBuilder.DropCheckConstraint(
                name: "result_status_check",
                table: "result");

            migrationBuilder.DropCheckConstraint(
                name: "patient_gender_check",
                table: "patient");

            migrationBuilder.DropCheckConstraint(
                name: "parameter_norm_age_max_check",
                table: "parameter_norm");

            migrationBuilder.DropCheckConstraint(
                name: "parameter_norm_age_min_check",
                table: "parameter_norm");

            migrationBuilder.DropCheckConstraint(
                name: "parameter_norm_gender_check",
                table: "parameter_norm");

            migrationBuilder.DropCheckConstraint(
                name: "parameter_norm_max_val_check",
                table: "parameter_norm");

            migrationBuilder.DropCheckConstraint(
                name: "parameter_norm_val_null_check",
                table: "parameter_norm");

            migrationBuilder.DropCheckConstraint(
                name: "office_schedule_day_check",
                table: "office_schedule");

            migrationBuilder.DropCheckConstraint(
                name: "office_schedule_time_check",
                table: "office_schedule");

            migrationBuilder.DropCheckConstraint(
                name: "office_type_check",
                table: "office");

            migrationBuilder.DropCheckConstraint(
                name: "lab_order_status_check",
                table: "lab_order");

            migrationBuilder.DropCheckConstraint(
                name: "es_office_check",
                table: "employee_shift");

            migrationBuilder.DropCheckConstraint(
                name: "es_time_check",
                table: "employee_shift");

            migrationBuilder.DropCheckConstraint(
                name: "es_type_check",
                table: "employee_shift");

            migrationBuilder.DropCheckConstraint(
                name: "est_day_check",
                table: "employee_schedule");

            migrationBuilder.DropCheckConstraint(
                name: "est_time_check",
                table: "employee_schedule");

            migrationBuilder.DropCheckConstraint(
                name: "appointment_purpose_check",
                table: "appointment");

            migrationBuilder.DropCheckConstraint(
                name: "appointment_status_check",
                table: "appointment");

            migrationBuilder.DropCheckConstraint(
                name: "analysis_price_check",
                table: "analysis");

            migrationBuilder.Sql("DROP TRIGGER IF EXISTS sample_set_expiry_date;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS sample_status_sync_order;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS create_lab_order;");
            migrationBuilder.Sql("DROP TYPE IF EXISTS int_list;");
        }
    }
}
