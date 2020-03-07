USE fake
GO

CREATE TYPE [dbo].[photo_tag_value_type] AS TABLE(
	[photo_id] [int] NOT NULL,
	[value] [nvarchar](200) NOT NULL
)
GO

CREATE TABLE [dbo].[photo_additional_data_type](
	[photo_additional_data_type_id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[photo_additional_data_type_name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_photo_additional_data_type_id] PRIMARY KEY CLUSTERED 
(
	[photo_additional_data_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[photo_additional_data](
	[photo_additional_data_id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[photo_id] [int] NOT NULL,
	[photo_additional_data_type_id] [int] NOT NULL,
	[content] [nvarchar](max) NOT NULL,
	[sort_order] [int] NULL,
 CONSTRAINT [PK_job_photo_additional_data_id] PRIMARY KEY CLUSTERED 
(
	[photo_additional_data_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE OR ALTER PROCEDURE [dbo].[spt_update_photo_tags]
(
    @tags [photo_tag_value_type] READONLY
)

AS 
BEGIN

    SET NOCOUNT ON;

    DECLARE @tag_metadata_type_id INT;

    SELECT TOP 1 @tag_metadata_type_id = [photo_additional_data_type_id]
    FROM [dbo].[photo_additional_data_type] WITH (NOLOCK)
    WHERE [photo_additional_data_type_name] = 'tag'

	DECLARE @photo_id BIGINT;

	SELECT TOP 1 @photo_id = [photo_id]
    FROM @tags

    CREATE TABLE #TagsUpdatesTempTable
    (
        photo_id BIGINT,
        photo_additional_data_type_id INT,
        content NVARCHAR(100),
        sort_order INT
    );

    INSERT INTO #TagsUpdatesTempTable(photo_id, photo_additional_data_type_id, content, sort_order) 
    select photo_id, @tag_metadata_type_id as photo_additional_data_type_id, value as content, NULL
    from @tags

    MERGE [dbo].[photo_additional_data] AS target 
    USING #TagsUpdatesTempTable AS source
    ON (target.photo_id = source.photo_id AND target.photo_additional_data_type_id = source.photo_additional_data_type_id AND source.sort_order = target.sort_order)
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (photo_id, photo_additional_data_type_id, content, sort_order)
        VALUES (photo_id, photo_additional_data_type_id, content, sort_order)
    WHEN NOT MATCHED BY SOURCE and target.photo_additional_data_type_id = @tag_metadata_type_id and target.photo_id = @photo_id THEN
        DELETE;

END 
GO
