CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

/* TABLES */
CREATE TABLE chat_user (
    id UUID PRIMARY KEY,
    email text NOT NULL UNIQUE,
    created_at TIMESTAMP default CURRENT_TIMESTAMP,
    updated_at TIMESTAMP default CURRENT_TIMESTAMP,
    name TEXT NOT NULL,
    online BOOLEAN NOT NULL default false,
    connection_id TEXT,
    last_login_time TIMESTAMP NOT NULL default CURRENT_TIMESTAMP
);

CREATE TABLE message (
    id UUID PRIMARY KEY,
    user_id UUID References chat_user(id),
    created_at TIMESTAMP default CURRENT_TIMESTAMP,
    updated_at TIMESTAMP default CURRENT_TIMESTAMP,
    content TEXT NOT NULL
);

CREATE INDEX idx_created_at ON message (created_at);

/*TRIGGER FUNCTION*/
CREATE FUNCTION updated_at_stamp() 
RETURNS trigger 
AS $updated_at_stamp$
    BEGIN
        NEW.updated_at := CURRENT_TIMESTAMP;
        RETURN NEW;
    END
$updated_at_stamp$ LANGUAGE plpgsql;

/*TRIGGER STATEMENTS*/
CREATE TRIGGER updated_at
BEFORE UPDATE ON chat_user
FOR EACH ROW
EXECUTE FUNCTION updated_at_stamp();

CREATE TRIGGER updated_at
BEFORE UPDATE ON message
FOR EACH ROW
EXECUTE FUNCTION updated_at_stamp();

/*PROCEDURES*/


/* CREATE CHAT USER AND MESSAGE */

CREATE OR REPLACE FUNCTION create_user(
    p_name TEXT,
    p_email TEXT
)
RETURNS VOID
AS $$
BEGIN
    INSERT INTO chat_user (id, name, email, online)
    VALUES (uuid_generate_v4(), p_name, p_email, true);
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION create_message(
    p_user_id UUID,
    p_content TEXT
)
RETURNS VOID
AS $$
BEGIN
    INSERT INTO message (id, user_id, content)
    VALUES (uuid_generate_v4(), p_user_id, p_content);
END;
$$ LANGUAGE plpgsql;

/* FUNCTION TO HANDLE USER LOGINS */
CREATE OR REPLACE FUNCTION user_login(
    p_user_id UUID,
    p_connection_id TEXT
)
RETURNS VOID
AS $$
BEGIN
    UPDATE chat_user
    SET
        online = true,
        last_login_time = CURRENT_TIMESTAMP,
        connection_id = p_connection_id
    WHERE
        id = p_user_id;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION user_logout(
    p_user_id UUID
)
RETURNS VOID
AS $$
BEGIN
    UPDATE chat_user
    SET
        online = false,
        connection_id = ''
    WHERE
        id = p_user_id;
END;
$$ LANGUAGE plpgsql;

/* MAIN GET MESSAGES */
CREATE OR REPLACE FUNCTION get_paginated_messages_with_names(
    p_last_created_at TIMESTAMP DEFAULT NULL,
    p_page_size INT DEFAULT 20
)
RETURNS TABLE (
    name TEXT,
    content TEXT,
    created_at TIMESTAMP
)
AS $$
BEGIN
    RETURN QUERY
    SELECT
        chat_user.name,
        message.content,
        message.created_at
    FROM
        message 
    INNER JOIN
        chat_user ON message.user_id = chat_user.id
    WHERE
        (p_last_created_at IS NULL or message.created_at < p_last_created_at)
    ORDER BY
        message.created_at desc
    LIMIT
        p_page_size;
END;
$$ LANGUAGE plpgsql;

/* GET USER */
CREATE OR REPLACE FUNCTION get_chat_user(
    p_id UUID,
    p_email TEXT
)
RETURNS TABLE (
        id UUID,
        created_at TIMESTAMP,
        updated_at TIMESTAMP,
        name TEXT,
        email TEXT,
        online BOOLEAN,
        last_login_time TIMESTAMP,
        message_id UUID,
        message_created_at TIMESTAMP,
        message_updated_at TIMESTAMP,
        content TEXT
)
AS $$
BEGIN
    RETURN QUERY
    SELECT
        chat_user.id,
        chat_user.created_at,
        chat_user.updated_at,
        chat_user.name,
        chat_user.email,
        chat_user.online,
        chat_user.last_login_time,
        message.id,
        message.created_at,
        message.updated_at,
        message.content
    FROM
        chat_user
    LEFT JOIN
        message ON chat_user.id = message.user_id
    WHERE
        (chat_user.id = p_id OR p_id IS NULL)
        AND (chat_user.email = p_email OR p_email IS NULL)
    ORDER BY
        message.created_at;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION get_online_users() 
returns table (
    name text
)
AS $$
BEGIN
    RETURN QUERY
    SELECT chat_user.name
    FROM chat_user
    WHERE chat_user.online = true;
END;
$$ LANGUAGE plpgsql;

