USE [master]
GO
/****** Object:  Database [blog]    Script Date: 3/4/2025 4:11:14 pm ******/
CREATE DATABASE [blog]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'blog', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS01\MSSQL\DATA\blog.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'blog_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS01\MSSQL\DATA\blog_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [blog] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [blog].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [blog] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [blog] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [blog] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [blog] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [blog] SET ARITHABORT OFF 
GO
ALTER DATABASE [blog] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [blog] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [blog] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [blog] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [blog] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [blog] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [blog] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [blog] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [blog] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [blog] SET  DISABLE_BROKER 
GO
ALTER DATABASE [blog] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [blog] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [blog] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [blog] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [blog] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [blog] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [blog] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [blog] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [blog] SET  MULTI_USER 
GO
ALTER DATABASE [blog] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [blog] SET DB_CHAINING OFF 
GO
ALTER DATABASE [blog] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [blog] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [blog] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [blog] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [blog] SET QUERY_STORE = ON
GO
ALTER DATABASE [blog] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [blog]
GO
/****** Object:  Table [dbo].[message]    Script Date: 3/4/2025 4:11:15 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[message](
	[MessageID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](max) NOT NULL,
	[Email] [varchar](max) NOT NULL,
	[Subject] [varchar](max) NOT NULL,
	[Message] [varchar](max) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_message] PRIMARY KEY CLUSTERED 
(
	[MessageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Posts]    Script Date: 3/4/2025 4:11:16 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Posts](
	[PostID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](200) NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[AuthorID] [int] NOT NULL,
	[Category] [nvarchar](50) NOT NULL,
	[Status] [nvarchar](20) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[LastModifiedDate] [datetime] NOT NULL,
	[Views] [int] NOT NULL,
	[Featuredlmage] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK__Posts__AA12603877F59429] PRIMARY KEY CLUSTERED 
(
	[PostID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users]    Script Date: 3/4/2025 4:11:16 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[username] [varchar](50) NOT NULL,
	[password] [varchar](50) NOT NULL,
	[email] [varchar](50) NOT NULL,
	[fullname] [varchar](50) NOT NULL,
	[usertype] [varchar](50) NOT NULL,
	[resetcode] [int] NULL,
	[isActive] [bit] NULL,
	[createdDate] [datetime] NULL,
 CONSTRAINT [PK_users] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Posts] ADD  CONSTRAINT [DF__Posts__Status__5629CD9C]  DEFAULT ('Draft') FOR [Status]
GO
ALTER TABLE [dbo].[Posts] ADD  CONSTRAINT [DF__Posts__CreatedDa__571DF1D5]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Posts] ADD  CONSTRAINT [DF__Posts__LastModif__5812160E]  DEFAULT (getdate()) FOR [LastModifiedDate]
GO
ALTER TABLE [dbo].[Posts] ADD  CONSTRAINT [DF__Posts__Views__59063A47]  DEFAULT ((0)) FOR [Views]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT (getdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[Posts]  WITH CHECK ADD  CONSTRAINT [FK__Posts__AuthorID__59FA5E80] FOREIGN KEY([AuthorID])
REFERENCES [dbo].[users] ([UserID])
GO
ALTER TABLE [dbo].[Posts] CHECK CONSTRAINT [FK__Posts__AuthorID__59FA5E80]
GO
USE [master]
GO
ALTER DATABASE [blog] SET  READ_WRITE 
GO
