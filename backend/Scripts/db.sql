CREATE TABLE users (
    id UUID PRIMARY KEY,
    created_at TIMESTAMP default CURRENT_TIMESTAMP,
    updated_at TIMESTAMP default CURRENT_TIMESTAMP,
    name TEXT NOT NULL,
    logged_in BOOLEAN NOT NULL,
    last_login_time TIMESTAMP NOT NULL
);

CREATE TABLE Message (
    id UUID PRIMARY KEY,
    user_id UUID References users(id),
    created_at TIMESTAMP default CURRENT_TIMESTAMP,
    updated_at TIMESTAMP default CURRENT_TIMESTAMP,
    content TEXT NOT NULL
);
