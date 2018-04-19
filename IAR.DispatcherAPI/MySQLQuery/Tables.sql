CREATE TABLE `dispatchcenters` (
  `Id` int(11) NOT NULL,
  `apikey` varchar(100) DEFAULT NULL,
  `dispatchermastername` varchar(50) DEFAULT NULL,
  `dispatcheruserpassword` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `ix_tmp_autoinc` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `dispatchdestinationaddresses` (
  `Id` int(11) NOT NULL,
  `subscriberid` int(11) NOT NULL,
  `emailaddress` varchar(2000) NOT NULL,
  `name` varchar(1000) NOT NULL,
  `cr_isdeleted` tinyint(4) DEFAULT NULL,
  `cr_lastupdated` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `ix_tmp_autoinc` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


CREATE TABLE `dispatchmessages` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `arrivedon` datetime NOT NULL,
  `messageheader` varchar(2000) NOT NULL,
  `messagebody` varchar(8000) NOT NULL,
  `subscriberid` int(11) NOT NULL,
  `destinationemailaddress` varchar(2000) NOT NULL,
  `originationemailaddress` varchar(2000) NOT NULL,
  `messagesubject` varchar(1024) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `ix_tmp_autoinc` (`id`),
  KEY `idx_dispatchmessages` (`subscriberid`,`arrivedon`)
) ENGINE=InnoDB AUTO_INCREMENT=1610636042 DEFAULT CHARSET=utf8;

CREATE TABLE `dispatchunmatchedmessages` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `arrivedon` datetime NOT NULL,
  `messageheader` varchar(2000) NOT NULL,
  `messagesubject` varchar(1024) DEFAULT NULL,
  `messagebody` varchar(8000) DEFAULT NULL,
  `messagefrom` varchar(1000) DEFAULT NULL,
  `messageto` varchar(1000) DEFAULT NULL,
  `destsubscriberid` int(11) DEFAULT NULL,
  `destsubscribername` varchar(1000) DEFAULT NULL,
  `destsubscriberstatusid` int(11) DEFAULT NULL,
  `destsubscriberstatusname` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id`,`arrivedon`),
  KEY `ix_tmp_autoinc` (`id`),
  KEY `idx_unmacheddispatchmessagesbyarrival` (`arrivedon`)
) ENGINE=InnoDB AUTO_INCREMENT=1610613304 DEFAULT CHARSET=utf8;

CREATE TABLE `subscriberstatus` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `description` varchar(250) NOT NULL,
  `isactive` tinyint(4) DEFAULT '0',
  `cr_isdeleted` tinyint(4) DEFAULT NULL,
  `cr_lastupdated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `ix_tmp_autoinc` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8;


CREATE TABLE `tbl_subscribers` (
  `Id` int(11) NOT NULL,
  `subscribername` varchar(50) DEFAULT NULL,
  `usedispatchfromattachment` tinyint(4) DEFAULT '0',
  `enableemailinput` bit(1) DEFAULT NULL,
  `statusid` int(11) DEFAULT NULL,
  `loginname` varchar(50) DEFAULT NULL,
  `mailingcountry` varchar(50) DEFAULT NULL,
  `mailingstate` varchar(50) DEFAULT NULL,
  `subscribertypeid` int(11) DEFAULT NULL,
  `fullname` varchar(100) DEFAULT NULL,
  `originationemailaddress` varchar(2000) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `ix_tmp_autoinc` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
