USE fake
GO

CREATE TABLE [dbo].[photo](
	[photoId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	CONSTRAINT [PK_photo_detail] PRIMARY KEY CLUSTERED 
(
	[photoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[sold_photos](
	[photo_id] [int] NOT NULL,
	[sold_by] [tinyint] NOT NULL,
	CONSTRAINT [PK_sold_photos] PRIMARY KEY NONCLUSTERED 
(
	[photo_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER PROCEDURE [dbo].[sp_sold_photos_create]
(
	@photo_id BIGINT,
	@sold_by INT
)
AS
BEGIN

	DECLARE @table_var TABLE (photoId BIGINT, user_id INT)

	;WITH cte
	AS 
	(
		SELECT CAST(au.photoId AS BIGINT) AS photoId, au.UserId, wa.sold_by
		FROM sold_photos wa
		INNER JOIN photo au ON wa.photo_id = au.photoId
		WHERE au.photoId = @photo_id -- implicit conversion inside CTE
	)
	UPDATE wa
	SET sold_by = @sold_by -- implicit conversion in set clause
	OUTPUT inserted.photo_id, cte.UserId INTO @table_var -- implicit conversion in output into clause
	FROM dbo.sold_photos wa JOIN cte ON cte.photoId = wa.photo_id -- implicit conversion in from clause
	WHERE photo_id = @photo_id -- implicit conversion in where clause
	
	-- negative scenario, casting values so they don't cause implicit conversion
	-- TODO : add a simple update where target is not aliased
	;WITH cte_two
	AS 
	(
		SELECT au.photoId, au.UserId, wa.sold_by
		FROM sold_photos wa
		INNER JOIN photo au ON wa.photo_id = au.photoId
		WHERE au.photoId = CAST(@photo_id AS INT)
	)
	UPDATE wa
	SET sold_by = CAST(@sold_by AS TINYINT)
	OUTPUT CAST(inserted.photo_id AS BIGINT), cte_two.UserId INTO @table_var
	FROM dbo.sold_photos wa JOIN cte_two ON cte_two.photoId = wa.photo_id
	WHERE photo_id = CAST(@photo_id AS INT)

END
GO
