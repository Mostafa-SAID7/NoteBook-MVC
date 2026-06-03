-- Sample Data for NoteBook (Optional)
-- This script populates the database with sample data for testing and development
-- Run this only on development databases

-- Disable constraints temporarily
SET session_replication_role = replica;

-- Insert sample user
INSERT INTO application_users (id, email, user_name, password_hash, full_name, is_active)
VALUES (
    '00000000-0000-0000-0000-000000000001'::uuid,
    'demo@example.com',
    'demo',
    '$2a$11$demo-hash-value',
    'Demo User',
    true
) ON CONFLICT DO NOTHING;

-- Insert sample notes
INSERT INTO notes (id, title, content, tags, user_id, created_at, updated_at, is_deleted, is_archived)
VALUES 
(
    '550e8400-e29b-41d4-a716-446655440001'::uuid,
    'Welcome to NoteBook',
    'This is a sample note to help you get started. You can create, edit, and delete notes.',
    'welcome,getting-started',
    '00000000-0000-0000-0000-000000000001'::uuid,
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP,
    false,
    false
),
(
    '550e8400-e29b-41d4-a716-446655440002'::uuid,
    'My Important Project',
    'Details about an important project. This note is tagged with work and important tags.',
    'work,important,project',
    '00000000-0000-0000-0000-000000000001'::uuid,
    CURRENT_TIMESTAMP - INTERVAL '1 day',
    CURRENT_TIMESTAMP - INTERVAL '1 day',
    false,
    false
),
(
    '550e8400-e29b-41d4-a716-446655440003'::uuid,
    'Meeting Notes',
    'Notes from today''s meeting including action items and decisions made.',
    'meeting,work',
    '00000000-0000-0000-0000-000000000001'::uuid,
    CURRENT_TIMESTAMP - INTERVAL '3 days',
    CURRENT_TIMESTAMP - INTERVAL '3 days',
    false,
    true
) ON CONFLICT DO NOTHING;

-- Re-enable constraints
SET session_replication_role = default;

-- Verify data insertion
SELECT 'Users:' as category, COUNT(*) as count FROM application_users
UNION ALL
SELECT 'Notes:' as category, COUNT(*) as count FROM notes
UNION ALL
SELECT 'Tags:' as category, COUNT(*) as count FROM tags;
