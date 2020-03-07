USE fake
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
	@sold_by TINYINT
)
AS
BEGIN

	SET NOCOUNT OFF

	UPDATE dbo.sold_photos WITH (ROWLOCK)
	SET sold_by = @sold_by
	WHERE photo_id = @photo_id

	IF @@rowcount=0 BEGIN
		INSERT INTO dbo.sold_photos WITH (ROWLOCK) (photo_id,sold_by)
		VALUES (@photo_id,@sold_by)
	END
	
END
GO
