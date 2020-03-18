CREATE DATABASE IF NOT EXISTS `ai_candidate`;
USE `ai_candidate`;

-- Drop Tables

DROP TABLE IF EXISTS `CANDIDATE_SKILL`;
DROP TABLE IF EXISTS `EXPERIENCE`;
DROP TABLE IF EXISTS `EDUCATION`;
DROP TABLE IF EXISTS `SKILL`;
DROP TABLE IF EXISTS `CANDIDATE`;

-- Create Tables

CREATE TABLE IF NOT EXISTS `CANDIDATE` (
`id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
`mail` tinytext NOT NULL,
`hashed_password` text NOT NULL,
`first_name` varchar(50) NOT NULL,
`last_name` varchar(50) NOT NULL,
`address` tinytext NOT NULL,
description TEXT NULL,
PRIMARY KEY (`id`),
UNIQUE KEY `USER_key_mail` (`mail`) USING HASH
)ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE IF NOT EXISTS `EXPERIENCE` (
`id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
`id_candidate` bigint(20) unsigned NOT NULL,
`title` TINYTEXT NOT NULL,
`description` TEXT NOT NULL,
`start` DATE NOT NULL,
`end` DATE,
PRIMARY KEY (`id`),
KEY `EXPERIENCE_key_candidate` (`id_candidate`),
CONSTRAINT `EXPERIENCE_key_candidate` FOREIGN KEY (`id_candidate`) REFERENCES `CANDIDATE` (`id`) ON DELETE CASCADE,
CONSTRAINT `EXPERIENCE_check_date` CHECK (`start` <= `end`)
)ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE IF NOT EXISTS `EDUCATION` (
`id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
`id_candidate` bigint(20) unsigned NOT NULL,
`title` TINYTEXT NOT NULL,
`description` TEXT NOT NULL,
`level` ENUM('L1','L2','L3','M1','M2','D1','D2','+') NOT NULL,
PRIMARY KEY (`id`),
KEY `EDUCATION_key_candidate` (`id_candidate`),
CONSTRAINT `EDUCATION_key_candidate` FOREIGN KEY (`id_candidate`) REFERENCES `CANDIDATE` (`id`) ON DELETE CASCADE
)ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE IF NOT EXISTS `SKILL`(
`id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
`title` TINYTEXT NOT NULL,
`type` ENUM('SOFT','TECHNICAL','OTHER') NOT NULL,
PRIMARY KEY (`id`)
)ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE IF NOT EXISTS `CANDIDATE_SKILL` (
`candidate` bigint(20) unsigned NOT NULL,
`skill` bigint(20) unsigned NOT NULL,
PRIMARY KEY (`candidate`,`skill`),
KEY `CANDIDATE_SKILL_key_candidate` (`candidate`),
CONSTRAINT `CANDIDATE_SKILL_fk_candidate` FOREIGN KEY (`candidate`) REFERENCES `CANDIDATE` (`id`) ON DELETE CASCADE,
KEY `CANDIDATE_SKILL_key_skill` (`skill`),
CONSTRAINT `CANDIDATE_SKILL_fk_skill` FOREIGN KEY (`skill`) REFERENCES `SKILL` (`id`) ON DELETE CASCADE
)ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- INSERT DATA

INSERT INTO SKILL (`title`, `type`) VALUES
('Agile', 'SOFT'),
('Decision-making', 'SOFT'),
('Management', 'SOFT'),
('C', 'TECHNICAL'),
('C++', 'TECHNICAL'),
('Java', 'TECHNICAL'),
('.NET', 'TECHNICAL'),
('VIDEO GAMES', 'OTHER');