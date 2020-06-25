
INSERT INTO [dbo].[Categories] ([Name]) VALUES
('Programming'),--1
('Math'),--2
('Chemistry'),--3
('Engineering'),--4
('Physics')--5

INSERT INTO [dbo].[Courses] ([InstructorID], [Name]) VALUES
(NULL, 'Intro to Math'),--1
(NULL, 'Intermediate Math'),--2
(NULL, 'Advanced Math'),--3
(NULL, 'Intro to Python')--4

INSERT INTO [dbo].[ShopItems] ([Name], [Description], [OriginalPrice], [Category]) VALUES
('Math 101', 'Introduces the basics of algebra and other associated skills', 20.00, 2),--1
('Math 201', 'Introduces the basics of calculus', 40.00, 2),--2
('Math 301', 'Introduces the basics of mathematical proofs', 60.00, 2),--3
('Math Bundle', 'Interested in maths? Buy all 3 in a bundle and save!', 100.00, 2),--4
('Learn Python', 'Teaches the fundamentals of Python', 50.00, 1)--5

INSERT INTO [dbo].[ShopItemCourses] (ItemID, CourseID) VALUES
(1, 1),--Math 101
(2, 2),--Math 201
(3, 3),--Math 301
(4, 1),--Math bundle
(4, 2),
(4, 3),
(5, 4)--Python

INSERT INTO [dbo]. [AspNetRoles] ([ID], [Name]) VALUES
( 1 ,'Instructor'),
( 2 ,'Student'),
(3, 'Undecided')


INSERT INTO [dbo].[Courses] ([InstructorID], [Name]) VALUES
(1, 'Intro to Math'),
(1, 'Intermediate Math'),
(1, 'Advanced Math')

INSERT INTO [dbo].[Lessons] (Title, MaterialA, MaterialB, CourseID) VALUES
('The Numbers, one through ten', 'today we will examine what numbers are.
Numbers, for the uninitiated, are a way to represent the abstract notion of quantity. 
So if rocks exist, and we ask "How many?" what we are doing is playing the numbers game. 
You will learn later in this course how numbers greater than ten work, but for now we shall work on
sequencing numbers. The first ten numbers are one, two, three, four, five, six, seven, eight, nine, and ten.
You may have heard about zero, but do not worry about that until later. Memorize this list of words
in sequence, and later we shall examine the consequences of numbers', '', 2),
('Sequencing: Number after Number', 'Today we will look at what it means to be in sequential order,
and use that as the starting point for understanding addition. Numbers happen one after another.
As we learned in the last lesson, two comes after one, and three after two. Well, today we will 
learn the common nomenclature mathematicians use to describe these phenomena. 
We can consider the number directly after another number as adding to it. So rather than saying that
the number after two is three, we can alternatively look at it and say that 2 (two) + (plus) 1 (one) is 3 (three)
This opens up a whole new word of possible calculations, the consequences of which will be examined in the next lesson', '', 2),
('How to Add Single Digit Numbers','Welcome to intro to math. Today you will learn how to add numbers together.
Adding numbers might seem difficult at first, but it is important to remember the key secret
that makes addition simple. Begin by counting on your fingers up to the first number. 
So in the example of 4 + 5, count on your fingers to 4. Then, looking at the right side of the 
equation, count the other 5 fingers one at a time. Once you have done that, the number on your hand
represents the sum of two numbers added together', '', 2)
