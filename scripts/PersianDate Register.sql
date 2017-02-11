-- =============================================
-- Author:		Farzad E. (dNetGuru)
-- Create date: Monday, November 03 2008
-- Description:	Deploys the PersianDate assembly to the target Database.
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
-- NOTE: Do not forget to change the @Path variable !
-- =============================================
SP_CONFIGURE "CLR Enabled", 1
GO
DECLARE @Path NVARCHAR(1024)
SET @Path = 'C:\Users\dNetGuru\Desktop\SandBox\dNetTools.PersianDate.dll' -- TODO : Change this to a Proper value !
CREATE ASSEMBLY dNetSqlClassLibrary
FROM @Path WITH PERMISSION_SET = SAFE
GO
IF  EXISTS (SELECT * FROM sys.assembly_types at INNER JOIN sys.schemas ss on at.schema_id = ss.schema_id WHERE at.name = N'PersianDate' AND ss.name=N'dbo')
DROP TYPE [dbo].[PersianDate]
CREATE TYPE [dbo].[PersianDate]
EXTERNAL NAME [dNetSqlClassLibrary].[PersianDate]
GO
IF  EXISTS (SELECT * FROM sys.assembly_types at INNER JOIN sys.schemas ss on at.schema_id = ss.schema_id WHERE at.name = N'PersianDateTime' AND ss.name=N'dbo')
DROP TYPE [dbo].[PersianDateTime]
CREATE TYPE [dbo].[PersianDateTime]
EXTERNAL NAME [dNetSqlClassLibrary].[PersianDateTime]