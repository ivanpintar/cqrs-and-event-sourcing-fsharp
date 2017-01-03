USE [PinetreeShopFSharp]
GO
/****** Object:  Schema [EventStore]    Script Date: 2.1.2017. 22:20:53 ******/
CREATE SCHEMA [EventStore]
GO
/****** Object:  Schema [Orders]    Script Date: 2.1.2017. 22:20:53 ******/
CREATE SCHEMA [Orders]
GO
/****** Object:  Schema [Products]    Script Date: 2.1.2017. 22:20:53 ******/
CREATE SCHEMA [Products]
GO
/****** Object:  Table [EventStore].[CommandEntity]    Script Date: 2.1.2017. 22:20:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [EventStore].[CommandEntity](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AggregateId] [uniqueidentifier] NOT NULL,
	[CommandId] [uniqueidentifier] NOT NULL,
	[CausationId] [uniqueidentifier] NOT NULL,
	[CorrelationId] [uniqueidentifier] NOT NULL,
	[CommandPayload] [nvarchar](max) NOT NULL,
	[QueueName] [nvarchar](max) NOT NULL,
	[ProcessId] [uniqueidentifier] NULL,
	[ExpectedVersion] [int] NULL,
 CONSTRAINT [PK_dbo.CommandEntity] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [EventStore].[EventEntity]    Script Date: 2.1.2017. 22:20:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [EventStore].[EventEntity](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AggregateId] [uniqueidentifier] NOT NULL,
	[EventId] [uniqueidentifier] NOT NULL,
	[CausationId] [uniqueidentifier] NOT NULL,
	[CorrelationId] [uniqueidentifier] NOT NULL,
	[EventPayload] [nvarchar](max) NOT NULL,
	[Category] [nvarchar](max) NOT NULL,
	[ProcessId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_dbo.EventEntity] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [Orders].[Line]    Script Date: 2.1.2017. 22:20:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Orders].[Line](
	[Id] [uniqueidentifier] NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[ProductName] [nvarchar](max) NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[Quantity] [int] NOT NULL,
	[Order_Id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Orders.Line] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [Orders].[Order]    Script Date: 2.1.2017. 22:20:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Orders].[Order](
	[Id] [uniqueidentifier] NOT NULL,
	[State] [nvarchar](max) NULL,
	[StreetAndNumber] [nvarchar](max) NULL,
	[ZipAndCity] [nvarchar](max) NULL,
	[StateOrProvince] [nvarchar](max) NULL,
	[Country] [nvarchar](max) NULL,
	[LastEventNumber] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Order] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [Products].[Product]    Script Date: 2.1.2017. 22:20:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Products].[Product](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[Quantity] [int] NOT NULL,
	[Reserved] [int] NOT NULL,
	[LastEventNumber] [int] NOT NULL,
 CONSTRAINT [PK_Products.Product] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [Orders].[Line]  WITH CHECK ADD  CONSTRAINT [FK_Orders.Line_Orders.Order_Order_Id] FOREIGN KEY([Order_Id])
REFERENCES [Orders].[Order] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [Orders].[Line] CHECK CONSTRAINT [FK_Orders.Line_Orders.Order_Order_Id]
GO