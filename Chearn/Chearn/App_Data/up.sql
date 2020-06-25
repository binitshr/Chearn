CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](128) NOT NULL,
	[RoleId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](128) NOT NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEndDateUtc] [datetime] NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[UserName] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[CUsers](
	[ID]		INT IDENTITY(1, 1)			NOT NULL,
	[DoB]		DATE						NOT NULL,
	[Email]		VARCHAR(256)				NOT NULL,
	[FirstName] VARCHAR(128)				NOT NULL,
	[LastName]	VARCHAR(128)				NOT NULL,
	[About]		VARCHAR(4096)						,
	[AspID]		NVARCHAR(128) FOREIGN KEY REFERENCES AspNetUsers(ID),
	PRIMARY KEY(ID)
)

CREATE TABLE [dbo].[Roles](
	[ID]		INT IDENTITY(1, 1)			NOT NULL,
	[Name]		VARCHAR(128)				NOT NULL,
	PRIMARY KEY(ID)
)

CREATE TABLE [dbo].[UserRoles](
	[ID]		INT IDENTITY(1, 1)			NOT NULL,
	[UserID]	INT	FOREIGN KEY REFERENCES CUsers(ID),
	[RoleID]	INT	FOREIGN KEY REFERENCES Roles(ID)
	PRIMARY KEY(ID)
)

CREATE TABLE [dbo].[Instructors](
	[ID]			INT IDENTITY(1, 1)			NOT NULL,
	[CUserID]		INT	FOREIGN KEY REFERENCES CUsers(ID),
	[HasDoctorate]	BIT
	PRIMARY KEY(ID)
)

CREATE TABLE [dbo].[Students](
	[ID]				INT IDENTITY(1, 1)	NOT NULL,
	[YearsOfEducation]	INT					NOT NULL,
	[CUserID]			INT	FOREIGN KEY REFERENCES CUsers(ID),
	[PointTotal]		INT
	PRIMARY KEY(ID)
)

CREATE TABLE [dbo].[Categories](
	[ID]			INT IDENTITY(1, 1)		NOT NULL,
	[Name]			NVARCHAR(32)			NOT NULL,
	PRIMARY KEY(ID)
)

CREATE TABLE [dbo].[Courses](
	[ID]			INT IDENTITY(1, 1)		NOT NULL,
	[InstructorID]	INT FOREIGN KEY REFERENCES Instructors(ID),
	[Name]			NVARCHAR(256)			NOT NULL,
	[Description]	NVARCHAR(MAX),
	[Tag] NVARCHAR(256),
	[Picture] image,
	[Valid] Bit,
	[Comfirmed] Bit,
	PRIMARY KEY(ID)
)

CREATE TABLE [dbo].[CourseInstructors](
	[ID] INT IDENTITY (1,1) Not Null,
	[InstructorID]	INT FOREIGN KEY REFERENCES Instructors(ID),
	[CourseID] INT FOREIGN KEY REFERENCES Courses(ID)
)

CREATE TABLE [dbo].[Lessons](
	[ID]			INT IDENTITY(1, 1)		NOT NULL,
	[Title]			VARCHAR(512)			NOT NULL,
	[MaterialA]		VARCHAR(MAX)			NOT NULL,
	[MaterialB]		VARCHAR(8000)			NOT NULL,
	[ImageLink]		VARCHAR(3000),
	[VideoLink]		VARCHAR(3000),
	[CourseID]		INT FOREIGN KEY REFERENCES Courses(ID)
	PRIMARY KEY(ID)
)

