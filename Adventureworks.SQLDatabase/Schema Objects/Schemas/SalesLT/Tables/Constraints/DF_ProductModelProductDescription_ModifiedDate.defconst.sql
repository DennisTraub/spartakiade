﻿ALTER TABLE [SalesLT].[ProductModelProductDescription]
    ADD CONSTRAINT [DF_ProductModelProductDescription_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate];


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Default constraint value of GETDATE()', @level0type = N'SCHEMA', @level0name = N'SalesLT', @level1type = N'TABLE', @level1name = N'ProductModelProductDescription', @level2type = N'CONSTRAINT', @level2name = N'DF_ProductModelProductDescription_ModifiedDate';

