-- =============================================
-- Author:		Farzad E. (dNetGuru)
-- Create date: 
-- Description:	Simple T-SQL script to demonstrate PersianDate and PersianDateTime Types for MsSQL.
-- =============================================

-- •————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————•
-- | Code is published under terms mentioned on the project's page (on http://www.codeplex.com/PersianDate) under MIT Licence    -- |
-- •————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————•
-- •————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————•
-- | Copyright (c) 2008 Farzad E. (dNetGuru)                                                                                     -- |
-- |                                                                                                                             -- |
-- | Permission is hereby granted, free of charge, to any person obtaining a copy of this software and                           -- |
-- | associated documentation files (the "Software"), to deal in the Software without restriction, including                     -- |
-- | without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell                     -- |
-- | copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the                    -- |
-- | following conditions:                                                                                                       -- |
-- |                                                                                                                             -- |
-- | The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. |
-- |                                                                                                                             -- |
-- | THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED               -- |
-- | TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL               -- |
-- | THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF               -- |
-- | CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER                    -- |
-- | DEALINGS IN THE SOFTWARE.                                                                                                   -- |
-- •————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————•
-- •————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————•
-- | Keep in mind that this is the initial release, Please send me you comments, feedbacks and especially feature requests !     -- |
-- | Contact me via the Project's Page and/or mailto:farzade@gmail.com you can also particiapate in our community @ www.Hackerz.ir  |
-- •————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————•

-- =============================================
-- NOTE : Deploy the PersianDate assembly to the target Database before running this script.
-- =============================================

SP_CONFIGURE "CLR Enabled", 1
GO

------------------------------------------------------------
--=================- Creating Test Table -================--
------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Main]') AND type in (N'U'))
DROP TABLE [dbo].[Main]
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[Main](
	[ID] [uniqueidentifier] NOT NULL,
	[Date] [dbo].[PersianDate] NULL,
	[DateTime] [dbo].[PersianDateTime] NULL,
 CONSTRAINT [PK_Main] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
RECONFIGURE
GO
------------------------------------------------------------
--======================- TESTS -=========================--
------------------------------------------------------------
INSERT INTO Main  (ID, Date, DateTime)                -- My B-Day --
VALUES            (NEWID(), dbo.PersianDate::CreateDate(1368,06,31),dbo.PersianDateTime::CreateDateTime(1368,06,31,10,44,28))

INSERT INTO Main  (ID, Date, DateTime)
VALUES            (NEWID(), CONVERT(PersianDate, '1387/8/21'),CONVERT(PersianDateTime,'1387/8/21 15:23:17.183'))
------------------------------------------------------------
SELECT ID, Date as 'Raw PersianDate Object', Date.Year as Year, Date.Month as Month, Date.DayOfMonth as Day, Date.DayOfWeek as 'Day of Week',
	   Date.DayOfWeekName as 'Day Persian Name', Date.MonthName as 'Persian Month Name', Date.GetGeorgianDateTimeObj() as 'Georgian Equivalent Date',
	   Date.GetHashCode() as HashCode
FROM dbo.Main

SELECT ID, DateTime as 'Raw PersianDateTime Object', DateTime.Year as Year, DateTime.Month as Month, DateTime.DayOfMonth as Day, DateTime.DayOfWeek as 'Day of Week',
	   DateTime.Hour as 'Hour', DateTime.Minute as 'Minute', DateTime.Second as 'Seconds', DateTime.Millisecond as 'Milliseconds',
	   DateTime.DayOfWeekName as 'Day Persian Name', DateTime.MonthName as 'Persian Month Name', DateTime.GetGeorgianDateTimeObj() as 'Georgian Equivalent Date',
	   DateTime.GetHashCode() as HashCode, DateTime.Ticks as Ticks
FROM dbo.Main
------------------------------------------------------------
SELECT ID, Date.GetString(1) AS N'Date String', Date.ToString() AS 'Date' FROM dbo.Main WHERE (Date.DayOfMonth = 31 AND Date.Year = 1368) OR Date.DayOfWeekName = N'یکشنبه'
SELECT ID, DateTime.GetString(1), DateTime.DayOfWeek AS DoW FROM dbo.Main WHERE (DateTime.Hour = 15 AND DateTime.Second = 17) OR DateTime.Year = 1368
------------------------------------------------------------
--=====================- Cleanup -========================--
------------------------------------------------------------
DROP TABLE [dbo].[Main]