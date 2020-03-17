CREATE DATABASE IF NOT EXISTS `ai_company`;
USE `ai_company`;

-- Drop Tables

DROP TABLE IF EXISTS `COMPANY`;

-- Create Tables

CREATE TABLE IF NOT EXISTS `COMPANY` (
`id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
`mail` tinytext NOT NULL,
`hashed_password` text NOT NULL,
`name` varchar(50) NOT NULL,
`address` tinytext NOT NULL,
`description` TEXT NOT NULL,
PRIMARY KEY (`id`),
UNIQUE KEY `COMPANY_key_mail` (`mail`) USING HASH
)ENGINE=InnoDB DEFAULT CHARSET=utf8;