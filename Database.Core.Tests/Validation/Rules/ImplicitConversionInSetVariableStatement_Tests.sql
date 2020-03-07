USE fake
GO

CREATE TABLE [dbo].[photo](
	[photoId] [int] NOT NULL,
	[userId] [int] NOT NULL,
	CONSTRAINT [PK_photo_detail] PRIMARY KEY CLUSTERED 
(
	[photoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


DECLARE @photoId BIGINT
SET @photoId = (SELECT TOP 1 A.photoId FROM dbo.photo A ORDER BY A.photoId DESC)
SELECT @photoId AS PhotoId

DECLARE @anotherPhotoId INT
SET @anotherPhotoId = @photoId
SELECT @anotherPhotoId AS DifferentPhotoId

GO

