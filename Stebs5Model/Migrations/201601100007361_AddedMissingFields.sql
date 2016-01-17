﻿EXECUTE sp_rename @objname = N'dbo.Files', @newname = N'FileSystemNodes', @objtype = N'OBJECT'
IF object_id('[PK_dbo.Files]') IS NOT NULL BEGIN
    EXECUTE sp_rename @objname = N'[PK_dbo.Files]', @newname = N'PK_dbo.FileSystemNodes', @objtype = N'OBJECT'
END
ALTER TABLE [dbo].[FileSystemNodes] ADD [Discriminator] [nvarchar](128) NOT NULL DEFAULT ''
ALTER TABLE [dbo].[FileSystemNodes] ADD [Folder_Id] [bigint]
ALTER TABLE [dbo].[FileSystemNodes] ADD [FileSystem_Id] [bigint]
ALTER TABLE [dbo].[FileSystems] ADD [Root_Id] [bigint]
CREATE INDEX [IX_Folder_Id] ON [dbo].[FileSystemNodes]([Folder_Id])
CREATE INDEX [IX_FileSystem_Id] ON [dbo].[FileSystemNodes]([FileSystem_Id])
CREATE INDEX [IX_Root_Id] ON [dbo].[FileSystems]([Root_Id])
ALTER TABLE [dbo].[FileSystemNodes] ADD CONSTRAINT [FK_dbo.FileSystemNodes_dbo.FileSystemNodes_Folder_Id] FOREIGN KEY ([Folder_Id]) REFERENCES [dbo].[FileSystemNodes] ([Id])
ALTER TABLE [dbo].[FileSystemNodes] ADD CONSTRAINT [FK_dbo.FileSystemNodes_dbo.FileSystems_FileSystem_Id] FOREIGN KEY ([FileSystem_Id]) REFERENCES [dbo].[FileSystems] ([Id])
ALTER TABLE [dbo].[FileSystems] ADD CONSTRAINT [FK_dbo.FileSystems_dbo.FileSystemNodes_Root_Id] FOREIGN KEY ([Root_Id]) REFERENCES [dbo].[FileSystemNodes] ([Id])
DROP TABLE [dbo].[Folders]
INSERT [dbo].[__MigrationHistory]([MigrationId], [ContextKey], [Model], [ProductVersion])
VALUES (N'201601100007361_AddedMissingFields', N'Stebs5Model.Migrations.Configuration',  0x1F8B0800000000000400DD5DD96EE436167D1F60FE41D0531238555ED28D1EC34EE0F69231A66D37BADCC1BC192C892E0BADA522B11C1B83F9B279984F9A5F1852A224EEA29652C94180C0259287978797CBBD246FFFEF3FFF3DF9E5250A9D67986641129FBA07B37DD781B197F841BC3A7537E8F1C70FEE2F3FFFF52F27977EF4E2FC56E63B22F970C9383B759F105A1FCFE799F7042390CDA2C04B932C7944332F89E6C04FE687FBFB7F9B1F1CCC2186703196E39C7CD9C4288860FE03FF3C4F620FAED1068437890FC38C7EC7298B1CD5B90511CCD6C083A7EE02C165F62ECFE73A676100B0080B183EBA0E88E3040184053CFE9AC1054A9378B558E30F20BC7F5D439CEF118419A4821FD7D96DDBB07F48DA30AF0B9650DE264349D412F0E0889232178B77A2D6AD48C3B45D627AD12B69754EDDA97B1584B8E16245C7E7614A3271ACCE48DE3D87F9B257753CD60FF2DF9E73BE09D12685A731DCA014E01C9F37CB30F0FE015FEF936F303E8D3721EE9E8F2083141FF7500EBC78CD108C6E312C2B3016F9739AAC618A5EA9C05825108C91EB94F2A558235DE706BC7C82F10A3D9DBAF84FD7B90A5EA05F7EA1DDFB350EB002E34228DDE09F73969879CD4C235F85A86D582B4A0CC21D4F0E4EE33E307C7D818F54E66B9F6F2C69AE5850A4999429DA721DA3F73FB9CE2DAE1C2C4358B1C9B47B819214FE0A63980204FDCF002198C60403E6F449B5DF82E7609517156A25DD9FB9CE1718E6A9D953B09654E48166BA4A93E84B12727D52A43D2C924DEA6111EF134D867B90AE20B297EB4B92A026B18A3C4AA9489249A83C5D255307B5CC47506BD524A58619DA67CB0CA77A888EB23F83BA8A4A8AFF3FF0F463D23D76C6E93B304A1DD30E8C5249BB296112FA30B556BE3CF7F8EB8996E8F3A720F45318AB69CEA57DA8F3302CF349F24017D27B0DF4524909B489E99B6A4B7096AD6F219A95056705E4558AE1FE48D26F331671CFB12E57F7D5A16D5F1D1D2C1F8F3EBC7B0FFCA3F73FC1A377E34F0E8AE17A70F8C16AB84A134AAF49E2F0DDFB416AD52A33DEE8A6EA9594EDEF079AAD56663955D267459641549A400DAFD625EAF4559B482AABB7322B69509791505631F66828E5DD6EBDD61A972F39840BDBB5AA2AF036B7F0E375F4650482708079CFA2166C083E066904AB567E4CB09681B8B5CC9F4196E161EFFF1D644FC3EFEB84CA16D0DBA4581B170844EBADD7F6F92989E1ED265A12551FAFAEC1BAE6FE8FE40ADB12497A199352BDF13E25DEB764832E63FF026FFDBF22AF04243FEF83C81E601071CE3C0F66D9155666E89F279BDAAB818D98A3C3D6706482DAF5BEE33C0441A4DE785493E84399A9DE768869D2A643CAD0D6846F30A36A7C36A34A40C666D20BC9646A2BE8A76415C44D0C96995402166906E16886F64E9050E39CA9A1691E9558799241AA227DB09D64AE26C36F2573D8E9EF257BFB5094D3CF703E945D6D44F3EE23956E7D39CC6BFA0D849BA1ABEA341AF2213FFC68C861A73F1A7231F1E7E720F74F59185865660C6F955F6DBB358F3941B2B18703D7CCB12B1F670ED00D97B32C4BBC201F059CFBB276F1F1C2E33DA3D3E0EF631C90D4137A83953B586375C6029CBAFBB3D981448A1EB772D66A1D9B02FE0F62CB993636345D74226B65D41EB5A8846C4B80D65D3D2A03F9418E858CFCA98E4DFB7FB06B3C771C64DD7699DC16CD57380575829A3C84B5ACBC9F9A97F4C01527D4BBF802861041E7CC2B8EC9CF41E6015F9E42F060F65B08A62051F63636F5119EE5614A0A01E27320A75B418CE4252188BD600DC24696849296DB37D2F6AA0E31E502AE614C2A6C64C2A672B573910850D523744A13432D1451321275BDADB718EBBE66FC7CA368A0D648D5E81FB551B6A2803A7E46503F1D0D36556B9DE0E32A1F33873777B6CA2360A184A6D5C0E849E8B8D67622823A1A9AE514BD0E53188582A343330AA96DB4E551C8F333EA28E4697843A3B0F02635F7B3E05A9A82EAF1DEACF1F71F1A7246D53B8E8389A95D611492CB84B8044C59D5BB58E6770C5F90C25F8225A42E934C79C1A9805D4024991599EBD486A862F69696021D10BDE563002BAC830640EAA99560F88D7B0388A8CC26C05AE11B40E99D04098819CC2DC42ACF1A8C72D18D580BD8D2016F84A52B8B00CB6824D3BFE2FD222697E60A9238489A3D14558314FA248DB966C7843D1C370C71011B42A47B6D0A468C6E0B3BC785B2110D7CE8FC146311525C3535F2213B31ACDC18FDD8E01C175B2543758949E6A3C9AB61EBD7609A426739031F06370483A398397B93221FB0CA9498ED6B3B0B9B6906A5DE4087D626D690518A3E201BEC326B624467F4D99B7D1D9951D8787603B1172DE51266A24465FED919801DA9104C368D9294A20FC8061D84263214E6889541D2910ADE841868EE288F43AA9D6F9576322FDE54D10F2773CDE3AB931BB05E07F18A798C45BF388BE225D6F98F8BF62F95A20263EE710C8BFBF4AA2694A460058554F23AC88757419AA10B80C01290C3A0733F92B209FB7CCDB6AFAC4CBD0397BBB0DC0996E5C8DF8C9A318F2E14D6102D74859B1511632A3F60D7AEA1328043DEC48110A48A53FDF324DC44B1DEBAD397AE5E5BB110D547250E4EF583DC9CCF4FBFCB27662CC045907969100531C08D54C19CCC053A24835062BB4D775C67E4EFBBC7EFF48F61BE7F335D54DCF862CB175F4667557EDC31450E65F5A467B5A32A68995ECF322DE7A07EF38F7A97D3AA7B86EE9A1D31AA5BD3EDB8E49C25EDD934177FEBD345C71E9177377D7BA7F23C75EF213D848EE7D297C932ADF36FEA51CAB3581645773EBBB31ED359D676BD54BBF5DA778FA1EC76460F7DDFC002D04F2D31982BF2121893668FCABF626031F9147B44E1A9020B2924B590927D90C009C92674C2D330AACE615F83FC0481459753ED91158F1158684572076C85CC629A3DAAE2BD020BAC48B6C7AE1F2F8833E684D727AD0FADFD02551C41F45BA13418DB990A8759E0980BD99C89597F6E8945AF5C4B60F4FB24D548EB656BAF46C591533F35D260E8E71AEE0E333FD5182F5EEB31B98BC9DC746EBA98ADC76BA7ACDB5609DE3B279878E241A0C9C066B2B533A489C3517C20A53E3494C9B19A4B289AFA349FA9BCBD5CDAFB049DE738A340B5BFE03A2317CDAB4BE656AD153DB1EDD5413A066DB2EACB7CFD15427364DA55236AB821B44273063B2DB530357948D5280E841B35A3C866EFD331F3CE9EF676A43D8718561BD8E3EC492983A6ADBDD5407DCED9606B7379ADAD6A05F3A643CC494C14A6F3DA49E947CFC9423AD813B3549B9AEA804F38C83BA1876ACDA116A553B6228BEB94BB334C7BDE9219C9305BFC1E9E87417E625466B80171F0083354BCF1730FF70F0E85A08DD309A038CF329F0BDAD1187E8DEFBA111EEC2E03BCCF2EA776D3ABDC1E518CE267907A4F20FD2E022FDFB34856AF61F9A88DBDB084C3191E517E40D8B2C9CC568EE756BA0B7A1DFBF0E5D4FD575EF0D8B9FEE7435576CFB94BF15FC7CEBEF3EFB6CD13E681F642B0E5ED05E91201F34FA3E7D5DADC9A6D5A720B3CEB0F87C6607AB0F1A49A429A99258D267FE59FF7F0B2F9350E7EDFE0847B4C28A1588C69324C6028F371CF44C378D9B38AF5B528CAAB6BCF1EE6837BB592A628DA439A0E21BFDEEE50E2826B295185A1D03D96D6324083C4D1EAB5D02B6365F54254C4C31A0A6F100A75F1AEBA6069635DF9F827CA635DB56BAC3AF65517D1B471AFF255B767D42BFB09A82C39CC22F3F676720D474763CC8D5BD9CD4D6B7994420EF533A4A4B0422DE07A840EEAA0196F2CEACE60CBB422A8CE60D8BB54ED2D44D2119E2CF6887562C02DFC495D63C8381D1FE0365D641DFEE1ADA2D536B51A4FEA4678EEDD21F6D0D0E1865A6ADFE01A32BE76D8694693237E5CEDB08CCBB4ADA9A4738CA737377D584609D21EDD8DA0161DE3554D254455FD167EB791A9C60C4665B885FDE66350ED34E8882290C12EE34D8DAD53BA8B93938D6A63115B6AE8AD4E4B457D931B9C760A3885BD8DF999F5A4E7191AD9649711B5C69E6774376B273BCF5846CF9A8C4EED6A43B4338DB2DE0DED3C2E96FCAC5EEC503E52926872D58E238D4D565C203A75FD658255A0704135458DD157DA106ACB5C597345BCE12055C527AB2A2BA2A4AB4337E82AAB0781B6C23A8BBE527DCC08B162662A906A64D2CC55B56B1FDD9F1A1B48F398ABD5049131D54DD72C63DD348FB96E4D6C9211A27E29079DEAEEB729FE88CA776A8D34A5385F1AB15B347EB4860F19CF6B98BEB7246F7A91BB9481725461071BB6219AA751CA0086D389D4C5C9CD7CB56C3A37FF6A5E170ED9F8410373F51BF92D299C50E0AD9E7DCEAD7B9AA78043367EC8385B3D9B3EE4486F11574BBE668FF7EB9B985CC9297E5DC02C58D510E4F9400C3D6EA75EE5B98E1F93D26010242AB30847C83710011F6FE3CF52143C020FE164721B27FFC75768489CCB6809FDEBF86E83D61B849B0CA365C89DC813C3C3547F1E3C8C97F9E46E9DFFAB614334018B19905B4C77F1C70DDE1655725F290EAD3510C4A2A1574E485F2272F564F55A21DD26B12510A5AF32C4EE61B40E315876172FC033EC221B56BF4F7005BCD7FA8A820EA4B92378DA4F2E02B04A4194518CBA3CFE8975D88F5E7EFE3F20DCF36FFE830000 , N'6.1.3-40302')
