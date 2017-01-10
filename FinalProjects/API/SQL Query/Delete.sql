delete from Devices

DBCC CHECKIDENT ('[Devices]', RESEED, 0);
GO