-- /////////////////////////////////////////////////////////////////////////////////////////////////////////////
CREATE PROCEDURE `GetLatestDispatchMessageForSubscriber`(
	p_SubscriberId int
)
BEGIN
-- ============================================================================================================
-- Author:		Edgar Mondragón
-- Create date: 09/13/2016
-- Description:	Stored procedure that get Latest Dispatch Message for subscriber
-- ============================================================================================================
select ID, ArrivedOn, MessageHeader, MessageBody, SubscriberID, DestinationEmailAddress, OriginationEmailAddress, MessageSubject 
	from DispatchMessages
	where subscriberid = p_SubscriberId
	and ArrivedOn = (select max(ArrivedOn) from DispatchMessages where subscriberid = p_SubscriberId);
END

-- /////////////////////////////////////////////////////////////////////////////////////////////////////////////
CREATE PROCEDURE `GetSubscriberByOriginationDestination`(
	p_Origination varchar(2000),
	p_Destination varchar(2000) 
)
BEGIN
-- =================================================================================
-- Author:		Edgar Mondragón
-- Create date: 09/13/2016
-- Description:	Stored procedure that get subscribers by origination and destination
-- =================================================================================

/* SP Table Subscriber "ViewSubscribers*/
call SPGetSplitStringsSubscribers();
/* SP Table Origination emails "OriginationAdresses*/
call SPGetSplitStringsOrigination(p_Origination, ',');
/* SP Table Destination emails "DestinationAdresses*/
call SPGetSplitStringsDestination(p_Destination, ',');

SELECT DISTINCT
    S.id id, DA.EmailAddress
FROM
    tbl_subscribers as S
        INNER JOIN
    ViewSubscribers as OA ON S.id = OA.id
        INNER JOIN
    dispatchdestinationaddresses as DA ON S.id = DA.subscriberid
WHERE
    S.enableemailinput = 1
        AND S.statusid IN (1 , 3, 7)
        AND (OA.EmailAddress IN (SELECT 
            EmailAddress
        FROM
             OriginationAdresses)
        OR OA.EmailAddress IN (SELECT 
            RIGHT(EmailAddress, length(EmailAddress) - INSTR(EmailAddress, '@') +1)
        FROM
            OriginationAdresses))
        AND DA.EmailAddress IN (SELECT 
            EmailAddress
        FROM
            DestinationAdresses);
            
 DROP TABLE IF EXISTS OriginationAdresses; 
 DROP TABLE IF EXISTS DestinationAdresses;
END

-- /////////////////////////////////////////////////////////////////////////////////////////////////////////////
CREATE PROCEDURE `GetSubscriberInfoBySubscriberIdsInUnmatchedDispatch`(
	p_Destination varchar(2000)
)
BEGIN

CALL SplitCVSToTableOfBigInts(p_Destination, ',');

SELECT 
    s.Id,
    s.SubscriberName,
    ss.Id as StatusId,
    ss.Description AS StatusName,
    s.LoginName,
    s.mailingcountry,
    s.mailingstate,
    s.SubscriberTypeID,
    s.FullName
FROM
    tbl_subscribers s
        INNER JOIN
    subscriberstatus ss ON s.StatusID = ss.Id
WHERE
    s.Id IN (SELECT 
            ValueInt
        FROM
            ValueTable);
END

-- /////////////////////////////////////////////////////////////////////////////////////////////////////////////
CREATE PROCEDURE `SPGetSplitStringsDestination`(p_List longtext, p_Delimiter  NVARCHAR(255))
BEGIN
-- ============================================================================================================
-- Author:		Edgar Mondragón
-- Create date: 09/13/2016
-- Description:	Stored procedure that convert to xml destinacion email address and return mail in multiple rows
-- ============================================================================================================
DECLARE incitems INTEGER;
declare totalitems integer;

DROP TEMPORARY TABLE IF EXISTS TempList;
   CREATE TEMPORARY TABLE TempList(
		`id` MEDIUMINT NOT NULL AUTO_INCREMENT,
        `Item` LongText,
        PRIMARY KEY (id)
    );
    
    DROP TABLE IF EXISTS DestinationAdresses;
    CREATE TABLE DestinationAdresses(
        `EmailAddress` VARCHAR(256),
        `isAnEmailAddress` VARCHAR(128)
    );

Insert Into TempList (Item)
	SELECT CONVERT( CONCAT('<i>', REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(p_List
			, '&', '&amp;'), '"', '&quot;'), '\'', '&apos;'), '<', ''), '>', ''), p_Delimiter, '</i><i>'), '</i>') USING UTF8) Item;

set totalitems = (SELECT(LENGTH(Item) - LENGTH(REPLACE(Item, "</i>", "")))/ LENGTH("</i>") AS count
					FROM TempList);
SET incitems = 1;
REPEAT
	insert into DestinationAdresses
	SELECT 
		EXTRACTVALUE(A.Item, CONCAT('//i[', incitems, ']')) as Item,
		CASE INSTR(EXTRACTVALUE(A.Item, CONCAT('//i[', incitems, ']')), '@')
			WHEN 1 THEN 1
			ELSE 0
		END AS isAnEmailAddress
	FROM 
		TempList A;
	 SET incitems = incitems + 1;
	 UNTIL incitems >= totalitems + 1
END REPEAT;    
END

-- /////////////////////////////////////////////////////////////////////////////////////////////////////////////
CREATE   PROCEDURE `SPGetSplitStringsOrigination`(p_List longtext, p_Delimiter  NVARCHAR(255))
BEGIN

-- ============================================================================================================
-- Author:		Edgar Mondragón
-- Create date: 09/13/2016
-- Description:	Stored procedure that convert to xml origination email address and return mail in multiple rows
-- ============================================================================================================

