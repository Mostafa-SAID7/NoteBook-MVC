-- Create users table
CREATE TABLE IF NOT EXISTS application_users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email VARCHAR(255) NOT NULL UNIQUE,
    user_name VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    full_name VARCHAR(255) NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    is_active BOOLEAN NOT NULL DEFAULT true,
    last_login_at TIMESTAMP NULL
);

-- Create notes table
CREATE TABLE IF NOT EXISTS notes (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    title VARCHAR(255) NOT NULL,
    content TEXT NOT NULL,
    tags TEXT DEFAULT '',
    user_id UUID NOT NULL REFERENCES application_users(id) ON DELETE CASCADE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    is_deleted BOOLEAN NOT NULL DEFAULT false,
    deleted_at TIMESTAMP NULL,
    is_archived BOOLEAN NOT NULL DEFAULT false,
    archived_at TIMESTAMP NULL
);

-- Create tags table
CREATE TABLE IF NOT EXISTS tags (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    user_id UUID NOT NULL REFERENCES application_users(id) ON DELETE CASCADE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    usage_count INTEGER NOT NULL DEFAULT 0,
    UNIQUE(user_id, name)
);

-- Create indexes for better query performance
CREATE INDEX IF NOT EXISTS idx_notes_user_id ON notes(user_id);
CREATE INDEX IF NOT EXISTS idx_notes_user_id_not_deleted ON notes(user_id) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_notes_user_id_not_archived ON notes(user_id, is_archived) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_notes_title_content_search ON notes USING GIN (to_tsvector('english', title || ' ' || content));
CREATE INDEX IF NOT EXISTS idx_tags_user_id ON tags(user_id);

-- Grant permissions to application user (create this user in your database first)
-- GRANT SELECT, INSERT, UPDATE, DELETE ON application_users, notes, tags TO notebook_user;
