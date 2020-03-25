CREATE DATABASE IF NOT EXISTS `ai_offer`;
USE `ai_offer`;

-- Drop Tables

DROP TABLE IF EXISTS `OFFER_SKILL`;
DROP TABLE IF EXISTS `OFFER_CANDIDATE`;
DROP TABLE IF EXISTS `OFFER`;

-- Create Tables

CREATE TABLE IF NOT EXISTS `OFFER` (
`id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
`id_company` bigint(20) unsigned NOT NULL,
`title` tinytext NOT NULL,
`description` TEXT NOT NULL,
`level` ENUM('TRAINEE','JUNIOR','SENIOR','OTHER'),
`type` ENUM('PART-TIME','FULL-TIME','OTHER') NOT NULL,
`wage` DECIMAL,
PRIMARY KEY (`id`)
)ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE IF NOT EXISTS `OFFER_SKILL` (
`offer` bigint(20) unsigned NOT NULL,
`skill` bigint(20) unsigned NOT NULL,
PRIMARY KEY (`offer`,`skill`),
KEY `OFFER_SKILL_key_offer` (`offer`),
CONSTRAINT `OFFER_SKILL_fk_candidate` FOREIGN KEY (`offer`) REFERENCES `OFFER` (`id`) ON DELETE CASCADE
)ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE IF NOT EXISTS `OFFER_CANDIDATE` (
`offer` bigint(20) unsigned NOT NULL,
`candidate` bigint(20) unsigned NOT NULL,
PRIMARY KEY (`offer`,`candidate`),
KEY `OFFER_CANDIDATE_key_offer` (`offer`),
CONSTRAINT `OFFER_CANDIDATE_fk_candidate` FOREIGN KEY (`offer`) REFERENCES `OFFER` (`id`) ON DELETE CASCADE
)ENGINE=InnoDB DEFAULT CHARSET=utf8;