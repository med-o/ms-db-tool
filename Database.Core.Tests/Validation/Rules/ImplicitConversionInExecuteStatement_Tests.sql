USE fake
GO

CREATE OR ALTER PROCEDURE [dbo].[spTestOne]
(
	@some_id bigint,
	@yet_another_id int,
	@title varchar(128) = NULL,
	@sub_title varchar(50) = NULL,
	@is_new bit = NULL,
	@sale_price decimal(20,6) = NULL,
	@group_id int = NULL,
	@xml xml
)
AS BEGIN
	SET NOCOUNT ON;	
	SELECT 1
END
GO

CREATE OR ALTER PROCEDURE dbo.spTestExecute (
	@the_other_id_filed int,
	@yet_another_id int,
	@new_title varchar(128),
	@group_id bigint = null
) AS BEGIN

	SET NOCOUNT ON;
	
	declare @new_xml xml
	exec dbo.spTestOne
		@the_other_id_filed, -- mandatory not aliased
		@yet_another_id, -- mandatory not aliased
		@new_title, -- optional not aliased
		@xml = @new_xml output, -- optional, aliased and not in order from this point on
		@sale_price = DEFAULT, -- using default
		@group_id = NULL -- pass in null

END
GO
