EXEC sp_MSforeachtable 'DISABLE TRIGGER ALL ON ?'
GO
EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'
GO
EXEC sp_MSforeachtable 'SET QUOTED_IDENTIFIER ON; DELETE FROM ?'
GO
EXEC sp_MSforeachtable 'ALTER TABLE ? CHECK CONSTRAINT ALL'
GO
EXEC sp_MSforeachtable 'ENABLE TRIGGER ALL ON ?'
GO
