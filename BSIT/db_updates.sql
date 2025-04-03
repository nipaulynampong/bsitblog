-- Create Categories table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Categories' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Categories](
        [CategoryID] [int] IDENTITY(1,1) NOT NULL,
        [CategoryName] [nvarchar](100) NOT NULL,
        [Description] [nvarchar](500) NULL,
        [Slug] [nvarchar](100) NOT NULL,
        [ParentCategoryID] [int] NULL,
        [CreatedDate] [datetime] NOT NULL DEFAULT (getdate()),
        CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED ([CategoryID] ASC),
        CONSTRAINT [UQ_CategoryName] UNIQUE ([CategoryName]),
        CONSTRAINT [UQ_CategorySlug] UNIQUE ([Slug])
    )
    
    -- Add foreign key for parent category
    ALTER TABLE [dbo].[Categories] ADD CONSTRAINT [FK_Categories_ParentCategory]
    FOREIGN KEY ([ParentCategoryID]) REFERENCES [dbo].[Categories] ([CategoryID])
END

-- Create Tags table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Tags' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Tags](
        [TagID] [int] IDENTITY(1,1) NOT NULL,
        [TagName] [nvarchar](50) NOT NULL,
        [Slug] [nvarchar](50) NOT NULL,
        CONSTRAINT [PK_Tags] PRIMARY KEY CLUSTERED ([TagID] ASC),
        CONSTRAINT [UQ_TagName] UNIQUE ([TagName]),
        CONSTRAINT [UQ_TagSlug] UNIQUE ([Slug])
    )
END

-- Create PostTags relationship table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PostTags' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[PostTags](
        [PostID] [int] NOT NULL,
        [TagID] [int] NOT NULL,
        CONSTRAINT [PK_PostTags] PRIMARY KEY CLUSTERED ([PostID] ASC, [TagID] ASC),
        CONSTRAINT [FK_PostTags_Posts] FOREIGN KEY ([PostID]) REFERENCES [dbo].[Posts] ([PostID]) ON DELETE CASCADE,
        CONSTRAINT [FK_PostTags_Tags] FOREIGN KEY ([TagID]) REFERENCES [dbo].[Tags] ([TagID]) ON DELETE CASCADE
    )
END

-- Add new columns to Posts table if they don't exist
IF NOT EXISTS (SELECT * FROM syscolumns WHERE id=OBJECT_ID('Posts') AND name='Excerpt')
BEGIN
    ALTER TABLE [dbo].[Posts] ADD [Excerpt] [nvarchar](500) NULL
END

IF NOT EXISTS (SELECT * FROM syscolumns WHERE id=OBJECT_ID('Posts') AND name='PublishDate')
BEGIN
    ALTER TABLE [dbo].[Posts] ADD [PublishDate] [datetime] NULL
END

IF NOT EXISTS (SELECT * FROM syscolumns WHERE id=OBJECT_ID('Posts') AND name='MetaTitle')
BEGIN
    ALTER TABLE [dbo].[Posts] ADD [MetaTitle] [nvarchar](100) NULL
END

IF NOT EXISTS (SELECT * FROM syscolumns WHERE id=OBJECT_ID('Posts') AND name='MetaDescription')
BEGIN
    ALTER TABLE [dbo].[Posts] ADD [MetaDescription] [nvarchar](160) NULL
END

IF NOT EXISTS (SELECT * FROM syscolumns WHERE id=OBJECT_ID('Posts') AND name='Slug')
BEGIN
    ALTER TABLE [dbo].[Posts] ADD [Slug] [nvarchar](200) NULL
END

IF NOT EXISTS (SELECT * FROM syscolumns WHERE id=OBJECT_ID('Posts') AND name='AllowComments')
BEGIN
    ALTER TABLE [dbo].[Posts] ADD [AllowComments] [bit] NOT NULL DEFAULT (1)
END

-- Generate slugs for existing posts that don't have one
IF EXISTS (SELECT * FROM syscolumns WHERE id=OBJECT_ID('Posts') AND name='Slug')
BEGIN
    -- Update posts with NULL slug values
    UPDATE Posts 
    SET Slug = LOWER(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
        Title, ' ', '-'), '.', ''), ',', ''), ':', ''), ';', ''), '?', '')) + '-' + CAST(PostID AS nvarchar(10))
    WHERE Slug IS NULL OR Slug = ''
END

-- Add constraint for unique slugs
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UQ_PostSlug' AND xtype='UQ')
BEGIN
    -- Only add the constraint if we don't have NULL values
    IF NOT EXISTS (SELECT TOP 1 1 FROM Posts WHERE Slug IS NULL)
    BEGIN
        ALTER TABLE [dbo].[Posts] ADD CONSTRAINT [UQ_PostSlug] UNIQUE ([Slug])
    END
END

-- Insert default categories
IF NOT EXISTS (SELECT TOP 1 * FROM Categories)
BEGIN
    INSERT INTO [dbo].[Categories] ([CategoryName], [Description], [Slug], [ParentCategoryID])
    VALUES 
    ('Uncategorized', 'Default category for posts', 'uncategorized', NULL),
    ('Technology', 'Posts about technology', 'technology', NULL),
    ('Programming', 'Programming related posts', 'programming', 2),
    ('Web Development', 'Web development posts', 'web-development', 3),
    ('Database', 'Database related posts', 'database', 3),
    ('News', 'News and announcements', 'news', NULL),
    ('Tutorials', 'Tutorials and how-to guides', 'tutorials', NULL)
END

-- Insert some common tags
IF NOT EXISTS (SELECT TOP 1 * FROM Tags)
BEGIN
    INSERT INTO [dbo].[Tags] ([TagName], [Slug])
    VALUES 
    ('ASP.NET', 'asp-net'),
    ('VB.NET', 'vb-net'),
    ('SQL Server', 'sql-server'),
    ('HTML', 'html'),
    ('CSS', 'css'),
    ('JavaScript', 'javascript'),
    ('Bootstrap', 'bootstrap'),
    ('BSIT', 'bsit'),
    ('Web Design', 'web-design'),
    ('Blogging', 'blogging')
END 