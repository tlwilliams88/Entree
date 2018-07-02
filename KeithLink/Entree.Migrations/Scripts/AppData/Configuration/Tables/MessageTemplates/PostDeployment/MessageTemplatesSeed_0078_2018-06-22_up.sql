declare @Templates as TABLE 
( 
    [TemplateKey] NVARCHAR (50)  NULL,
    [Subject]     NVARCHAR (MAX) NULL,
    [IsBodyHtml]  BIT            NOT NULL,
    [Body]        NVARCHAR (MAX) NULL,
    [CreatedUtc]  DATETIME       DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc] DATETIME       DEFAULT (getutcdate()) NOT NULL,
    [Type]        INT            DEFAULT ((0)) NOT NULL
)


INSERT INTO @Templates
VALUES (
	'UserFeedbackNotice'
	, 'User Feedback to {Audience} from {CustomerName}'
	, 1
	, '{NotifHeader}

		<table style="width: 100%;">
			<tr>
				<td>User Name </td>
				<td>{UserFirstName} {UserLastName}</td>
			</tr>
			<tr>
				<td>User Email Address </td>
				<td>{SourceEmailAddress}</td>
			</tr>
			<tr>
				<td>Sales Rep Name </td>
				<td>{SalesRepName}</td>
			</tr>
		</table>
		<table style="width: 100%;">
			<tr>
				<td>Subject </td>
				<td>{Subject}</td>
			</tr>
			<tr>
				<td>Content </td>
				<td>{Content}</td>
			</tr>
		</table>
		'
	, getdate()
	, getdate()
	, 0
	)

MERGE INTO [Configuration].[MessageTemplates] A
USING @Templates B ON (A.[TemplateKey] = B.[TemplateKey])
WHEN MATCHED THEN
    UPDATE SET A.[Subject] = B.[Subject], A.[IsBodyHtml] = B.[IsBodyHtml], A.[Body] = B.[Body], A.[ModifiedUtc] = B.[ModifiedUtc], A.[Type] = B.[Type]
WHEN NOT MATCHED THEN
    INSERT ([TemplateKey],[Subject],[IsBodyHtml],[Body],[CreatedUtc],[ModifiedUtc],[Type]) 
	  VALUES(B.[TemplateKey],B.[Subject],B.[IsBodyHtml],B.[Body],B.[CreatedUtc],B.[ModifiedUtc],B.[Type]);