DECLARE incitems INTEGER;
declare totalitems integer;

DROP TEMPORARY TABLE IF EXISTS TempList;
   CREATE TEMPORARY TABLE TempList(
		`id` MEDIUMINT NOT NULL AUTO_INCREMENT,
        `Item` LongText,
        PRIMARY KEY (id)
    );
    
    DROP TABLE IF EXISTS OriginationAdresses;
    CREATE TABLE OriginationAdresses(
        `EmailAddress` VARCHAR(256),
        `isAnEmailAddress` VARCHAR(128)
    );

Insert Into TempList (Item)
	SELECT CONVERT( CONCAT('<i>', REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(p_List
			, '&', '&amp;'), '"', '&quot;'), '\'', '&apos;'), '<', ''), '>', ''), p_Delimiter, '</i><i>'), '</i>') USING UTF8) Item;

set totalitems = (SELECT(LENGTH(Item) - LENGTH(REPLACE(Item, "</i>", "")))/ LENGTH("</i>") AS count
					FROM TempList);
SET incitems = 1;
REPEAT
	insert into OriginationAdresses
	SELECT 
		EXTRACTVALUE(A.Item, CONCAT('//i[', incitems, ']')) as Item,
		CASE INSTR(EXTRACTVALUE(A.Item, CONCAT('//i[', incitems, ']')), '@')
			WHEN 1 THEN 1
			ELSE 0
		END AS isAnEmailAddress
	FROM 
		TempList A;
	 SET incitems = incitems + 1;
	 UNTIL incitems >= totalitems + 1
END REPEAT;    
END

-- /////////////////////////////////////////////////////////////////////////////////////////////////////////////

CREATE   PROCEDURE `SPGetSplitStringsSubscribers`()
BEGIN
-- ========================================================================================================================================
-- Author:		Edgar Mondragón
-- Create date: 09/13/2016
-- Description:	Stored procedure that convert to xml originationemailaddress related with susbcriber table and return mail in multiple rows
-- ========================================================================================================================================
DECLARE incitems INTEGER;
DECLARE incrows INTEGER;
declare totalitems integer;
declare totalrows integer;
DROP TEMPORARY TABLE IF EXISTS TempList;
   CREATE TEMPORARY TABLE TempList(
		`id` MEDIUMINT NOT NULL AUTO_INCREMENT,
        `subscriberid` integer,
        `Item` LongText,
        PRIMARY KEY (id)
    );    
    DROP TEMPORARY TABLE IF EXISTS ViewSubscribers;
    CREATE TEMPORARY TABLE ViewSubscribers(
		`id` integer,
        `EmailAddress` VARCHAR(256),
        `isAnEmailAddress` VARCHAR(128)
    );
Insert Into TempList (subscriberid,Item)
	SELECT su.id, CONVERT( CONCAT('<i>', REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(su.originationemailaddress
			, '&', '&amp;'), '"', '&quot;'), '\'', '&apos;'), '<', ''), '>', ''), ',', '</i><i>'), '</i>') USING UTF8) Item
	FROM dispatcherapi.tbl_subscribers su where su.originationemailaddress is not null;
set totalrows = (select count(1) from dispatcherapi.tbl_subscribers where originationemailaddress is not null);      
set incrows = 1;
while totalrows >= incrows do
set totalitems = (SELECT(LENGTH(Item) - LENGTH(REPLACE(Item, "</i>", "")))/ LENGTH("</i>") AS count
				FROM TempList where id = incrows);
SET incitems = 1;
		 while totalitems >= incitems do
			insert into ViewSubscribers
			SELECT 
				A.subscriberid,
				EXTRACTVALUE(A.Item, CONCAT('//i[', incitems, ']')),
				CASE INSTR(EXTRACTVALUE(A.Item, CONCAT('//i[', incitems, ']')), '@') 
					WHEN 1 THEN 1
					ELSE 0
				END AS isAnEmailAddress
			FROM 
				TempList A 
                where id = incrows;
					 SET incitems = incitems + 1;
		END while;
	SET incrows = incrows + 1;
END while;
END

-- ////////////////////////////////////////////////////////////////////////////////////////////////////////////
CREATE   PROCEDURE `SplitCVSToTableOfBigInts`(
	p_String longtext, p_Delimiter nvarchar(10)
)
BEGIN
-- ============================================================================================================
-- Author:		Edgar Mondragón
-- Create date: 09/13/2016
-- Description:	SplitCVSToTableOfBigInts
-- ============================================================================================================
 declare p_NextValue integer;
 declare p_Pos double;
 declare p_NextPos double;
 declare p_CommaCheck nvarchar(1);
 
DROP TEMPORARY TABLE IF EXISTS ValueTable;
	CREATE TEMPORARY TABLE ValueTable (ValueInt integer);

 
if p_String != '' then
-- Initialize
-- set p_CommaCheck = right(p_String,1);

 -- Check for trailing Comma, if not exists, INSERT
 -- if (@CommaCheck <> @Delimiter )
 set p_String = concat(p_String , p_Delimiter);
 
 -- Get position of first Comma
 set p_Pos = INSTR(p_String, p_Delimiter);
 set p_NextPos = 1;
 
 
  -- Loop while there is still a comma in the String of levels
 while p_pos <>  0 do
 
  set p_NextValue = cast(substring(p_String, 1, p_Pos - 1) as UNSIGNED integer);
 
  insert into ValueTable Values (p_NextValue);
 
  set p_String = substring(p_String, p_pos + 1, LENGTH(p_String));
  
  set p_NextPos = p_Pos;
  set p_pos  = INSTR(p_String, p_Delimiter);
  
 end while; 
end if;

END