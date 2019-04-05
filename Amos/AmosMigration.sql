USE [Amos]
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'bdd327a3-5845-4cc5-bd49-af74943c8aa0', N'Admin')
GO
INSERT [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'0eb69405-19d5-417a-92d1-72156f45ccff', NULL, 0, N'ALnus8dXk24z2unTLyn4S2T16NQEji+LhJNDjKQ6cqkRhQ52zeHfacDVRgU7OkskZQ==', N'f40ed64e-1f33-48e7-9b2d-0b1b70919d5f', NULL, 0, 0, NULL, 0, 0, N'kcomerford@rktcreative.com')
GO
INSERT [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'83130bde-09e6-4ee8-872b-dc51319d4b42', NULL, 0, N'AL/8aKOCXhUN+wadBQ11HZvFSgk+m9lwFxdhRvCRUFzvCAgzeuGzF1EojhD5NXE0kw==', N'09c99ce1-156a-4dc2-8f90-44336b81ffe0', NULL, 0, 0, NULL, 0, 0, N'tsmale@rktcreative.com')
GO
INSERT [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'8793fc6b-a851-4585-991b-8f1ae6af07d8', NULL, 0, N'AP0HYM8gC25nzonwNLoYvRMG/TC75M2bdwvy9Jox9muFHqvMa4Jjk10QJnwHmxQO5A==', N'800c054b-0b73-4b21-b0f9-af8ed2a9914c', NULL, 0, 0, NULL, 0, 0, N'rkinnen@rktcreative.com')
GO
INSERT [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'aa6af4b1-87a1-4494-8951-3968808f3c20', NULL, 0, N'AKSokqx8G30X1a7++79qzifSoYj/ZLYMtQtWlsQq7yxnEHwiQec07uuyfuO+o4dILw==', N'53b6c8d4-2672-4021-b787-45983082c3a2', NULL, 0, 0, NULL, 0, 0, N'admin@kbrwyle.com')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0eb69405-19d5-417a-92d1-72156f45ccff', N'bdd327a3-5845-4cc5-bd49-af74943c8aa0')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'83130bde-09e6-4ee8-872b-dc51319d4b42', N'bdd327a3-5845-4cc5-bd49-af74943c8aa0')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'8793fc6b-a851-4585-991b-8f1ae6af07d8', N'bdd327a3-5845-4cc5-bd49-af74943c8aa0')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'aa6af4b1-87a1-4494-8951-3968808f3c20', N'bdd327a3-5845-4cc5-bd49-af74943c8aa0')
GO