CREATE TABLE [dbo].[Questions](
	[ID]		INT IDENTITY(1, 1)			NOT NULL,
	[LessonID]	INT FOREIGN KEY REFERENCES Lessons(ID),
	[Text]		VARCHAR(2048)				NOT NULL,
	[Option1]   VARCHAR(256)				NOT NULL,
	[Option2]   VARCHAR(256)				NOT NULL,
	[Option3]   VARCHAR(256)				,
	[Option4]   VARCHAR(256)				,
	[Answer]	VARCHAR(256)				NOT NULL,
	[Points]	INT									
	PRIMARY KEY(ID)
)
CREATE TABLE [dbo].[Reviews](
	[ID]			INT IDENTITY(1, 1)		NOT NULL,
	[QuestionID]	INT FOREIGN KEY REFERENCES Questions(ID),
	[UserID]		NVARCHAR(128) FOREIGN KEY REFERENCES AspNetUsers(ID),
	[TimeStamp]		DATE					NOT NULL,
	[IsCorrect]		BIT						NOT NULL,
	[Level]			INT						NOT NULL
)
CREATE TABLE [dbo].[Edges](
	[ID]		INT IDENTITY(1, 1)		NOT NULL,
	[ChildID]	INT FOREIGN KEY REFERENCES Lessons(ID),
	[ParentID]	INT FOREIGN KEY REFERENCES Lessons(ID),
	PRIMARY KEY(ID)
)

CREATE TABLE [dbo].[StudentLessons](
	[ID]				INT IDENTITY(1, 1)	NOT NULL,
	[StudentID]			INT FOREIGN KEY REFERENCES Students(ID),
	[LessonID]			INT FOREIGN KEY REFERENCES Lessons(ID),
	[NumberCorrect]		INT					NOT NULL,
	[NumberOfTrials]	INT					NOT NULL,
	[NextReview]		DATE				NOT NULL,
	[SRSLevel]			INT					NOT NULL,
	PRIMARY KEY(ID)
)

CREATE TABLE [dbo].[Enrollments](
[ID]				INT IDENTITY(1, 1) NOT NULL,
[StudentID]			INT FOREIGN KEY REFERENCES Students(ID),
[CourseID]          INT FOREIGN KEY REFERENCES Courses(ID),
[Date]				DATE   NOT NULL,
PRIMARY KEY(ID)

)

CREATE TABLE [dbo].[ShopItems](
	[ID]			INT IDENTITY(1, 1)							NOT NULL,
	[Name]			NVARCHAR(256)								NOT NULL,
	[Description]	NVARCHAR(MAX),
	[OriginalPrice]	DECIMAL(8, 2),
	[Category]		INT FOREIGN KEY REFERENCES Categories(ID)	NOT NULL,
	PRIMARY KEY(ID)
)

CREATE TABLE [dbo].[ShopItemCourses](
	[ItemID]		INT FOREIGN KEY REFERENCES ShopItems(ID)	NOT NULL,
	[CourseID]		INT FOREIGN KEY REFERENCES Courses(ID)		NOT NULL,
	PRIMARY KEY(ItemID, CourseID)
)

CREATE TABLE [dbo].[OwnedItems](
	[UserID]		INT FOREIGN KEY REFERENCES CUsers(ID)		NOT NULL,
	[ItemID]		INT FOREIGN KEY REFERENCES ShopItems(ID)	NOT NULL,
	PRIMARY KEY (UserID, ItemID)
)

CREATE TABLE [dbo].[ItemTags](
	[ShopItemID]	INT FOREIGN KEY REFERENCES ShopItems(ID),
	[Tag]			NVARCHAR(24)			NOT NULL,
	PRIMARY KEY (ShopItemID, Tag)
)

CREATE TABLE [dbo].[TagScores](
	[Tag0]			NVARCHAR(24)			NOT NULL,
	[Tag1]			NVARCHAR(24)			NOT NULL,
	[Score]			INT,
	PRIMARY KEY(Tag0, Tag1)
) 
CREATE TABLE [dbo].[BlogPosts](
	[ID]			INT IDENTITY(1, 1)			NOT NULL,
	[Author]		VARCHAR(128)				NOT NULL,
	[PublishDate]	DATETIME					NOT NULL,
	[Title]			VARCHAR(1024)				NOT NULL,
	[Text]			VARCHAR(MAX)				NOT NULL
)
INSERT INTO [dbo]. [AspNetRoles] ([ID], [Name]) VALUES
	(0, 'Admin'),
	(1 ,'Instructor'),
	(2 ,'Student'),
	(3, 'Undecided')
