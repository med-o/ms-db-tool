USE [fake]
GO

-- positive scenario
CREATE TABLE [dbo].[photo](
	[photoId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[userId] [int] NOT NULL,
	[Title] [varchar](128) NULL,
 CONSTRAINT [PK_photo_detail] PRIMARY KEY CLUSTERED 
(
	[photoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO

-- positive scenario
CREATE view dbo.vw_photo
AS SELECT * FROM .photo a
GO

-- positive scenario
CREATE OR ALTER PROCEDURE [dbo].[sp_get_photo]
(
    @photo_id     INT,
    @user_id      INT         = 0,
    @rows           INT         = 20
)
AS 
BEGIN
    SELECT	*
	FROM	dbo.photo a WITH (NOLOCK) 
	INNER JOIN dbo.category c WITH (NOLOCK) 
		ON a.categoryid = c.categoryid
	WHERE	a.photoid = @photo_id; 
END
GO

-- positive scenario
ALTER FUNCTION [dbo].[fn_photo_sold] (
  @buyer_id AS INT,
  @sale_state TINYINT = 1,
  @max_rows AS INT
)
RETURNS VARCHAR(8000)
AS
BEGIN
  RETURN ''
END
GO

-- one negative scenario, add more files to test the other types
CREATE TABLE [dbo].[FileNameCorrespondsToSchemaObject_Tests](
	[photoId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[userId] [int] NOT NULL,
	[Title] [varchar](128) NULL,
 CONSTRAINT [PK_photo_detail] PRIMARY KEY CLUSTERED 
(
	[photoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
