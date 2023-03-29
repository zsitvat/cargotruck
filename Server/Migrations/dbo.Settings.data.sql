SET IDENTITY_INSERT [dbo].[Settings] ON
INSERT INTO [dbo].[Settings] ([Id], [SettingName], [SettingValue], [Date]) VALUES (1, N'CurrencyExchangeWaitTime', N'3600', N'2023-02-25 17:49:36')
INSERT INTO [dbo].[Settings] ([Id], [SettingName], [SettingValue], [Date]) VALUES (3, N'ExchangeApiKey', N'XwSDWGpGqDKu8ZIbOl56Kne74V14oEpC', N'2023-02-25 00:00:00')
SET IDENTITY_INSERT [dbo].[Settings] OFF



//add settings for currency api
migrationBuilder.Sql("SET IDENTITY_INSERT [dbo].[Settings] ON\r\nINSERT INTO [dbo].[Settings] ([Id], [SettingName], [SettingValue], [Date]) VALUES (1, N'CurrencyExchangeWaitTime', N'3600', N'2023-02-25 00:00:00')\r\nINSERT INTO [dbo].[Settings] ([Id], [SettingName], [SettingValue], [Date]) VALUES (3, N'ExchangeApiKey', N'05NabWkvhpEG1pI2IOX2n2fJsdK2zjUO', N'2023-02-25 00:00:00')\r\nSET IDENTITY_INSERT [dbo].[Settings] OFF\r\n");
