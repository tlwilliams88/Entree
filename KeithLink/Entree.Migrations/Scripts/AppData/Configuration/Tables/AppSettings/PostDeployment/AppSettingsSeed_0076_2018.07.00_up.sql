declare @AppSettings as TABLE 
( 
    [Key]      VARCHAR (50)  NOT NULL,
    [Value]    VARCHAR (MAX) NOT NULL,
    [Comment]  VARCHAR (MAX) NOT NULL,
    [Disabled] BIT           DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Key] ASC)
)

INSERT INTO @AppSettings 
           ([Key],[Value],[Comment],[Disabled])
     VALUES
           ('MenuMaxSharedKey','GUgq%y3F?n];gX&X}bn@fg%D*.n!z`6mz@3xK`''efh8>Xz@;6Zt})9y$kv;$v[Tk[fP$''%qyNS!(wh*Jc[uBS*TKkr.w?]q,}AK;U,*;.JBdR^jN3K_4QUMxjyrMeY8MahduTxrjd>t,Ajy?3>cA~;)q,!<fgd9rcFqKedDPxDUK9JC`Ld7-+U%&;9jjxeGRhM2/b(&z@MdTP.c3w6kpnc8Q@-bjn;&<^D/Rmmdn[>!;4j_WL`8%B?jEB3?E!e7fd','MenuMaxSSOSharedKey',0)

MERGE INTO [Configuration].[AppSettings] A
USING @AppSettings B ON (A.[Key] = B.[Key])
WHEN MATCHED THEN
    UPDATE SET A.[Comment] = B.[Comment], A.[Value] = B.[Value], A.[Disabled] = B.[Disabled]
WHEN NOT MATCHED THEN
    INSERT (Comment, [Key], Value, [Disabled]) 
	  VALUES(B.[Comment],B.[Key],B.[Value],B.[Disabled]);