USE fake
GO

CREATE TABLE [dbo].[photo](
	[photoId] [int] NOT NULL,
	[photo_group_id] [bigint] NULL,
 CONSTRAINT [PK_photo_detail] PRIMARY KEY CLUSTERED 
(
	[photoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[photo_descriptor](
	[photo_descriptor_id] [bigint] NOT NULL,
	[photo_group_id] [bigint] NULL,
	[photo_id] [bigint] NULL,
 CONSTRAINT [PK_photo_descriptor] PRIMARY KEY NONCLUSTERED 
(
	[photo_descriptor_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE OR ALTER FUNCTION [dbo].[fn_generate_photo_descriptor_keys] (@photo_id BIGINT)
RETURNS VARCHAR(MAX)
AS
BEGIN
    
    DECLARE @key_values VARCHAR(MAX);
	
	WITH CTE (descriptor_name, descriptor_value)
	AS 
	(
		SELECT DISTINCT "Hey", 1
		FROM photo_descriptor ld WITH (NOLOCK)
		INNER JOIN photo p WITH (NOLOCK) ON ld.photo_id = p.photoId
		WHERE (p.photoId = @photo_id) AND ISNULL(p.quantity, 1) > 0

		UNION

		SELECT DISTINCT "Hi", 2
		FROM photo_descriptor ld WITH (NOLOCK)
		INNER JOIN photo p WITH (NOLOCK) ON ld.photo_id IS NULL AND ld.photo_group_id = p.photo_group_id
		WHERE (p.photoId = @photo_id) AND ISNULL(p.quantity, 1) > 0
	)
	SELECT 1 FROM CTE

	RETURN @key_values;

END
GO
