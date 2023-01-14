CREATE TABLE ServerData(
    database_creation DATE NOT NULL, 
    version int
);
INSERT INTO ServerData VALUES  (NOW(), 1);


CREATE TABLE users
(
    id SERIAL PRIMARY KEY,
    steam VARCHAR(15),
    license VARCHAR(255),
    xbl VARCHAR(255),
    live VARCHAR(255),
    discord VARCHAR(255),
    license2 VARCHAR(255),
    ip VARCHAR(255) NOT NULL,
    banned timestamp without time zone,
    ban_reason text,
    last_connection timestamp without time zone,
    admin_level int DEFAULT 0,
    CONSTRAINT users_discord_key UNIQUE (discord),
    CONSTRAINT users_license2_key UNIQUE (license2),
    CONSTRAINT users_license_key UNIQUE (license),
    CONSTRAINT users_live_key UNIQUE (live),
    CONSTRAINT users_steam_key UNIQUE (steam),
    CONSTRAINT users_xbl_key UNIQUE (xbl)
);